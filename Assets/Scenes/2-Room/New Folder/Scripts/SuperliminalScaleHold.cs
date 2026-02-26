using UnityEngine;

public class SuperliminalScaleHold : MonoBehaviour
{
 public Transform viewOrigin;          // 建议拖 XR 的 Main Camera (CenterEye)
    public float holdDistance = 1.2f;     // 抓住时物体在视线前方多远（可做成滚轮/摇杆调）

    private bool holding;
    private float startDistance;
    private Vector3 startScale;

    void Update()
    {
        if (!holding || viewOrigin == null) return;

        // 把物体固定在视线前方 holdDistance 处
        Vector3 targetPos = viewOrigin.position + viewOrigin.forward * holdDistance;
        transform.position = targetPos;

        // 按距离比例缩放（Superliminal 核心）
        float d = holdDistance;
        float ratio = (startDistance <= 0.0001f) ? 1f : (d / startDistance);
        transform.localScale = startScale * ratio;
    }

    public void BeginHold()
    {
        if (viewOrigin == null) return;
        holding = true;

        startDistance = Vector3.Distance(viewOrigin.position, transform.position);
        startScale = transform.localScale;

        // 可选：抓起时关物理
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    public void EndHold()
    {
        holding = false;

        // 可选：放下恢复物理
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
    }

    // 可选：给摇杆/按钮调用来改变距离（越远越大）
    public void SetHoldDistance(float d)
    {
        holdDistance = Mathf.Max(0.2f, d);
    }
}
