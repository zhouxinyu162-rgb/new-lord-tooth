using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    public string keyTag = "Key";

    public UnityEvent OnTriggerZoneEntered = new UnityEvent();
    public UnityEvent OnTriggerZoneExited = new UnityEvent();

    private void Update()
    {
 
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (OnTriggerZoneEntered != null && CheckCollisionObject(other.gameObject))
        {
            OnTriggerZoneEntered.Invoke(); 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other);
        if (OnTriggerZoneExited != null && CheckCollisionObject(other.gameObject))
        {
            OnTriggerZoneExited.Invoke();
        }
    }

    private bool CheckCollisionObject(GameObject otherGameObject)
    {
        return otherGameObject.CompareTag(keyTag.Trim());
    }
}
