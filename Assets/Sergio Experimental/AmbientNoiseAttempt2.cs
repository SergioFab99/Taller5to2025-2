using UnityEngine;

public class AmbientNoiseAttempt2 : MonoBehaviour
{
    [SerializeField] private AudioClip policeSirensInterior;
    [SerializeField] private AudioClip carNoisesInterior;
    [SerializeField] private AudioClip rainInterior;

    [SerializeField] private AudioSource policeSourceInterior;
    [SerializeField] private AudioSource carSourceInterior;
    [SerializeField] private AudioSource rainSourceInterior;

    [Range(0f, 1f)][SerializeField] private float volume1Interior = 1f;
    [Range(0f, 1f)][SerializeField] private float volume2Interior = 1f;
    [Range(0f, 1f)][SerializeField] private float volume3Interior = 1f;

    void Awake()
    {
        policeSourceInterior.clip = policeSirensInterior;
        policeSourceInterior.loop = true;
        policeSourceInterior.playOnAwake = false;
        policeSourceInterior.volume = volume1Interior;
        policeSourceInterior.spatialBlend = 0f;
        policeSourceInterior.Play();

        carSourceInterior.clip = carNoisesInterior;
        carSourceInterior.loop = true;
        carSourceInterior.playOnAwake = false;
        carSourceInterior.volume = volume2Interior;
        carSourceInterior.spatialBlend = 0f;
        carSourceInterior.Play();

        rainSourceInterior.clip = rainInterior;
        rainSourceInterior.loop = true;
        rainSourceInterior.playOnAwake = false;
        rainSourceInterior.volume = volume3Interior;
        rainSourceInterior.spatialBlend = 0f;
        rainSourceInterior.Play();
    }

}
