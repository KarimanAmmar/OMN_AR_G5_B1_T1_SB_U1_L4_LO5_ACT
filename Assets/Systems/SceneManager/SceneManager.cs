using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    [Header("Scene Lifecycle Events")]
    public UnityEvent2 OnSceneStarted;
    public UnityEvent2 OnSceneUnloaded;

    [Header("Scene Loading Events")]
    public UnityEvent2 OnSceneLoadBegin;
    public UnityEvent2 OnSceneLoadComplete;

    [Header("Application Events")]
    public UnityEvent2 OnAppPaused;
    public UnityEvent2 OnAppResumed;
    public UnityEvent2 OnAppQuitting;

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += HandleSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += HandleSceneUnloaded;
        Application.quitting += HandleAppQuitting;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= HandleSceneUnloaded;
        Application.quitting -= HandleAppQuitting;
    }

    private void Start()
    {
        OnSceneStarted?.Invoke();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            OnAppPaused?.Invoke();
        else
            OnAppResumed?.Invoke();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoadComplete?.Invoke();
    }

    private void HandleSceneUnloaded(Scene scene)
    {
        OnSceneUnloaded?.Invoke();
    }

    private void HandleAppQuitting()
    {
        OnAppQuitting?.Invoke();
    }

    // --- Scene Loading Functions ---

    public void LoadScene(string sceneName)
    {
        OnSceneLoadBegin?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int buildIndex)
    {
        OnSceneLoadBegin?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void LoadSceneAsync(string sceneName)
    {
        OnSceneLoadBegin?.Invoke();
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    public void ReloadCurrentScene()
    {
        OnSceneLoadBegin?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        int next = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        if (next < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            OnSceneLoadBegin?.Invoke();
            UnityEngine.SceneManagement.SceneManager.LoadScene(next);
        }
    }

    public void LoadPreviousScene()
    {
        int prev = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
        if (prev >= 0)
        {
            OnSceneLoadBegin?.Invoke();
            UnityEngine.SceneManagement.SceneManager.LoadScene(prev);
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Fade + Load Functions ---

    public void LoadSceneWithFade(string sceneName)
    {
        SceneFader.Instance.FadeIn(() => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;
    }
}
