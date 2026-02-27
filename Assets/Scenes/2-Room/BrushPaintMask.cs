using UnityEngine;

public class BrushPaintMask : MonoBehaviour
{
    public LayerMask stainLayer;
    public RenderTexture maskRT;
    public Material stampMat;

    public float brushSize = 0.05f;
    public float cleanSpeed = 0.5f; // 越大越快，0.2 很慢，1 很快
    public Transform tip;

private void OnTriggerStay(Collider other)
{
    if (!other.gameObject.layer.Equals(Mathf.RoundToInt(Mathf.Log(stainLayer.value, 2)))) { } // 可不写，下面 Raycast 有 mask

    if (tip == null) return;

    // 朝对方表面最近点发射
    Vector3 target = other.ClosestPoint(tip.position);
    Vector3 dir = (target - tip.position);
    float dist = dir.magnitude + 0.02f;
    if (dist <= 0.0001f) return;
    dir /= dist;

    if (Physics.Raycast(tip.position, dir, out RaycastHit hit, dist, stainLayer, QueryTriggerInteraction.Ignore))
    {
        Paint(hit.textureCoord);
    }
}
void Paint(Vector2 uv)
{
    stampMat.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
    stampMat.SetFloat("_Radius", brushSize);

    // 关键：每帧只加一点点
    stampMat.SetFloat("_Strength", cleanSpeed * Time.deltaTime);

    RenderTexture temp = RenderTexture.GetTemporary(maskRT.width, maskRT.height, 0, maskRT.format);

    Graphics.Blit(maskRT, temp);
    Graphics.Blit(temp, maskRT, stampMat);

    RenderTexture.ReleaseTemporary(temp);
}
}