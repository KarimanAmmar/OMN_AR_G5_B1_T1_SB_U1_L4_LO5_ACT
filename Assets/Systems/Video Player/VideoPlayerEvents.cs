using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;


[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerEvents : MonoBehaviour
{
    [Header ("On Video Started")]
    public UnityEvent2 OnVideoStarted;


    [Header("On Video Finished")]
    public UnityEvent2 OnVideoFinished;

    [Header("On Video Frame Reached")]
    public long TargetFrame;
    public UnityEvent2 OnFrameReached;
    private bool _triggered;


    private VideoPlayer _videoPlayer;


    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

        _videoPlayer.started += HandleVideoStarted;

        _videoPlayer.loopPointReached += HandleVideoFinished;

        _videoPlayer.sendFrameReadyEvents = true;
        _videoPlayer.frameReady += HandleFrameReady;
    }


    private void OnDestroy()
    {
        if (_videoPlayer != null)
        {
            _videoPlayer.started -= HandleVideoStarted;
            _videoPlayer.loopPointReached -= HandleVideoFinished;
            _videoPlayer.frameReady -= HandleFrameReady;
        }
    }


    private void HandleVideoStarted(VideoPlayer source)
    {
        OnVideoStarted?.Invoke();
    }


    private void HandleFrameReady(VideoPlayer source, long frameIdx)
    {
        if (!_triggered && frameIdx >= TargetFrame)
        {
            _triggered = true;
            OnFrameReached?.Invoke();
        }
    }


    private void HandleVideoFinished(VideoPlayer source)
    {
        OnVideoFinished?.Invoke();
    }
}