using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayAudioOnTriggerEnter : MonoBehaviour
{
    public AudioClip clip;
    private AudioSource source;
    private Vector3 originalScale; // Store once
    private Coroutine scaleRoutine;
    public DrumTutorialManager activeTutorialManager; // Set from manager

    [Header("Tag of drumstick")]
    public string targetTag = "Drumstick";

    [Header("Velocity Settings")]
    public bool useVelocity = true;
    public float minVelocity = 0.1f;
    public float maxVelocity = 4f;

    [Header("Animation Settings")]
    public Transform objectToAnimate; // assign parent drum object here
    public float scaleMultiplier = 1.1f;
    public float scaleDuration = 0.1f;

    [Header("Particle Effect")]
    public ParticleSystem hitParticles;

    void Start()
    {
        source = GetComponent<AudioSource>();

        if (objectToAnimate == null)
        {
            Debug.LogWarning("objectToAnimate not assigned on " + gameObject.name);
        }
        else
        {
            originalScale = objectToAnimate.localScale; // store original scale on start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (useVelocity)
        {
            DrumstickVelocityTracker tracker = other.GetComponent<DrumstickVelocityTracker>();
            if (tracker && useVelocity)
            {
                float v = tracker.Velocity.magnitude;
                Debug.Log("Velocity Magnitude: " + v);

                float volume = Mathf.InverseLerp(minVelocity, maxVelocity, v);
                source.PlayOneShot(clip, volume);
            }
            else //fallback
            {
                Debug.LogWarning("VelocityEstimator not found on: " + other.name);
                source.PlayOneShot(clip);
            }
        }
        AnimateDrum();

        if (hitParticles != null)
        {
            hitParticles.Play();
        }

        if (activeTutorialManager != null)
        {
            DrumID id = GetComponent<DrumID>();
            if (id != null)
            {
                string drumName = id.drumName;
                float videoTime = (float)activeTutorialManager.GetCurrentVideoTime();

                Debug.Log($"[Hit] Sending hit to TutorialManager: {drumName} at {videoTime:F2}s");
                activeTutorialManager.RegisterHit(drumName, videoTime);
            }
            else
            {
                Debug.LogWarning($"DrumID not found on: {gameObject.name}");
            }
        }
    }

    private void AnimateDrum()
    {
        if (objectToAnimate == null) return;

        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScalePulse(objectToAnimate));
    }

    IEnumerator ScalePulse(Transform target)
    {
        Vector3 targetScale = originalScale * scaleMultiplier;

        float t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t / scaleDuration);
            yield return null;
        }

        t = 0f;
        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t / scaleDuration);
            yield return null;
        }

        target.localScale = originalScale;
    }
}
