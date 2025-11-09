using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Volume ")]
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float loopVolume = 1f;

    [Header("Movement")]
    [SerializeField] private AudioClip walkNormal;
    [SerializeField] private AudioClip walkDirt;
    [SerializeField] private AudioClip runNormal;
    [SerializeField] private AudioClip runDirt;
    [SerializeField] private AudioClip drunkWalkNormal;
    [SerializeField] private AudioClip drunkWalkDirt;

    [Header("Low HP")]
    [SerializeField] private AudioClip lowHPBreathing;

    [Header("OneShot Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip damage1;
    [SerializeField] private AudioClip damage2;
    [SerializeField] private AudioClip damage3;
    [SerializeField] private AudioClip damageBlockedClip; 

    private AudioSource loopSource;
    private AudioSource oneShotSource;
    [SerializeField] private Player player;

    private enum MoveState { None, Walk, Run, DrunkWalk }
    private MoveState currentState = MoveState.None;

    private bool isOnDirt = false;
    private bool lowHPActive = false;
    private bool drunkWalk = false;

    private void Awake()
    {
        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.loop = false;
        oneShotSource.playOnAwake = false;
    }

    private void Update()
    {
        HandleMovementAudio();
    }

    public void SetSurface(bool onDirt)
    {
        if (isOnDirt != onDirt)
        {
            isOnDirt = onDirt;
            UpdateMovementLoop();
        }
    }

    public void SetDrunkWalk(bool active)
    {
        if (drunkWalk == active) return;
        drunkWalk = active;
        UpdateMovementLoop();
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
        bool running = Input.GetKey(KeyCode.LeftShift);

        MoveState desired = MoveState.None;

        if (moving)
        {
            if (drunkWalk) desired = MoveState.DrunkWalk;
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


    public void PlayJump() => PlayOneShot(jumpClip);
    public void PlayBlockedDamage() => PlayOneShot(damageBlockedClip);
    public void PlayAttack() => PlayOneShot(attackClip);
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
