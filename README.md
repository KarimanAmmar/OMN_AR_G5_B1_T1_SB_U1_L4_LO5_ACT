# OMN ICT - Systems README

This document explains the gameplay systems and editor utilities implemented under `Assets/Systems`.

For convenience, two managers are exposed through `Oman Tools` editor menu items:
- `Oman Tools/Score Manager`
- `Oman Tools/Game Sequence Manager`

And when you add `GameSequenceManager` using its tool, it will also ensure `DependenciesManager` exists on the same GameObject (per your requirement).

---

## High-Level Flow

The intended quiz runtime flow is:
1. `GameSequenceManager` owns the question order and the currently running `SubQuestionHolder`.
2. Each `SubQuestionHolder` declares a list of dependency names (strings).
3. `DependenciesManager` creates dependency GameObjects on-demand and tracks whether each dependency is complete.
4. When the currently active sub-question’s dependencies are complete, `GameSequenceManager` advances to the next sub-question / question.
5. In parallel, `ScoreManager` tracks the score and wrong attempts, and raises events based on correct/wrong answer outcomes.

---

## Score System (`ScoreManager`)

### Responsibility
`ScoreManager` is responsible for scoring the quiz:
- Counts `score` for correct answers.
- Limits wrong attempts using `wrongAttemptsPerQuestion`.
- Advances question counters using `currentQuestionNumber` and `answeredQuestions`.
- Fires events so other systems can react to scoring changes:
  - `onCorrectAnswer`
  - `onWrongAttempt`
  - `onWrongAnswer`

### Key Inspector Fields
Quiz setup (configuration):
- `numberOfQuestions` (min 1)
- `wrongAttemptsPerQuestion` (min 1)

Runtime counters (read-only):
- `score`
- `currentQuestionNumber`
- `currentWrongAttempts`
- `answeredQuestions`

Answer feedback UI references:
- `emoji_happy`, `emoji_neutral`, `emoji_sad`
- `answers_slices` (array)
- `correct_color`, `wrong_color`

Events:
- `onCorrectAnswer` (UnityEvent2)
- `onWrongAttempt` (UnityEvent2)
- `onWrongAnswer` (UnityEvent2)

### Core Methods
- `ResetScoreManager()`
  - Resets score and counters back to the initial state.
- `CorrectAnswer()`
  - If quiz is already finished (`IsQuizFinished`), returns early.
  - Checks whether `currentWrongAttempts >= wrongAttemptsPerQuestion`.
  - Shows the correct UI slice, plays “happy” emoji feedback with delayed toggling, increments `score`, invokes `onCorrectAnswer`, and calls `CompleteCurrentQuestion()`.
- `WrongAttempt()`
  - If quiz is already finished, returns early.
  - Shows the “sad” emoji briefly, increments `currentWrongAttempts`, invokes `onWrongAttempt`.
  - If wrong attempts reached the limit, shows wrong slice and invokes `onWrongAnswer` (and the question completion logic is intentionally commented out in the existing code).
- `CompleteCurrentQuestion()`
  - Increments `answeredQuestions`, resets `currentWrongAttempts`, and increments `currentQuestionNumber` unless quiz is finished.

### Oman Tools integration
Menu item: `Oman Tools/Score Manager`
- **Add ScoreManager to Selected**
  - Adds `ScoreManager` component to `Selection.activeGameObject`.
- **Create ScoreManager in Scene**
  - Creates a new GameObject named `ScoreManager` and attaches `ScoreManager`.

### Where it lives
- Runtime: `Assets/Systems/Score/ScoreManager.cs`
- Editor tool: `Assets/Systems/Score/Editor/ScoreManagerWindow.cs`

---

## Game Sequence System (`GameSequenceManager`)

### Responsibility
`GameSequenceManager` controls the progression of the quiz by:
- Holding the ordered list of questions (`questions`).
- Holding the currently active `QuestionHolder` and `SubQuestionHolder`.
- Starting sub-questions after a `beforeDelay`.
- Advancing automatically when the active sub-question is “ready” (dependencies completed).
- Providing quiz navigation helpers in inspector:
  - “Get Next/Previous” inspector buttons (editor-only)
  - “GoToQuestion” by `targetQuestionId`
- Playing hint visuals:
  - `hintButton` -> calls `PlayHint()`

Internally it relies on `DependenciesManager` to check dependency readiness.

### Data Model
`GameSequenceManager` contains serialized data classes:

#### `QuestionHolder`
- `id` (string): unique identifier for the question
- `subQuestions` (List):
  - Reorderable list of `SubQuestionHolder`

#### `SubQuestionHolder`
Core execution setup:
- `trialsNo` (int): number of wrong trials allowed before completing the sub-question
- `beforeDelay` (float seconds): delay before starting execution
- `playNarration` + `narrationId`:
  - When enabled, `audioManager.PlaySFX(narrationId)` is called on start
- Hint control:
  - `playHint` (bool)
  - `QuestionHint` (Animator): enabled by `PlayHint()`

Dependencies:
- `dependencies` (List<string>):
  - Each entry is a dependency name.
  - `GenerateDependencies()` creates dependency objects in `DependenciesManager`.
  - `RemoveDependency(string item)` removes it from both:
    - the `dependencies` list
    - `DependenciesManager`
- Readiness gate:
  - `IsReady()` checks every declared dependency and returns `true` only if each dependency’s `SimpleDependency.IsCompleted()` is true.

Events:
- `OnQuestionStart`
- `OnWrongAttempt`
- `OnWrong`
- `OnSuccess`
- `OnQuestionEnd`

### Runtime Variables
- `currentQuestionHolder`
- `currentSubQuestionHolder`

### Core Runtime Behavior
- `Awake/OnValidate` singleton wiring
  - `Instance` is set so other scripts can reference the active manager.
- `Start()`
  - Adds a listener to `hintButton.onClick` if `hintButton != null`
  - Calls `SelectNextQuestion()` to begin the sequence.
- `FixedUpdate()`
  - If `currentSubQuestionHolder != null` and `currentSubQuestionHolder.IsReady()`:
    - Computes whether this is the last sub-question and last question.
    - Calls `SelectNextQuestion()` when not at the end.

Sequence navigation:
- `SelectNextQuestion()`
  - If no question is selected yet:
    - Sets `currentQuestionHolder = questions[0]`
    - Starts first sub-question (`subQuestions[0]`)
  - If already in-progress:
    - Starts the next sub-question in the current question if possible
    - Otherwise moves to the next question (and its first sub-question)
  - If at the end:
    - Logs “No more questions.”

Sub-question start:
- `StartQuestion(subQuestion)` calls a coroutine:
  - Ends previous sub-question (`OnQuestionEnd`)
  - Assigns `currentSubQuestionHolder`
  - Waits `beforeDelay`
  - Plays narration if enabled
  - Invokes `OnQuestionStart`

Sub-question outcome:
- `Wrong()`
  - Decrements `trialsNo` after invoking `OnWrongAttempt`
  - If `trialsNo` reaches 0:
    - Invokes `OnWrong`
    - Marks `isCompleted = true`
    - Advances to the next sub-question/question
- `Success()`
  - Invokes `OnSuccess`
  - Marks `isCompleted = true`
  - Advances to the next question/sub-question

Hint:
- `PlayHint()`
  - If `currentSubQuestionHolder.playHint` is false: returns
  - If `QuestionHint != null`: enables it

### Oman Tools integration
Menu item: `Oman Tools/Game Sequence Manager`
- **Add GameSequenceManager (and DependenciesManager) to Selected**
  - Adds `GameSequenceManager` if missing.
  - Adds `DependenciesManager` if missing.
  - Ensures both are on the SAME GameObject.
- **Create GameSequenceManager in Scene**
  - Creates a new GameObject named `Game Sequence Manager`.
  - Attaches both `DependenciesManager` and `GameSequenceManager`.

### Where it lives
- Runtime: `Assets/Systems/Game Sequence Manager/GameSequenceManager.cs`
- Editor tool: `Assets/Systems/Game Sequence Manager/Editor/GameSequenceManagerWindow.cs`

---

## Dependencies System (`DependenciesManager`)

### Responsibility
`DependenciesManager` manages the dependencies required by sub-questions:
- Creates dependency GameObjects (`CreateDependency(string name)`)
- Removes them (`RemoveDependency(string name)`)
- Completes them (`CompleteDependency(string name)`)

Dependencies are identified by their name strings (the same strings stored in each `SubQuestionHolder.dependencies` list).

### Data Model
`DependencyHolder`
- `GameObject gO`
- `SimpleDependency dependency`

`DependenciesManager`
- `dependencies` (List<DependencyHolder>)

### Core Methods
- `CreateDependency(string name)`
  - Ignores null/empty names.
  - Prevents duplicates by checking if a dependency with the same GameObject name exists.
  - Creates a new GameObject named `name`.
  - Parents it under the `DependenciesManager` transform.
  - Adds a `SimpleDependency` component to the new GameObject.
  - Stores both `gO` and `dependency` in `dependencies`.
- `RemoveDependency(string name)`
  - Ignores null/empty names.
  - If not found, returns.
  - Calls `DestroyImmediate` on the dependency GameObject.
  - Removes the holder from `dependencies`.
- `CompleteDependency(string name)`
  - Finds the dependency holder and calls `dependency._Complete()`.

### Where it lives
- Runtime: `Assets/Systems/Game Sequence Manager/DependenciesManager.cs`

---

## How to Set Up in a Scene (Recommended)

1. Add `GameSequenceManager` using `Oman Tools/Game Sequence Manager`.
   - Use **Add GameSequenceManager (and DependenciesManager) to Selected** if you already have a preferred root GameObject.
2. Assign `GameSequenceManager` references in the inspector:
   - `audioManager` (used when `playNarration` is enabled)
   - `hintButton` (used when `playHint` is enabled)
3. Configure `questions`:
   - Add each `QuestionHolder` with its `id`.
   - For each question, add its `subQuestions`.
   - For each `SubQuestionHolder`, set:
     - `beforeDelay`
     - `playNarration`, `narrationId`
     - `playHint`, `QuestionHint`
     - `trialsNo`
     - `dependencies` list (strings)
     - events: `OnQuestionStart`, `OnWrongAttempt`, `OnWrong`, `OnSuccess`, `OnQuestionEnd`
4. If you want dependencies to exist in the scene immediately, press `GenerateDependencies()` (it’s exposed on `SubQuestionHolder`) after the `dependencies` list is filled.
5. Add `ScoreManager` using `Oman Tools/Score Manager`:
   - Either attach to an existing GameObject or create a new `ScoreManager` GameObject.
6. Wire scoring events:
   - Connect your question success/failure events to `ScoreManager.CorrectAnswer()` / `ScoreManager.WrongAttempt()` via the inspector events where appropriate.


