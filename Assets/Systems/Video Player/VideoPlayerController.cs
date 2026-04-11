using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using System.IO;

public class VideoPlayerController : MonoBehaviour
{
    [Header("Video Name + Extension")]
    [SerializeField] private string videoName;
    [SerializeField] private bool playOnStart;


    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RectTransform controls;


    [Header("Video Controls")]
    [SerializeField] private Slider ProgressSlider;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonPause;
    [SerializeField] private TextMeshProUGUI TimeProgressText;

    [Space(10)]

    [SerializeField] private Vector2 controlsOnHoverPosition;
    [SerializeField] private Vector2 controlsNotOnHoverPosition;

    [Space(10)]

    [SerializeField] private bool ShowHoursWhenNeeded = true;

    private bool _userSeeking;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (ProgressSlider != null)
        {
            ProgressSlider.minValue = 0f;
            ProgressSlider.maxValue = 1f;
            ProgressSlider.wholeNumbers = false;
            ProgressSlider.onValueChanged.AddListener(OnSliderValueChanged);
            RegisterSliderPointerEvents();
        }

        if (buttonPlay != null)
            buttonPlay.onClick.AddListener(Play);
        if (buttonPause != null)
            buttonPause.onClick.AddListener(Pause);

        if (videoPlayer != null)
            videoPlayer.prepareCompleted += OnVideoPrepared;

        ShowHideControls(false);

        SetVideoUrl();
    }


    private void Update()
    {
        if (videoPlayer == null)
            return;

        RefreshTimeText();

        if (ProgressSlider == null)
            return;

        if (_userSeeking || !videoPlayer.isPrepared)
            return;

        double len = GetDuration();
        if (len <= 0.0001)
            return;

        float normalized = Mathf.Clamp01((float)(videoPlayer.time / len));
        ProgressSlider.SetValueWithoutNotify(normalized);
    }


    private void SetVideoUrl()
    {
        videoPlayer.playOnAwake = false;

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, videoName);

        if (playOnStart)
            Play();
    }


    private void Play()
    {
        if (videoPlayer == null) return;
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            if (buttonPause != null) buttonPause.gameObject.SetActive(true);
            if (buttonPlay != null) buttonPlay.gameObject.SetActive(false);
        }
    }


    private void Pause()
    {
        if (videoPlayer == null) return;
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            if (buttonPause != null) buttonPause.gameObject.SetActive(false);
            if (buttonPlay != null) buttonPlay.gameObject.SetActive(true);
        }
    }


    public void ShowHideControls(bool isVisible)
    {
        if (isVisible)
            controls.DOAnchorPos(controlsOnHoverPosition, 0.25f, true);
        else
            controls.DOAnchorPos(controlsNotOnHoverPosition, 0.25f, true);
    }


    #region EDITOR


    [Button("Show Controls", ButtonHeight = 50)]
    private void ShowControls_Edtior()
    {
        controls.localPosition = controlsOnHoverPosition;
    }


    [Button("Hide Controls", ButtonHeight = 50)]
    private void HideControls_Edtior()
    {
        controls.localPosition = controlsNotOnHoverPosition;
    }


    #endregion


    private void OnSliderValueChanged(float normalized)
    {
        if (videoPlayer == null || !videoPlayer.isPrepared)
            return;

        double len = GetDuration();
        if (len <= 0.0001)
            return;

        videoPlayer.time = normalized * len;
    }


    private void OnVideoPrepared(VideoPlayer source)
    {
        if (ProgressSlider != null)
            ProgressSlider.SetValueWithoutNotify(0f);
        RefreshTimeText();
    }

    private void RefreshTimeText()
    {
        if (TimeProgressText == null || videoPlayer == null)
            return;

        if (!videoPlayer.isPrepared)
        {
            TimeProgressText.text = "--:-- / --:--";
            return;
        }

        double len = GetDuration();
        if (len <= 0.0001)
        {
            TimeProgressText.text = "--:-- / --:--";
            return;
        }

        double current = _userSeeking && ProgressSlider != null
            ? ProgressSlider.value * len
            : videoPlayer.time;

        current = Math.Max(0, current);
        TimeProgressText.text = $"{FormatTime(current)} / {FormatTime(len)}";
    }

    private string FormatTime(double seconds)
    {
        if (seconds < 0) seconds = 0;
        int total = (int)Math.Floor(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;

        if (ShowHoursWhenNeeded && h > 0)
            return $"{h}:{m:00}:{s:00}";

        return $"{m}:{s:00}";
    }


    private double GetDuration()
    {
        if (videoPlayer == null)
            return 0;

        if (videoPlayer.length > 0.0001)
            return videoPlayer.length;

        if (videoPlayer.clip != null && videoPlayer.clip.length > 0.0001)
            return videoPlayer.clip.length;

        return 0;
    }

    private void RegisterSliderPointerEvents()
    {
        var sliderGo = ProgressSlider.gameObject;
        var trigger = sliderGo.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = sliderGo.AddComponent<EventTrigger>();

        var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => UserSeekStart());
        trigger.triggers.Add(down);

        var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => UserSeekEnd());
        trigger.triggers.Add(up);
    }

    private void UserSeekStart()
    {
        _userSeeking = true;
    }

    private void UserSeekEnd()
    {
        _userSeeking = false;
        if (videoPlayer != null && videoPlayer.isPrepared && ProgressSlider != null)
        {
            double len = GetDuration();
            if (len > 0.0001)
                videoPlayer.time = ProgressSlider.value * len;
        }
    }


    private void OnDestroy()
    {
        if (ProgressSlider != null)
            ProgressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);

        if (videoPlayer != null)
            videoPlayer.prepareCompleted -= OnVideoPrepared;
    }
}
