using UnityEngine;

[
    RequireComponent(typeof(AudioSource))
]
public class GestorAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource movementLoopSource;

    [Header("Character Clips")]
    [SerializeField] private AudioClip jumpingClip;
    [SerializeField] private AudioClip hittingClip;
    [SerializeField] private AudioClip protectingClip;
    [SerializeField] private AudioClip runningClip;
    [SerializeField] private AudioClip walkingClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private enum MovementState
    {
        None,
        Walking,
        Running
    }

    private static readonly KeyCode[] RunKeys =
    {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
    };

    private MovementState currentMovementState = MovementState.None;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (movementLoopSource == null)
        {
            movementLoopSource = gameObject.AddComponent<AudioSource>();
        }

        movementLoopSource.playOnAwake = false;
        movementLoopSource.loop = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryPlayClip(jumpingClip, nameof(jumpingClip));
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlayClip(hittingClip, nameof(hittingClip));
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryPlayClip(protectingClip, nameof(protectingClip));
        }

        UpdateMovementAudio();
    }

    private void UpdateMovementAudio()
    {
        MovementState desiredState = EvaluateMovementState();

        if (desiredState == currentMovementState)
        {
            return;
        }

        bool transitionSucceeded = true;

        switch (desiredState)
        {
            case MovementState.None:
                StopMovementLoop();
                break;
            case MovementState.Walking:
                transitionSucceeded = PlayMovementLoop(walkingClip, nameof(walkingClip));
                break;
            case MovementState.Running:
                transitionSucceeded = PlayMovementLoop(runningClip, nameof(runningClip));
                break;
        }

        if (transitionSucceeded)
        {
            currentMovementState = desiredState;
        }
    }

    private MovementState EvaluateMovementState()
    {
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        foreach (KeyCode key in RunKeys)
        {
            if (Input.GetKey(key))
            {
                return shiftHeld ? MovementState.Running : MovementState.Walking;
            }
        }

        return MovementState.None;
    }

    private bool PlayMovementLoop(AudioClip clip, string clipName)
    {
        if (movementLoopSource == null)
        {
            Debug.LogError("GestorAudio: No se encontró AudioSource de movimiento.");
            StopMovementLoop();
            return false;
        }

        if (clip == null)
        {
            Debug.LogWarning($"GestorAudio: Falta asignar el clip '{clipName}'.");
            StopMovementLoop();
            return false;
        }

        if (movementLoopSource.clip == clip && movementLoopSource.isPlaying)
        {
            return true;
        }

        movementLoopSource.Stop();
        movementLoopSource.clip = clip;
        movementLoopSource.volume = volume;
        movementLoopSource.Play();
        Debug.Log($"GestorAudio: Reproduciendo loop '{clipName}'.");
        return true;
    }

    private void StopMovementLoop()
    {
        if (movementLoopSource != null && movementLoopSource.isPlaying)
        {
            movementLoopSource.Stop();
            movementLoopSource.clip = null;
            Debug.Log("GestorAudio: Loop de movimiento detenido.");
        }
    }

    private void TryPlayClip(AudioClip clip, string clipName)
    {
        if (clip == null || audioSource == null)
        {
            if (clip == null)
            {
                Debug.LogWarning($"GestorAudio: Falta asignar el clip '{clipName}'.");
            }

            if (audioSource == null)
            {
                Debug.LogError("GestorAudio: No se encontró AudioSource para reproducir el audio.");
            }

            return;
        }

        audioSource.PlayOneShot(clip, volume);
        Debug.Log($"GestorAudio: Reproduciendo clip '{clipName}'.");
    }
}
