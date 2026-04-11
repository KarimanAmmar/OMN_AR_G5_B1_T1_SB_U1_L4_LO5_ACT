using System;
using System.Collections;
using UnityEngine;

public class TransformExtension : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Vector3 _originalScale;
    private Coroutine _activeCoroutine;

    private void Awake()
    {
        SnapshotOriginal();
    }

    public void SnapshotOriginal()
    {
        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;
        _originalScale = transform.localScale;
    }

    private static void MaybeSetActive(GameObject go, bool set_active)
    {
        if (set_active)
            go.SetActive(true);
    }

    // ─── Position ───

    public void SetPosition(Vector3 position, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.position, position, v => transform.position = v, duration);
    }

    public void SetPositionWithDelay(Vector3 position, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.position, position, v => transform.position = v, duration), set_active);
    }

    public void SetLocalPosition(Vector3 position, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localPosition, position, v => transform.localPosition = v, duration);
    }

    public void SetLocalPositionWithDelay(Vector3 position, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localPosition, position, v => transform.localPosition = v, duration), set_active);
    }

    public void MoveToTransform(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.position, target.position, v => transform.position = v, duration);
    }

    public void MoveToTransformWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.position, target.position, v => transform.position = v, duration), set_active);
    }

    public void MoveToTransformLocal(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localPosition, target.localPosition, v => transform.localPosition = v, duration);
    }

    public void MoveToTransformLocalWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localPosition, target.localPosition, v => transform.localPosition = v, duration), set_active);
    }

    public void MoveByOffset(Vector3 offset, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localPosition, transform.localPosition + offset, v => transform.localPosition = v, duration);
    }

    public void MoveByOffsetWithDelay(Vector3 offset, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localPosition, transform.localPosition + offset, v => transform.localPosition = v, duration), set_active);
    }

    // ─── Rotation ───

    public void SetRotation(Vector3 euler, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        RunRot(transform.rotation, Quaternion.Euler(euler), false, duration);
    }

    public void SetRotationWithDelay(Vector3 euler, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => RunRot(transform.rotation, Quaternion.Euler(euler), false, duration), set_active);
    }

    public void SetLocalRotation(Vector3 euler, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        RunRot(transform.localRotation, Quaternion.Euler(euler), true, duration);
    }

    public void SetLocalRotationWithDelay(Vector3 euler, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => RunRot(transform.localRotation, Quaternion.Euler(euler), true, duration), set_active);
    }

    public void MatchRotation(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        RunRot(transform.rotation, target.rotation, false, duration);
    }

    public void MatchRotationWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => RunRot(transform.rotation, target.rotation, false, duration), set_active);
    }

    public void RotateByOffset(Vector3 euler, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        RunRot(transform.localRotation, transform.localRotation * Quaternion.Euler(euler), true, duration);
    }

    public void RotateByOffsetWithDelay(Vector3 euler, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => RunRot(transform.localRotation, transform.localRotation * Quaternion.Euler(euler), true, duration), set_active);
    }

    public void LookAt(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 dir = target.position - transform.position;
        if (dir == Vector3.zero) return;
        RunRot(transform.rotation, Quaternion.LookRotation(dir), false, duration);
    }

    public void LookAtWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => LookAt(target, duration, false), set_active);
    }

    public void LookAt2D(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        RunRot(transform.localRotation, Quaternion.Euler(0f, 0f, angle), true, duration);
    }

    public void LookAt2DWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => LookAt2D(target, duration, false), set_active);
    }

    // ─── Scale ───

    public void SetScale(Vector3 scale, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localScale, scale, v => transform.localScale = v, duration);
    }

    public void SetScaleWithDelay(Vector3 scale, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localScale, scale, v => transform.localScale = v, duration), set_active);
    }

    public void SetUniformScale(float scale, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localScale, Vector3.one * scale, v => transform.localScale = v, duration);
    }

    public void SetUniformScaleWithDelay(float scale, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localScale, Vector3.one * scale, v => transform.localScale = v, duration), set_active);
    }

    public void MatchScale(Transform target, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localScale, target.localScale, v => transform.localScale = v, duration);
    }

    public void MatchScaleWithDelay(Transform target, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localScale, target.localScale, v => transform.localScale = v, duration), set_active);
    }

    public void MultiplyScale(float multiplier, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localScale, transform.localScale * multiplier, v => transform.localScale = v, duration);
    }

    public void MultiplyScaleWithDelay(float multiplier, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localScale, transform.localScale * multiplier, v => transform.localScale = v, duration), set_active);
    }

    public void MultiplyScale(Vector3 multiplier, float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 target = Vector3.Scale(transform.localScale, multiplier);
        Run(transform.localScale, target, v => transform.localScale = v, duration);
    }

    public void MultiplyScaleWithDelay(Vector3 multiplier, float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () =>
        {
            Vector3 target = Vector3.Scale(transform.localScale, multiplier);
            Run(transform.localScale, target, v => transform.localScale = v, duration);
        }, set_active);
    }

    // ─── Parent ───

    public void SetParent(Transform new_parent, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        transform.SetParent(new_parent, true);
    }

    public void SetParentWithDelay(Transform new_parent, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => SetParent(new_parent, false), set_active);
    }

    // ─── Flip ───

    public void FlipX(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 s = transform.localScale;
        s.x = -s.x;
        Run(transform.localScale, s, v => transform.localScale = v, duration);
    }

    public void FlipXWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () =>
        {
            Vector3 s = transform.localScale;
            s.x = -s.x;
            Run(transform.localScale, s, v => transform.localScale = v, duration);
        }, set_active);
    }

    public void FlipY(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 s = transform.localScale;
        s.y = -s.y;
        Run(transform.localScale, s, v => transform.localScale = v, duration);
    }

    public void FlipYWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () =>
        {
            Vector3 s = transform.localScale;
            s.y = -s.y;
            Run(transform.localScale, s, v => transform.localScale = v, duration);
        }, set_active);
    }

    public void FlipZ(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Vector3 s = transform.localScale;
        s.z = -s.z;
        Run(transform.localScale, s, v => transform.localScale = v, duration);
    }

    public void FlipZWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () =>
        {
            Vector3 s = transform.localScale;
            s.z = -s.z;
            Run(transform.localScale, s, v => transform.localScale = v, duration);
        }, set_active);
    }

    // ─── Punch & Shake ───

    public void PunchScale(float intensity = 0.2f, float duration = 0.3f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Stop();
        _activeCoroutine = StartCoroutine(PunchScaleRoutine(intensity, duration));
    }

    public void PunchScaleWithDelay(float intensity = 0.2f, float duration = 0.3f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => PunchScale(intensity, duration, false), set_active);
    }

    public void Shake(float intensity = 0.1f, float duration = 0.3f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Stop();
        _activeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    public void ShakeWithDelay(float intensity = 0.1f, float duration = 0.3f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Shake(intensity, duration, false), set_active);
    }

    // ─── Reset ───

    public void ResetPosition(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localPosition, _originalPosition, v => transform.localPosition = v, duration);
    }

    public void ResetPositionWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localPosition, _originalPosition, v => transform.localPosition = v, duration), set_active);
    }

    public void ResetRotation(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        RunRot(transform.localRotation, _originalRotation, true, duration);
    }

    public void ResetRotationWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => RunRot(transform.localRotation, _originalRotation, true, duration), set_active);
    }

    public void ResetScale(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Run(transform.localScale, _originalScale, v => transform.localScale = v, duration);
    }

    public void ResetScaleWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => Run(transform.localScale, _originalScale, v => transform.localScale = v, duration), set_active);
    }

    public void ResetAll(float duration = 0f, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Stop();
        if (duration <= 0f)
        {
            transform.localPosition = _originalPosition;
            transform.localRotation = _originalRotation;
            transform.localScale = _originalScale;
        }
        else
        {
            _activeCoroutine = StartCoroutine(ResetAllRoutine(duration));
        }
    }

    public void ResetAllWithDelay(float duration = 0f, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => ResetAll(duration, false), set_active);
    }

    // ─── Visibility ───

    public void SetActive(bool active, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        gameObject.SetActive(active);
    }

    public void SetActiveWithDelay(bool active, float delay = 0f, bool set_active = false)
    {
        RunDelayed(delay, () => SetActive(active, false), set_active);
    }

    // ─── Stop ───

    public void Stop()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
    }

    // ─── Internal ───

    private void RunDelayed(float delay, Action action, bool set_active = false)
    {
        MaybeSetActive(gameObject, set_active);
        Stop();
        if (delay <= 0f)
            action();
        else
            _activeCoroutine = StartCoroutine(DelayThenRun(delay, action));
    }

    private IEnumerator DelayThenRun(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        _activeCoroutine = null;
        action();
    }

    private void Run(Vector3 from, Vector3 to, Action<Vector3> setter, float duration)
    {
        Stop();
        if (duration <= 0f)
            setter(to);
        else
            _activeCoroutine = StartCoroutine(LerpVector3(from, to, setter, duration));
    }

    private void RunRot(Quaternion from, Quaternion to, bool local, float duration)
    {
        Stop();
        if (duration <= 0f)
        {
            if (local) transform.localRotation = to;
            else transform.rotation = to;
        }
        else
        {
            _activeCoroutine = StartCoroutine(LerpQuat(from, to, local, duration));
        }
    }

    private IEnumerator LerpVector3(Vector3 from, Vector3 to, System.Action<Vector3> setter, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            setter(Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration)));
            yield return null;
        }
        setter(to);
        _activeCoroutine = null;
    }

    private IEnumerator LerpQuat(Quaternion from, Quaternion to, bool local, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Quaternion q = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            if (local) transform.localRotation = q;
            else transform.rotation = q;
            yield return null;
        }
        if (local) transform.localRotation = to;
        else transform.rotation = to;
        _activeCoroutine = null;
    }

    private IEnumerator PunchScaleRoutine(float intensity, float duration)
    {
        Vector3 original = transform.localScale;
        Vector3 punched = original + Vector3.one * intensity;
        float half = Mathf.Max(duration * 0.5f, 0.05f);

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(original, punched, Mathf.SmoothStep(0f, 1f, elapsed / half));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(punched, original, Mathf.SmoothStep(0f, 1f, elapsed / half));
            yield return null;
        }

        transform.localScale = original;
        _activeCoroutine = null;
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        Vector3 original = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float decay = 1f - Mathf.Clamp01(elapsed / duration);
            transform.localPosition = original + UnityEngine.Random.insideUnitSphere * (intensity * decay);
            yield return null;
        }

        transform.localPosition = original;
        _activeCoroutine = null;
    }

    private IEnumerator ResetAllRoutine(float duration)
    {
        Vector3 fromPos = transform.localPosition;
        Quaternion fromRot = transform.localRotation;
        Vector3 fromScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.localPosition = Vector3.Lerp(fromPos, _originalPosition, t);
            transform.localRotation = Quaternion.Slerp(fromRot, _originalRotation, t);
            transform.localScale = Vector3.Lerp(fromScale, _originalScale, t);
            yield return null;
        }

        transform.localPosition = _originalPosition;
        transform.localRotation = _originalRotation;
        transform.localScale = _originalScale;
        _activeCoroutine = null;
    }
}
