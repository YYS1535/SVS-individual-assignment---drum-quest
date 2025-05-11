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
    public VideoClip[] songVideos; // Assign 6 video clips in Inspector

    private bool isDragging = false;

    void Start()
    {
        // Initially show only song selection
        playAlongPanel.SetActive(false);

        // Hook up buttons
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

        // Activate the video player GameObject first!
        videoPlayer.gameObject.SetActive(true);

        // Set the selected clip
        videoPlayer.clip = songVideos[index];

        // Prepare and play
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) =>
        {
            scrubSlider.minValue = 0;
            scrubSlider.maxValue = (float)vp.length;
            vp.Play();
        };

        // UI updates
        playAlongPanel.SetActive(true);
    }

    void Update()
    {
        if (videoPlayer.isPrepared && !isDragging)
        {
            scrubSlider.value = (float)videoPlayer.time;
        }
    }

    public void OnScrubDragStart()
    {
        isDragging = true;
    }

    public void OnScrubDragEnd()
    {
        isDragging = false;
        videoPlayer.time = scrubSlider.value;
    }

    public void OnScrubValueChanged(float value)
    {
        if (isDragging)
        {
            videoPlayer.time = value;
        }
    }

    public void PlayVideo() => videoPlayer.Play();

    public void PauseVideo() => videoPlayer.Pause();

    public void QuitSession()
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;

        playAlongPanel.SetActive(false);
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        // Optionally reset slider
        scrubSlider.value = 0;
    }
}
