using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TriggerEnterEvents : MonoBehaviour
{
    public string checkTag;

    public UnityEvent triggerEvent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(checkTag))
        {
            triggerEvent.Invoke();
        }
    }
}
