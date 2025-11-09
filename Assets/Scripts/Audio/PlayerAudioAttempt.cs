using UnityEngine;

public class PlayerAudioAttempt : MonoBehaviour
{
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float loopVolume = 1f;

    [Header("walking")]
    [SerializeField] private AudioClip walkNormal;
    [SerializeField] private AudioClip walkDirt;
    [SerializeField] private AudioClip runNormal;
    [SerializeField] private AudioClip runDirt;
    [SerializeField] private AudioClip drunkWalkNormal;
    [SerializeField] private AudioClip drunkWalkDirt;
    [Header("low hp")]
    [SerializeField] private AudioClip lowHPBreathing;
    [Header("single inputs")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip damage1;
    [SerializeField] private AudioClip damage2;
    [SerializeField] private AudioClip damage3;
    [SerializeField] private AudioClip damageBlocked;
    [Header("audio sources")]
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private AudioSource oneShotSource;

    //ok el plan es meterle 2 audiosource, uno para los que loopean y otro para los q funcan una vez
    //puro metodo publico para llamarlo desde donde va el player
    //cualquier cosa es culpa de sergio
    //igual esto es solo para la build de ahora, mas adelante puedo hacer el modular que le dije a XxAkiGaming2003xX

    private enum MoveState { None, Walk, Run, DrunkWalk }
    private MoveState currentState = MoveState.None;

    private bool isOnDirt = false;
    private bool lowHPActive = false;

    private void Awake()
    {
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        oneShotSource.loop = false;
        oneShotSource.playOnAwake = false;
    }

    private void Update()
    {
        HandleMovementAudio();
    }

    //helpers
    public void SetSurface(bool onDirt)
    {
        if (isOnDirt != onDirt)
        {
            isOnDirt = onDirt;
            UpdateMovementLoop();
        }
    }

    public void SetLowHP(bool active)
    {
        if (lowHPActive == active) return;
        lowHPActive = active;

        if (active)
            StartBreathingLoop();
        else if (loopSource.clip == lowHPBreathing)
            StopLoop();
    }

    private void HandleMovementAudio()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool running = Input.GetKey(KeyCode.LeftShift); //no se cual es la tecla exacta
        bool drunk = false; // placeholder

        MoveState desired = MoveState.None;

        if (moving)
        {
            if (drunk) desired = MoveState.DrunkWalk;
            else desired = running ? MoveState.Run : MoveState.Walk;
        }

        if (desired != currentState)
        {
            currentState = desired;
            UpdateMovementLoop();
        }
    }

    private void UpdateMovementLoop()
    {
        loopSource.Stop();

        AudioClip next = null;

        switch (currentState)
        {
            case MoveState.Walk:
                next = isOnDirt ? walkDirt : walkNormal;
                break;
            case MoveState.Run:
                next = isOnDirt ? runDirt : runNormal;
                break;
            case MoveState.DrunkWalk:
                next = isOnDirt ? drunkWalkDirt : drunkWalkNormal;
                break;
            default:
                next = null;
                break;
        }

        if (next != null)
        {
            loopSource.clip = next;
            loopSource.volume = loopVolume;
            loopSource.Play();
        }
    }

    private void StartBreathingLoop()
    {
        loopSource.Stop();
        loopSource.clip = lowHPBreathing;
        loopSource.volume = loopVolume * 0.8f;
        loopSource.Play();
    }

    private void StopLoop()
    {
        if (loopSource.isPlaying)
            loopSource.Stop();
    }

    public void PlayJump() => PlayOneShot(jump);
    public void PlayAttack() => PlayOneShot(attack);
    public void PlayBlockedDamage() => PlayOneShot(damageBlocked);
    
    public void PlayDamage()
    {
        AudioClip chosen = RandomDamageClip();
        if (chosen) PlayOneShot(chosen);
    }

    private AudioClip RandomDamageClip()
    {
        int roll = Random.Range(0, 3);
        return roll switch
        {
            0 => damage1,
            1 => damage2,
            2 => damage3,
            _ => damage1
        };
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        oneShotSource.volume = sfxVolume;
        oneShotSource.PlayOneShot(clip);
    }
}