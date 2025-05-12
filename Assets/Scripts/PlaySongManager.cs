using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlaySongManager : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("UI Panels")]
    public GameObject playAlongPanel;

    [Header("Controls")]
    public Slider scrubSlider;
    public Button playButton;
    public Button pauseButton;
    public Button quitButton;

    [Header("Videos")]
    public VideoClip[] songVideos;

    private bool isDragging = false;
    private bool isPrepared = false;

    void Start()
    {
        playAlongPanel.SetActive(false);

        playButton.onClick.AddListener(PlayVideo);
        pauseButton.onClick.AddListener(PauseVideo);
        quitButton.onClick.AddListener(QuitSession);

        videoPlayer.loopPointReached += OnVideoEnded;
    }

    public void PlaySongByIndex(int index)
    {
        if (index < 0 || index >= songVideos.Length)
        {
            Debug.LogWarning("Invalid song index");
            return;
        }

        videoPlayer.Stop();
        videoPlayer.clip = songVideos[index];
        videoPlayer.gameObject.SetActive(true);

        isPrepared = false;

        // Unsubscribe previous prepare listener to avoid stacking
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        videoPlayer.Prepare();
        playAlongPanel.SetActive(true);
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        scrubSlider.minValue = 0;
        scrubSlider.maxValue = (float)vp.length;
        scrubSlider.value = 0;

        isPrepared = true;
        vp.Play();
    }

    void Update()
    {
        if (isPrepared && videoPlayer.isPlaying && !isDragging)
        {
            scrubSlider.value = Mathf.Clamp((float)videoPlayer.time, 0, (float)videoPlayer.length);
        }
    }

    public void OnScrubDragStart()
    {
        isDragging = true;
    }

    public void OnScrubDragEnd()
    {
        isDragging = false;
        SeekTo(scrubSlider.value);
    }

    public void OnScrubValueChanged(float value)
    {
        if (isDragging)
        {
            SeekTo(value);
        }
    }

    private void SeekTo(float time)
    {
        if (!isPrepared) return;

        time = Mathf.Clamp(time, 0, (float)videoPlayer.length);
        videoPlayer.time = time;
        Debug.Log($"Seeked to: {time:F2}s");
    }

    public void PlayVideo() => videoPlayer.Play();

    public void PauseVideo() => videoPlayer.Pause();

    public void QuitSession()
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;
        isPrepared = false;

        playAlongPanel.SetActive(false);
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        scrubSlider.value = scrubSlider.maxValue;
    }
}
