using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Video;

public class TutorialSessionManager : MonoBehaviour
{
    public VideoPlayer[] tutorialVideos;
    public DrumTutorialManager[] tutorialHintManagers;
    public GameObject VideoPanel;
    public GameObject VideoController;
    public GameObject endTutorialUI;
    public GameObject endFinalTutorialUI;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private int currentTutorialIndex = -1;

    void Start()
    {
        foreach (var vp in tutorialVideos)
            vp.loopPointReached += OnVideoComplete;

        endTutorialUI.SetActive(false);
        endFinalTutorialUI.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void StartTutorial(int index)
    {
        if (index < 0 || index >= tutorialVideos.Length)
        {
            Debug.LogWarning("Invalid tutorial index");
            return;
        }

        // Stop and hide all videos
        foreach (var vp in tutorialVideos)
        {
            vp.Stop();
            vp.time = 0;
            vp.gameObject.SetActive(false);
        }

        // Reset UI
        VideoPanel.SetActive(true);
        VideoController.SetActive(true);
        endTutorialUI.SetActive(false);
        endFinalTutorialUI.SetActive(false);
        resultPanel.SetActive(false);

        // Start new tutorial
        currentTutorialIndex = index;
        tutorialVideos[index].gameObject.SetActive(true);
        tutorialVideos[index].Play();
        tutorialHintManagers[index].StartTutorial(tutorialVideos[index]);
        AssignActiveTutorialManagerToAllDrums(tutorialHintManagers[index]);
    }

    public void PauseTutorial()
    {
        if (currentTutorialIndex < 0) return;
        tutorialVideos[currentTutorialIndex].Pause();
        tutorialHintManagers[currentTutorialIndex].PauseTutorial();
    }

    public void ResumeTutorial()
    {
        if (currentTutorialIndex < 0) return;
        tutorialVideos[currentTutorialIndex].Play();
        tutorialHintManagers[currentTutorialIndex].ResumeTutorial(tutorialVideos[currentTutorialIndex]);
    }

    public void QuitTutorial()
    {
        if (currentTutorialIndex < 0) return;
        tutorialVideos[currentTutorialIndex].Stop();
        tutorialHintManagers[currentTutorialIndex].QuitTutorial();
        AssignActiveTutorialManagerToAllDrums(null);

        VideoPanel.SetActive(false);
        VideoController.SetActive(false);
        endTutorialUI.SetActive(false);
        endFinalTutorialUI.SetActive(false);
        resultPanel.SetActive(false);
    }

    private void OnVideoComplete(VideoPlayer finishedVideo)
    {
        if (tutorialVideos[currentTutorialIndex] != finishedVideo) return;

        if (currentTutorialIndex == 0)
        {
            VideoController.SetActive(false);
            endTutorialUI.SetActive(true);
            return;
        }

        var currentTutorial = tutorialHintManagers[currentTutorialIndex];
        int hits = currentTutorial.SuccessfulHits;
        int total = currentTutorial.TotalHints;
        float accuracy = currentTutorial.GetAccuracyPercent();

        string feedback;
        if (accuracy >= 90f) feedback = "Excellent!";
        else if (accuracy >= 75f) feedback = "Great!";
        else if (accuracy >= 50f) feedback = "Good job!";
        else feedback = "Keep practicing!";

        resultText.text = $"{hits}/{total}  {accuracy:F0}%\n{feedback}";
        resultPanel.SetActive(true);

        endTutorialUI.SetActive(false);
        endFinalTutorialUI.SetActive(false);
        VideoController.SetActive(false);

        if (currentTutorialIndex < tutorialVideos.Length - 1)
        {
            endTutorialUI.SetActive(true);
        }
        else
        {
            endFinalTutorialUI.SetActive(true);
        }
    }

    public void AssignActiveTutorialManagerToAllDrums(DrumTutorialManager activeManager)
    {
        PlayAudioOnTriggerEnter[] allDrums = FindObjectsOfType<PlayAudioOnTriggerEnter>();
        foreach (var drum in allDrums)
        {
            drum.activeTutorialManager = activeManager;
        }
    }

    // Button helper methods
    public void ReplayCurrentTutorial() => StartTutorial(currentTutorialIndex);
    public void PlayNextTutorial() => StartTutorial(currentTutorialIndex + 1);
    public void StartTutorialOne() => StartTutorial(0);
    public void StartTutorialTwo() => StartTutorial(1);
    public void StartTutorialThree() => StartTutorial(2);
    public void StartTutorialFour() => StartTutorial(3);
}
