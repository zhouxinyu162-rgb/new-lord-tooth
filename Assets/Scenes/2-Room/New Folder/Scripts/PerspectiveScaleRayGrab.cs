using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class PerspectiveScaleRayGrab : MonoBehaviour
{
    [Header("Camera (HMD)")]
    [Tooltip("If empty, will use Camera.main at runtime.")]
    public Transform hmd;

    [Header("Ray / Distance")]
    [Tooltip("Max distance for raycast placement while grabbed.")]
    public float maxRayDistance = 50f;

    [Tooltip("If raycast hits nothing, place object this far in front of the interactor.")]
    public float fallbackDistance = 2f;

    [Tooltip("Layer mask for raycast. Default = Everything.")]
    public LayerMask raycastMask = ~0;

    [Header("Smoothing")]
    [Tooltip("Higher = snappier position follow.")]
    public float positionLerp = 25f;

    [Tooltip("Higher = snappier scale follow.")]
    public float scaleLerp = 25f;

    [Header("Scale Clamp")]
    public float minUniformScale = 0.05f;
    public float maxUniformScale = 50f;

    [Header("Physics While Grabbed")]
    [Tooltip("Set Rigidbody.isKinematic = true while grabbed to reduce jitter.")]
    public bool makeKinematicWhileGrabbed = true;

    [Tooltip("Disable gravity while grabbed.")]
    public bool disableGravityWhileGrabbed = true;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private XRBaseInteractor interactor; // Works with Near-Far Interactor and others
    private Rigidbody rb;

    private Vector3 initialScale;
    private float initialHmdDistance;

    private bool grabbed;
    private bool oldKinematic;
    private bool oldUseGravity;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);

        if (hmd == null && Camera.main != null)
            hmd = Camera.main.transform;
    }

    private void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as XRBaseInteractor;

        if (hmd == null && Camera.main != null)
            hmd = Camera.main.transform;

        if (interactor == null || hmd == null)
        {
            grabbed = false;
            Debug.LogWarning("[PerspectiveScaleRayGrab] Missing interactor or HMD.");
            return;
        }

        initialScale = transform.localScale;

        // Use current object distance to HMD as baseline
        initialHmdDistance = Vector3.Distance(hmd.position, transform.position);
        if (initialHmdDistance < 0.001f) initialHmdDistance = 0.001f;

        // Optional: make physics stable during scale/position manipulation
        if (rb != null)
        {
            oldKinematic = rb.isKinematic;
            oldUseGravity = rb.useGravity;

            if (makeKinematicWhileGrabbed) rb.isKinematic = true;
            if (disableGravityWhileGrabbed) rb.useGravity = false;
        }

        grabbed = true;

        Debug.Log("[PerspectiveScaleRayGrab] Grabbed. Interactor=" + interactor.name);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        grabbed = false;
        interactor = null;

        // Restore physics
        if (rb != null)
        {
            rb.isKinematic = oldKinematic;
            rb.useGravity = oldUseGravity;
        }

        // Important: We DO NOT reset scale/position.
        // This makes the object "stay big" after release (Superliminal feel).
        Debug.Log("[PerspectiveScaleRayGrab] Released.");
    }

    private void Update()
    {
        if (!grabbed || interactor == null || hmd == null) return;

        // Ray origin & direction:
        // For Near-Far Interactor (and most interactors), transform.forward works well.
        Vector3 origin = interactor.transform.position;
        Vector3 dir = interactor.transform.forward;

        // Find target point in the world
        Vector3 targetPoint;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, maxRayDistance, raycastMask, QueryTriggerInteraction.Ignore))
            targetPoint = hit.point;
        else
            targetPoint = origin + dir * fallbackDistance;

        // Compute new scale factor based on distance to HMD (camera)
        float currentHmdDistance = Vector3.Distance(hmd.position, targetPoint);
        if (currentHmdDistance < 0.001f) currentHmdDistance = 0.001f;

        float factor = currentHmdDistance / initialHmdDistance;

        // Uniform scale (Superliminal style)
        Vector3 targetScale = initialScale * factor;

        // Clamp uniformly (avoid infinite growth/shrink)
        float uniform = targetScale.x; // assume uniform; if not, this still works as a guide
        uniform = Mathf.Clamp(uniform, minUniformScale, maxUniformScale);

        // Rebuild uniform scale
        targetScale = new Vector3(uniform, uniform, uniform);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * positionLerp);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerp);
    }
}