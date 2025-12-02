using UnityEngine;

public class ClickAndEventsManager : MonoBehaviour
{
    public AudioClip leftClickSound;
    public AudioClip rightClickSound;
    public AudioClip eSound;
    public AudioClip spaceSound;
    public AudioClip shiftSound;

    private AudioSource audioSource;
    private bool shiftIsPlaying = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaySound(leftClickSound);
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlaySound(rightClickSound);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaySound(eSound);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound(spaceSound);
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !shiftIsPlaying)
        {
            audioSource.clip = shiftSound;
            audioSource.loop = true;
            audioSource.Play();
            shiftIsPlaying = true;
        }

        if (shiftIsPlaying && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            audioSource.Stop();
            audioSource.loop = false;
            shiftIsPlaying = false;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && !shiftIsPlaying)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
