using UnityEngine;

public class StateVisuals : MonoBehaviour
{
    public Transform iconRoot;
    public SpriteRenderer iconRenderer;

    public Sprite idleIcon;
    public Sprite alertIcon;
    public Sprite attackIcon;
    public Sprite blockIcon;
    public Sprite stunnedIcon;  
    public Sprite orbitStarIcon; 
    public Sprite exposedIcon;
    public Sprite recoverIcon;
    public Sprite deadIcon;

    public float popScale = 1.4f;
    public float popDuration = 0.15f;
    public float returnDuration = 0.15f;

    public bool enableOrbitingStars = true;
    public int starCount = 3;
    public float orbitRadius = 0.5f;
    public float orbitHeight = 0.25f;
    public float orbitSpeed = 180f;

    private Transform[] KOStars;
    private EnemyStateHandler ai;
    private IEnemyState lastState;

    private Vector3 baseScale;
    private bool isPopping = false;
    private float orbitAngle = 0f;

    private void Awake()
    {
        ai = GetComponent<EnemyStateHandler>();

        if (iconRoot == null)
        {
            GameObject g = new GameObject("IconRoot");
            g.transform.SetParent(transform);
            g.transform.localPosition = new Vector3(0, 2.2f, 0);
            iconRoot = g.transform;
        }

        if (iconRenderer == null)
            iconRenderer = iconRoot.gameObject.AddComponent<SpriteRenderer>();

        baseScale = iconRoot.localScale;

        KOStars = new Transform[starCount];

        for (int i = 0; i < starCount; i++)
        {
            GameObject s = new GameObject("KOStar_" + i);
            s.transform.SetParent(iconRoot);
            s.transform.localPosition = Vector3.zero;
            var sr = s.AddComponent<SpriteRenderer>();
            sr.sprite = orbitStarIcon;
            sr.sortingOrder = iconRenderer.sortingOrder + 1;
            KOStars[i] = s.transform;
            s.SetActive(false);
        }
    }

    private void Update()
    {
        if (Camera.main)
            iconRoot.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        IEnemyState state = ai.GetCurrentState();
        if (state != lastState)
        {
            UpdateIcon(state);
            PlayPopAnimation();
            lastState = state;
        }

        if (state is StunState1)
            UpdateOrbitingStars();
        else
            DisableOrbitingStars();
    }

    private void UpdateIcon(IEnemyState state)
    {
        if (state is StunState1 && enableOrbitingStars)
        {
            iconRenderer.sprite = null;
            EnableOrbitingStars();
            return;
        }

        DisableOrbitingStars();

        if (state is IdleState1)
            iconRenderer.sprite = idleIcon;
        else if (state is AlertState1)
            iconRenderer.sprite = alertIcon;
        else if (state is AttackState1)
            iconRenderer.sprite = attackIcon;
        else if (state is BlockState1)
            iconRenderer.sprite = blockIcon;
        else if (state is ExposedState1)
            iconRenderer.sprite = exposedIcon;
        else if (state is RecoverState1)
            iconRenderer.sprite = recoverIcon;
        else if (state is DeadState1)
            iconRenderer.sprite = deadIcon;
        else
            iconRenderer.sprite = idleIcon;
    }

    private void EnableOrbitingStars()
    {
        for (int i = 0; i < KOStars.Length; i++)
            KOStars[i].gameObject.SetActive(true);
    }

    private void DisableOrbitingStars()
    {
        for (int i = 0; i < KOStars.Length; i++)
            KOStars[i].gameObject.SetActive(false);
    }

    private void UpdateOrbitingStars()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        for (int i = 0; i < KOStars.Length; i++)
        {
            float step = 360f / KOStars.Length;
            float angle = orbitAngle + step * i;

            float rad = angle * Mathf.Deg2Rad;

            KOStars[i].localPosition = new Vector3(
                Mathf.Cos(rad) * orbitRadius,
                orbitHeight,
                Mathf.Sin(rad) * orbitRadius
            );
        }
    }

    private void PlayPopAnimation()
    {
        if (isPopping) return;
        StartCoroutine(PopRoutine());
    }

    private System.Collections.IEnumerator PopRoutine()
    {
        isPopping = true;

        float t = 0f;
        while (t < popDuration)
        {
            float k = t / popDuration;
            iconRoot.localScale = Vector3.Lerp(baseScale, baseScale * popScale, k);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < returnDuration)
        {
            float k = t / returnDuration;
            iconRoot.localScale = Vector3.Lerp(baseScale * popScale, baseScale, k);
            t += Time.deltaTime;
            yield return null;
        }

        iconRoot.localScale = baseScale;
        isPopping = false;
    }
}
