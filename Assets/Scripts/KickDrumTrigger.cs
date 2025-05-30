using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickDrumTrigger : MonoBehaviour
{
    public AudioClip kickClip;
    public AudioSource audioSource;

    public Transform kickDrum; // Drum mesh to animate
    public float scaleUpAmount = 1.1f;
    public float scaleDuration = 0.1f;

    public XRControls controls; // Replace with your generated input class
    public DrumTutorialManager activeTutorialManager; // Reference set by TutorialSessionManager
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Awake()
    {
        controls = new XRControls();
        controls.Gameplay.KickLeft.performed += _ => TriggerKick();
        controls.Gameplay.KickRight.performed += _ => TriggerKick();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        if (kickDrum == null) kickDrum = transform;
        originalScale = kickDrum.localScale;
    }

    void TriggerKick()
    {
        float randomVolume = Random.Range(0.8f, 1f);
        audioSource.PlayOneShot(kickClip, randomVolume);

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleDrum());

        // === Register hit if in tutorial mode ===
        if (activeTutorialManager != null)
        {
            DrumID id = GetComponent<DrumID>();
            if (id != null)
            {
                string drumName = id.drumName; // Should be "kick"
                float videoTime = (float)activeTutorialManager.GetCurrentVideoTime();

                Debug.Log($"[Kick] Sending hit to TutorialManager: {drumName} at {videoTime:F2}s");
                activeTutorialManager.RegisterHit(drumName, videoTime);
            }
            else
            {
                Debug.LogWarning("DrumID not found on kick drum!");
            }
        }
    }

    IEnumerator ScaleDrum()
    {
        kickDrum.localScale = originalScale;

        Vector3 targetScale = originalScale * scaleUpAmount;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            kickDrum.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / scaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        kickDrum.localScale = targetScale;
        elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            kickDrum.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / scaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        kickDrum.localScale = originalScale;
        scaleCoroutine = null;
    }
}
