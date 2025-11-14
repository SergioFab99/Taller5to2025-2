using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Splits rendering into a grayscale base pass (main camera) and a color-only pass (helper camera).
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class SelectiveColorPostProcess : MonoBehaviour
{
    [Header("Grayscale Post Effect")]
    [Tooltip("Shader that converts the frame to grayscale. Drag your existing shader here.")]
    public Shader grayscaleShader;

    [Header("Objects That Stay In Color")]
    [Tooltip("Renderers that must remain in color. Their layers are reassigned automatically.")]
    public List<Renderer> colorRenderers = new List<Renderer>();

    [Tooltip("Name of the layer used exclusively by the color-only renderers.")]
    public string colorLayerName = "ColorOnly";

    private Material grayscaleMaterial;
    private Camera mainCamera;
    private Camera colorCamera;
    private int colorLayer = -1;
    private int originalMainMask;
    private bool mainMaskOverrideActive;
    private readonly Dictionary<Renderer, int> originalLayers = new Dictionary<Renderer, int>();
    private static readonly List<Renderer> scratchList = new List<Renderer>();
    private bool missingLayerWarned;

    private void OnEnable()
    {
        mainCamera = GetComponent<Camera>();
        ValidateShader();
        missingLayerWarned = false;
        CacheColorLayer();
        SetupColorCamera();
        SyncColorRenderers();
    }

    private void OnDisable()
    {
        RestoreRenderersLayer();
        TeardownColorCamera();
        CleanupMaterial();
    }

    private void OnValidate()
    {
        if (!mainCamera) mainCamera = GetComponent<Camera>();
        missingLayerWarned = false;
        CacheColorLayer();

        if (!Application.isPlaying && grayscaleMaterial != null && grayscaleMaterial.shader != grayscaleShader)
        {
            CleanupMaterial();
        }

        if (isActiveAndEnabled)
        {
            ValidateShader();
            SetupColorCamera();
            SyncColorRenderers();
        }
    }

    private void LateUpdate()
    {
        if (colorLayer < 0) CacheColorLayer();
        if (!colorCamera && colorLayer >= 0) SetupColorCamera();
        if (colorCamera && colorLayer < 0) TeardownColorCamera();

        SyncColorRenderers();
        if (colorCamera && mainCamera) colorCamera.enabled = mainCamera.enabled;

        if (mainMaskOverrideActive && mainCamera && colorLayer >= 0)
        {
            int colorLayerMask = 1 << colorLayer;
            int currentWithColor = mainCamera.cullingMask | colorLayerMask;
            if (currentWithColor != originalMainMask)
            {
                originalMainMask = currentWithColor;
                mainCamera.cullingMask = originalMainMask & ~colorLayerMask;
            }
        }
    }

    private void ValidateShader()
    {
        if (!grayscaleShader)
        {
            Debug.LogWarning($"{nameof(SelectiveColorPostProcess)}: Assign a grayscale shader in the inspector.");
            return;
        }

        if (!grayscaleShader.isSupported)
        {
            Debug.LogWarning($"{nameof(SelectiveColorPostProcess)}: The assigned shader is not supported on this platform.");
            return;
        }

        if (!grayscaleMaterial || grayscaleMaterial.shader != grayscaleShader)
        {
            CleanupMaterial();
            grayscaleMaterial = new Material(grayscaleShader) { hideFlags = HideFlags.HideAndDontSave };
        }
    }

    private void CacheColorLayer()
    {
        colorLayer = LayerMask.NameToLayer(colorLayerName);
        if (colorLayer < 0)
        {
            if (!missingLayerWarned)
            {
                Debug.LogWarning($"{nameof(SelectiveColorPostProcess)}: Create a layer named '{colorLayerName}' via Project Settings > Tags and Layers.");
                missingLayerWarned = true;
            }
            ReleaseMainCameraMask();
        }
        else
        {
            missingLayerWarned = false;
        }
    }

    private void SetupColorCamera()
    {
        if (!mainCamera || colorLayer < 0) return;

        int colorLayerMask = 1 << colorLayer;
        originalMainMask = mainCamera.cullingMask | colorLayerMask;
        mainMaskOverrideActive = true;

        if (!colorCamera)
        {
            var colorCameraObject = new GameObject("ColorCamera")
            {
                hideFlags = Application.isPlaying ? HideFlags.None : HideFlags.HideInHierarchy | HideFlags.DontSave
            };

            colorCameraObject.transform.SetParent(transform, false);
            colorCamera = colorCameraObject.AddComponent<Camera>();
        }

        colorCamera.CopyFrom(mainCamera);
        colorCamera.cullingMask = colorLayerMask;
        colorCamera.clearFlags = CameraClearFlags.Depth;
        colorCamera.depth = mainCamera.depth + 1f;
        colorCamera.useOcclusionCulling = mainCamera.useOcclusionCulling;
        colorCamera.allowMSAA = mainCamera.allowMSAA;
        colorCamera.allowHDR = mainCamera.allowHDR;
        colorCamera.stereoTargetEye = mainCamera.stereoTargetEye;
        colorCamera.targetTexture = null;
        colorCamera.enabled = mainCamera.enabled;
        colorCamera.name = "ColorCamera";

        mainCamera.cullingMask = originalMainMask & ~colorLayerMask;
    }

    private void TeardownColorCamera()
    {
        ReleaseMainCameraMask();

        if (!colorCamera) return;

        if (Application.isPlaying)
        {
            Destroy(colorCamera.gameObject);
        }
        else
        {
            DestroyImmediate(colorCamera.gameObject);
        }

        colorCamera = null;
    }

    private void SyncColorRenderers()
    {
        if (colorLayer < 0)
        {
            RestoreRenderersLayer();
            return;
        }

        foreach (var renderer in colorRenderers)
        {
            if (!renderer) continue;

            if (!originalLayers.ContainsKey(renderer)) originalLayers.Add(renderer, renderer.gameObject.layer);
            renderer.gameObject.layer = colorLayer;
        }

        scratchList.Clear();
        foreach (var kvp in originalLayers)
        {
            if (!colorRenderers.Contains(kvp.Key)) scratchList.Add(kvp.Key);
        }

        foreach (var renderer in scratchList)
        {
            if (!renderer)
            {
                originalLayers.Remove(renderer);
                continue;
            }

            if (!originalLayers.TryGetValue(renderer, out var originalLayer))
            {
                continue;
            }

            renderer.gameObject.layer = originalLayer;
            originalLayers.Remove(renderer);
        }
    }

    private void RestoreRenderersLayer()
    {
        foreach (var kvp in originalLayers)
        {
            if (!kvp.Key) continue;
            kvp.Key.gameObject.layer = kvp.Value;
        }

        originalLayers.Clear();
        ReleaseMainCameraMask();
    }

    private void CleanupMaterial()
    {
        if (!grayscaleMaterial) return;

        if (Application.isPlaying)
        {
            Destroy(grayscaleMaterial);
        }
        else
        {
            DestroyImmediate(grayscaleMaterial);
        }

        grayscaleMaterial = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Apply the grayscale shader to the main camera while the color camera overlays its output.
        if (!grayscaleMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, grayscaleMaterial);
    }

    private void ReleaseMainCameraMask()
    {
        if (!mainMaskOverrideActive || !mainCamera) return;

        mainCamera.cullingMask = originalMainMask;
        mainMaskOverrideActive = false;
    }
}
