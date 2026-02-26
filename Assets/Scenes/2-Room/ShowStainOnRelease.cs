using UnityEngine;

public class ShowStainOnRelease : MonoBehaviour
{
    public GameObject stain;   // 拖入 Stain
    public void ShowStain()
    {
        if (stain != null) stain.SetActive(true);
    }
}