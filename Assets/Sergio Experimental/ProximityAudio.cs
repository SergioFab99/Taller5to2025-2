using UnityEngine;

public class ProximityAudio : MonoBehaviour
{
    [Header("Settings")]
    public AudioClip clip;
    public float detectionRadius = 5f;
    public float fadeSpeed = 2f;

    [Header("References")]
    public Transform player;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.spatialBlend = 0f;
        
        if (clip != null)
        {
            audioSource.clip = clip;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("ProximityAudio: Player not assigned and no object with tag 'Player' found.");
            }
        }
    }

    private void Update()
    {
        if (player == null || audioSource.clip == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            
            audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, Time.deltaTime * fadeSpeed);
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * fadeSpeed);

                if (audioSource.volume < 0.01f)
                {
                    audioSource.Stop();
                    audioSource.volume = 0f;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius * 2.5f);
    }
}
