using UnityEngine;

public class StainDisappear : MonoBehaviour
{
    public float secondsToClean = 3f;

    // 把刷毛 trigger zone(p4) 拖到这里（最稳，不用Tag不怕层级）
    public Collider brushTrigger;

    float timer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (brushTrigger == null) return;

        // 只认指定的刷毛触发器
        if (other != brushTrigger) return;

        timer += Time.deltaTime;

        // 调试：看看是不是真的在计时
        // Debug.Log($"Cleaning... {timer:F2}/{secondsToClean}");

        if (timer >= secondsToClean)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == brushTrigger)
        {
            timer = 0f; // 离开就重置（连续刷3秒）
        }
    }
}