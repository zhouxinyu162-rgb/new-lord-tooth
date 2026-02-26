using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class ScaleWhileGrabbed : MonoBehaviour
{
    [Header("References")]
    public Transform head;                 // 不填则自动用 Camera.main
    public Transform distanceFrom;         // 不填则用抓取点/手的位置

    [Header("Distance -> Scale")]
    public float minDistance = 0.2f;       // 最近距离（米）
    public float maxDistance = 1.5f;       // 最远距离（米）
    public float minScale = 0.2f;          // 最小缩放倍数
    public float maxScale = 2.0f;          // 最大缩放倍数
    public bool nearBigger = true;         // true=越近越大；false=越近越小
    public float smooth = 12f;             // 平滑

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool grabbed = false;
    private Vector3 baseScale;
    private Transform currentInteractor;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        baseScale = transform.localScale;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void Start()
    {
        if (head == null && Camera.main != null)
            head = Camera.main.transform;
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        currentInteractor = args.interactorObject.transform;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        grabbed = false;
        currentInteractor = null;
        // 可选：松手后回到原始大小
        // transform.localScale = baseScale;
    }

    void Update()
    {
        if (!grabbed) return;
        if (head == null) return;

        Transform from = distanceFrom != null ? distanceFrom : currentInteractor;
        if (from == null) return;

        float d = Vector3.Distance(from.position, head.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, d); // 0..1

        // nearBigger=true：距离越小，t越小，但我们要越大 -> 反转
        if (nearBigger) t = 1f - t;

        float s = Mathf.Lerp(minScale, maxScale, t);
        Vector3 desired = baseScale * s;

        transform.localScale = Vector3.Lerp(transform.localScale, desired, Time.deltaTime * smooth);
    }
}