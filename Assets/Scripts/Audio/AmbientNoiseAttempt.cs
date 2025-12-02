using UnityEngine;

public class AmbientNoiseAttempt : MonoBehaviour
{
    [SerializeField] private AudioClip policeSirens;
    [SerializeField] private AudioClip carNoises;
    [SerializeField] private AudioClip rain;

    [SerializeField] private AudioSource policeSource;
    [SerializeField] private AudioSource carSource;
    [SerializeField] private AudioSource rainSource;

    [Range(0f, 1f)][SerializeField] private float volume1 = 1f;
    [Range(0f, 1f)][SerializeField] private float volume2 = 1f;
    [Range(0f, 1f)][SerializeField] private float volume3 = 1f;

    void Awake()
    {
        
        policeSource.clip = policeSirens;
        policeSource.loop = true;
        policeSource.playOnAwake = false;
        policeSource.volume = volume1;
        policeSource.spatialBlend = 0f;
        policeSource.Play();

        
        carSource.clip = carNoises;
        carSource.loop = true;
        carSource.playOnAwake = false;
        carSource.volume = volume2;
        carSource.spatialBlend = 0f;
        carSource.Play();


        rainSource.clip = rain;
        rainSource.loop = true;
        rainSource.playOnAwake = false;
        rainSource.volume = volume3;
        rainSource.spatialBlend = 0f;
        rainSource.Play();
    }
}
