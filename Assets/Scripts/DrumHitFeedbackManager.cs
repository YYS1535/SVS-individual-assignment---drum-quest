using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrumHitFeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public CanvasGroup canvasGroup;

    public float fadeInTime = 0.1f;
    public float displayTime = 0.4f;
    public float fadeOutTime = 0.3f;

    private Coroutine feedbackRoutine;

    public void ShowFeedback(string message)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ShowFeedbackRoutine(message));
    }

    IEnumerator ShowFeedbackRoutine(string message)
    {
        feedbackText.text = message;

        // Fade In
        canvasGroup.alpha = 0f;
        float t = 0f;
        while (t < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        t = 0f;
        while (t < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
