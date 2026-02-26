using UnityEngine;

public class ScrubToClean : MonoBehaviour
{
    public string brushTag = "Brush";
    public Renderer stainRenderer;
    public float cleanSpeed = 0.6f; // 每秒擦掉多少（越大越快）

    private float alpha = 1f;

    void Reset()
    {
        stainRenderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        // 每次出现污渍时重置为不透明（如果你希望重复玩）
        alpha = 1f;
        ApplyAlpha(alpha);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(brushTag)) return;

        alpha = Mathf.Clamp01(alpha - cleanSpeed * Time.deltaTime);
        ApplyAlpha(alpha);
    }

    void ApplyAlpha(float a)
    {
        if (stainRenderer == null) return;
        var mat = stainRenderer.material;
        var c = mat.color;
        c.a = a;
        mat.color = c;
    }
}