using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageEffect : MonoBehaviour
{
    public static DamageEffect Instance { get; private set; }

    [SerializeField] private Volume volume;
    [SerializeField] private float peakIntensity = 1f;
    [SerializeField] private float holdTime = 0.2f;
    [SerializeField] private float fadeTime = 0.6f;

    private FilmGrain grain;
    private Coroutine routine;

    void Awake()
    {
        Instance = this;
        if (volume == null) volume = GetComponent<Volume>();
        volume.profile.TryGet(out grain);
        if (grain != null) grain.intensity.value = 0f;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play()
    {
        if (grain == null) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(GrainRoutine());
    }

    IEnumerator GrainRoutine()
    {
        grain.intensity.value = peakIntensity;
        yield return new WaitForSeconds(holdTime);

        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeTime)
        {
            grain.intensity.value = Mathf.Lerp(peakIntensity, 0f, t);
            yield return null;
        }
        grain.intensity.value = 0f;
    }
}