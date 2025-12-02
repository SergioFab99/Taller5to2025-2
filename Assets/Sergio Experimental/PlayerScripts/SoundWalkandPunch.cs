using UnityEngine;

public class SoundWalkandPunch : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip walkSound;
    public AudioClip punchSound;

    [Header("Audio Sources")]
    private AudioSource walkAudioSource;
    private AudioSource punchAudioSource;

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (sources.Length >= 2)
        {
            walkAudioSource = sources[0];
            punchAudioSource = sources[1];
        }
        else
        {
            walkAudioSource = gameObject.AddComponent<AudioSource>();
            punchAudioSource = gameObject.AddComponent<AudioSource>();
            
            punchAudioSource.playOnAwake = false;
            punchAudioSource.loop = false;
        }

        walkAudioSource.clip = walkSound;
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        if (isMoving && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
        else if (!isMoving && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }

        if (Input.GetMouseButtonDown(0))
        {
            punchAudioSource.Stop();
            punchAudioSource.clip = punchSound;
            punchAudioSource.Play();
        }
    }
}