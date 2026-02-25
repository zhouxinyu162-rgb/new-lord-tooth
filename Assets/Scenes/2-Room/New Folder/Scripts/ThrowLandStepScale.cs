using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class ThrowLandStepScale : MonoBehaviour
{
    [Header("Step Scale")]
    public float stepMultiplier = 1.2f;      // 每次落地后扩大 1.2x
    public float maxMultiplier = 6f;         // 最大倍数（防止无限大）
    public float scaleSmooth = 12f;          // 缩放平滑

    [Header("Throw & Land Detection")]
    public float minThrowSpeed = 1.0f;       // 松手瞬间速度超过这个才算“扔”
    public float landSpeedThreshold = 0.3f;  // 落地时速度小于这个认为“停住”
    public float minAirTime = 0.08f;         // 至少离开地面这么久才算一次有效扔
    public float groundCheckDistance = 0.08f;// 地面检测距离（按物体大小可调）
    public LayerMask groundMask = ~0;        // 地面层（默认全部）

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    private Vector3 baseScale;
    private float currentMultiplier = 1f;
    private float targetMultiplier = 1f;

    private bool grabbed = false;
    private bool thrownArmed = false;        // 已经“算作扔出”，等待落地触发一次
    private float releaseTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        baseScale = transform.localScale;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        thrownArmed = false; // 抓住时重置，防止上一轮残留
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        grabbed = false;
        releaseTime = Time.time;

        // 只有“松手瞬间速度够大”才认定为一次“扔”
        if (rb.linearVelocity.magnitude >= minThrowSpeed)
            thrownArmed = true;
    }

    void Update()
    {
        // 平滑缩放到目标倍数
        currentMultiplier = Mathf.Lerp(currentMultiplier, targetMultiplier, Time.deltaTime * scaleSmooth);
        transform.localScale = baseScale * currentMultiplier;

        if (grabbed) return;
        if (!thrownArmed) return;

        // 必须在空中待一会儿，避免松手就贴地立刻触发
        if (Time.time - releaseTime < minAirTime) return;

        // 判定“落地”：接近地面 + 速度变小（基本停住）
        if (IsGrounded() && rb.linearVelocity.magnitude <= landSpeedThreshold)
        {
            // 触发一次阶梯变大
            targetMultiplier = Mathf.Min(targetMultiplier * stepMultiplier, maxMultiplier);

            // 这一轮扔结束，只触发一次
            thrownArmed = false;
        }
    }

    private bool IsGrounded()
    {
        // 从物体中心向下射线检测地面
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }
}