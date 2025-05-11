using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DrumTutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class DrumHint
    {
        public float time;
        public string drumName;
        public DrumRingHighlighter ringToFlash;
        public bool wasHit;
    }

    [System.Serializable]
    public class DrumHintData
    {
        public float time;
        public string drum;
    }

    [System.Serializable]
    public class DrumRingMapping
    {
        public string drumName;
        public DrumRingHighlighter ring;
    }

    public TextAsset hintJsonFile;
    public List<DrumRingMapping> ringMappings = new List<DrumRingMapping>();
    private Dictionary<string, DrumRingHighlighter> drumNameToRing = new Dictionary<string, DrumRingHighlighter>();
    public DrumHitFeedbackManager feedbackManager; 

    public List<DrumHint> tutorialHints = new List<DrumHint>();
    public float acceptableHitWindow = 0.3f;

    private Coroutine hintRoutine;
    private int totalHints, successfulHits;

    public int TotalHints => tutorialHints.Count;
    public int SuccessfulHits => successfulHits;

    private bool isPaused = false;

    void Start()
    {
        foreach (var mapping in ringMappings)
        {
            if (!drumNameToRing.ContainsKey(mapping.drumName))
                drumNameToRing.Add(mapping.drumName, mapping.ring);
        }

        if (hintJsonFile != null)
        {
            var dataList = JsonUtilityWrapper.FromJsonList(hintJsonFile.text);
            tutorialHints.Clear();

            foreach (var data in dataList)
            {
                if (drumNameToRing.TryGetValue(data.drum, out var ring))
                {
                    tutorialHints.Add(new DrumHint
                    {
                        time = data.time,
                        drumName = data.drum,
                        ringToFlash = ring
                    });
                }
                else
                {
                    Debug.LogWarning($"No ring assigned for drum: {data.drum}");
                }
            }
        }
    }

    private VideoPlayer activeVideoPlayer;
    public void StartTutorial(VideoPlayer player)
    {
        activeVideoPlayer = player;
        totalHints = tutorialHints.Count;
        successfulHits = 0;
        isPaused = false;

        foreach (var hint in tutorialHints)
        {
            hint.wasHit = false;
        }

        if (hintRoutine != null)
            StopCoroutine(hintRoutine);

        hintRoutine = StartCoroutine(RunHints(player));
    }

    public void PauseTutorial()
    {
        isPaused = true;
        if (hintRoutine != null)
            StopCoroutine(hintRoutine);
    }

    public void ResumeTutorial(VideoPlayer player)
    {
        isPaused = false;
        if (hintRoutine != null)
            StopCoroutine(hintRoutine);

        hintRoutine = StartCoroutine(RunHints(player));
    }

    public void QuitTutorial()
    {
        isPaused = false;
        if (hintRoutine != null)
        {
            StopCoroutine(hintRoutine);
            hintRoutine = null;
        }
    }

    IEnumerator RunHints(VideoPlayer player)
    {
        foreach (var hint in tutorialHints)
        {
            while (player.time < hint.time)
                yield return null;

            hint.ringToFlash.Flash();
        }
    }

    public void RegisterHit(string drumName, float hitTime)
    {
        foreach (var hint in tutorialHints)
        {
            float delta = Mathf.Abs(hitTime - hint.time);

            if (hint.drumName == drumName && !hint.wasHit && delta <= acceptableHitWindow)
            {
                hint.wasHit = true;
                successfulHits++;

                if (feedbackManager != null)
                {
                    feedbackManager.ShowFeedback("Nice!");
                }
                Debug.Log($"Registered HIT: {drumName} at {hitTime:F2}s (expected: {hint.time:F2}s, : {delta:F2}s)");
                return;
            }
        }

        Debug.Log($"Missed or too late: {drumName} at {hitTime:F2}s (window ±{acceptableHitWindow}s)");
    }

    public double GetCurrentVideoTime()
    {
        return activeVideoPlayer != null ? activeVideoPlayer.time : 0;
    }

    public float GetAccuracyPercent()
    {
        return TotalHints > 0 ? (SuccessfulHits / (float)TotalHints) * 100f : 0f;
    }

    public static class JsonUtilityWrapper
    {
        [System.Serializable]
        private class DrumHintDataList
        {
            public List<DrumHintData> items;
        }

        public static List<DrumHintData> FromJsonList(string json)
        {
            json = "{\"items\":" + json + "}";
            return JsonUtility.FromJson<DrumHintDataList>(json).items;
        }
    }
}
