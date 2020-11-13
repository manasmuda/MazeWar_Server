using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TriggerExitEvents : MonoBehaviour
{
    public string checkTag;

    public UnityEvent triggerEvent;
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(checkTag))
        {
            triggerEvent.Invoke();
        }
    }
}
