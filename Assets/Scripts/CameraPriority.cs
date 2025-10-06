using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

#pragma warning disable CS0618
[DisallowMultipleComponent]
public class CameraPriority : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [Tooltip("Camera that performs the opening dolly shot.")]
    [SerializeField] private GameObject dollyCameraRoot;

    [Tooltip("Camera that should be active once gameplay begins (typically attached to the player).")]
    [SerializeField] private GameObject playerCameraRoot;

    [Header("Path Driver (Optional)")]
    [Tooltip("Reference to the Cinemachine cart that moves along the spline/path. If left empty the component searches on the dolly camera root and this GameObject.")]
    [SerializeField] private Component dollyCartComponent;

    [Header("Playback Settings")]
    [Tooltip("Automatically start the dolly-to-player camera handoff when this component starts.")]
    [SerializeField] private bool playOnStart = true;

    [Tooltip("Normalized completion threshold that the dolly must reach before switching to the player camera.")]
    [Range(0.7f, 0.999f)]
    [SerializeField] private float completionThreshold = 0.98f;

    [Tooltip("Maximum time (in seconds) to wait for the dolly to finish. Set to 0 to wait indefinitely.")]
    [Min(0f)]
    [SerializeField] private float completionTimeout = 0f;

    [Tooltip("Extra delay (in seconds) after the dolly completes before activating the player camera.")]
    [Min(0f)]
    [SerializeField] private float postCompletionDelay = 0.25f;

    [Header("Events")]
    [Tooltip("Invoked the first time the player camera becomes active.")]
    [SerializeField] private UnityEvent onSequenceFinished;

    private CinemachineSplineCart _splineCart;
    private CinemachineDollyCart _legacyCart;
    private Coroutine _activeRoutine;
    private bool _sequenceFinished;

    private void OnValidate()
    {
        CacheCartReferences();
        completionThreshold = Mathf.Clamp(completionThreshold, 0.7f, 0.999f);
    }

    private void Awake()
    {
        CacheCartReferences();
    }

    private void Start()
    {
        if (playOnStart)
            BeginSequence();
    }

    private void OnDisable()
    {
        if (_activeRoutine != null)
        {
            StopCoroutine(_activeRoutine);
            _activeRoutine = null;
        }

        if (!_sequenceFinished)
            EnsurePlayerCameraActive();
    }

    public void BeginSequence()
    {
        if (!isActiveAndEnabled)
            return;

        CacheCartReferences();

        if (dollyCameraRoot == null)
            Debug.LogWarning("CameraPriority is missing a reference to the dolly camera root.", this);

        if (playerCameraRoot == null)
            Debug.LogWarning("CameraPriority is missing a reference to the player camera root.", this);

        if (_sequenceFinished)
        {
            EnsurePlayerCameraActive();
            return;
        }

        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(SequenceRoutine());
    }

    public void ForceFinish()
    {
        if (_activeRoutine != null)
        {
            StopCoroutine(_activeRoutine);
            _activeRoutine = null;
        }

        EnsurePlayerCameraActive();
        _sequenceFinished = true;
        onSequenceFinished?.Invoke();
    }

    private IEnumerator SequenceRoutine()
    {
        SetCameraStates(dollyActive: true, playerActive: false);

        yield return WaitForDollyCompletion();

        if (postCompletionDelay > 0f)
            yield return new WaitForSeconds(postCompletionDelay);

        EnsurePlayerCameraActive();
        _sequenceFinished = true;
        _activeRoutine = null;
        onSequenceFinished?.Invoke();
    }

    private IEnumerator WaitForDollyCompletion()
    {
        if (!HasValidCart())
            yield break;

        float startTime = Time.time;
        float timeout = completionTimeout > 0f ? completionTimeout : float.PositiveInfinity;
        float highestProgress = 0f;

        while (true)
        {
            float progress = GetNormalizedProgress();

            if (progress >= completionThreshold || (highestProgress >= completionThreshold && progress < 0.05f))
                yield break;

            highestProgress = Mathf.Max(highestProgress, progress);

            if (Time.time - startTime >= timeout)
            {
                Debug.LogWarning("CameraPriority timed out waiting for the dolly cart to finish. Falling back to player camera.", this);
                yield break;
            }

            yield return null;
        }
    }

    private void EnsurePlayerCameraActive()
    {
        SetCameraStates(dollyActive: false, playerActive: true);
    }

    private void SetCameraStates(bool dollyActive, bool playerActive)
    {
        if (dollyCameraRoot != null)
            dollyCameraRoot.SetActive(dollyActive);

        if (playerCameraRoot != null)
            playerCameraRoot.SetActive(playerActive);
    }

    private bool HasValidCart()
    {
        if (_splineCart != null)
        {
            var container = _splineCart.Spline;
            if (!IsSplineContainerValid(container))
            {
                Debug.LogWarning("CameraPriority has a CinemachineSplineCart without an assigned spline. Switching immediately.", this);
                return false;
            }

            return true;
        }

        if (_legacyCart != null)
        {
            if (_legacyCart.m_Path == null)
            {
                Debug.LogWarning("CameraPriority has a CinemachineDollyCart without an assigned path. Switching immediately.", this);
                return false;
            }

            return true;
        }

        Debug.LogWarning("CameraPriority has no Cinemachine cart assigned. Switching immediately.", this);
        return false;
    }

    private float GetNormalizedProgress()
    {
        if (_splineCart != null)
            return EvaluateSplineCartProgress();

        if (_legacyCart != null)
            return EvaluateLegacyCartProgress();

        return 1f;
    }

    private float EvaluateSplineCartProgress()
    {
        var container = _splineCart.Spline;
        if (!IsSplineContainerValid(container))
            return 1f;

        var spline = container.Splines[0];
        float position = SanitizeSplinePosition(spline, _splineCart.SplinePosition, _splineCart.PositionUnits, out var maxPos);

        if (maxPos <= Mathf.Epsilon)
            return 1f;

        return Mathf.Clamp01(position / maxPos);
    }

    private float EvaluateLegacyCartProgress()
    {
        var path = _legacyCart.m_Path;
        if (path == null)
            return 1f;

        var units = _legacyCart.m_PositionUnits;
        float max = path.MaxUnit(units);
        if (max <= Mathf.Epsilon)
            return 1f;

        float standardized = path.StandardizeUnit(_legacyCart.m_Position, units);
        return Mathf.Clamp01(standardized / max);
    }

    private void CacheCartReferences()
    {
        if (dollyCartComponent != null)
        {
            _splineCart = dollyCartComponent as CinemachineSplineCart ?? dollyCartComponent.GetComponent<CinemachineSplineCart>();
            _legacyCart = dollyCartComponent as CinemachineDollyCart ?? dollyCartComponent.GetComponent<CinemachineDollyCart>();
        }

        if (_splineCart == null && dollyCameraRoot != null)
            _splineCart = dollyCameraRoot.GetComponent<CinemachineSplineCart>();

        if (_legacyCart == null && dollyCameraRoot != null)
            _legacyCart = dollyCameraRoot.GetComponent<CinemachineDollyCart>();

        if (_splineCart == null)
            _splineCart = GetComponent<CinemachineSplineCart>();

        if (_legacyCart == null)
            _legacyCart = GetComponent<CinemachineDollyCart>();
    }

    private static bool IsSplineContainerValid(SplineContainer container)
    {
        if (container == null)
            return false;

        var splines = container.Splines;
        return splines != null && splines.Count > 0 && splines[0] != null;
    }

    private static float SanitizeSplinePosition(Spline spline, float position, PathIndexUnit units, out float maxPosition)
    {
        if (spline == null)
        {
            maxPosition = 0f;
            return 0f;
        }

        switch (units)
        {
            case PathIndexUnit.Distance:
                maxPosition = Mathf.Max(0f, spline.GetLength());
                break;
            case PathIndexUnit.Knot:
                int knotCount = spline.Count;
                maxPosition = (!spline.Closed || knotCount < 2) ? Mathf.Max(0, knotCount - 1) : knotCount;
                break;
            default:
                maxPosition = 1f;
                break;
        }

        if (units == PathIndexUnit.Normalized)
            return Mathf.Clamp01(position);

        if (maxPosition <= Mathf.Epsilon)
            return 0f;

        if (!spline.Closed)
            return Mathf.Clamp(position, 0f, maxPosition);

        float wrapped = position % maxPosition;
        if (wrapped < 0f)
            wrapped += maxPosition;
        return wrapped;
    }
}

#pragma warning restore CS0618
