using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public GameObject tooptipText;
    public GameObject cursorDefault;
    public GameObject cursorHand;
    public GameObject cursorGrabbed;
    private SuperEnginal superEnginal;
    private GameObject target;

    void Start() {
        superEnginal = GetComponent<SuperEnginal>();
    }

    void Update() {
        if (superEnginal.target != null) {
            target = null;
            SuperProp sp = superEnginal.target.GetComponent<SuperProp>();
            if (sp != null) {
                
            }
            tooptipText.GetComponent<CanvasGroup>().alpha = 0;
            cursorDefault.GetComponent<CanvasGroup>().alpha = 0;
            cursorHand.GetComponent<CanvasGroup>().alpha = 0;
            cursorGrabbed.GetComponent<CanvasGroup>().alpha = 1;
        } else {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, superEnginal.maxScaleDistance, superEnginal.targetMask)) {
                if (target == null) {
                    target = hit.transform.gameObject;
                    SuperProp sp = target.GetComponent<SuperProp>();
                    if (sp != null) {
                        
                        tooptipText.GetComponent<Text>().text = sp.objectName;
                        tooptipText.GetComponent<CanvasGroup>().alpha = 1;
                        cursorDefault.GetComponent<CanvasGroup>().alpha = 0;
                        cursorHand.GetComponent<CanvasGroup>().alpha = 1;
                        cursorGrabbed.GetComponent<CanvasGroup>().alpha = 0;
                    }
                }
            } else {
               
                tooptipText.GetComponent<CanvasGroup>().alpha = 0;
                cursorDefault.GetComponent<CanvasGroup>().alpha = 1;
                cursorHand.GetComponent<CanvasGroup>().alpha = 0;
                cursorGrabbed.GetComponent<CanvasGroup>().alpha = 0;
                target = null;
            }
        }
    }
}
