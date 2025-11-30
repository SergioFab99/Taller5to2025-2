using UnityEngine;

public class SoundWalkandPunch : MonoBehaviour
{
    public AudioClip walkSound;
    public AudioClip punchSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
        }

        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(punchSound);
        }
    }
}
