using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitedTrigger;

    public event System.Action<Collider> StayedTrigger;
    private void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ExitedTrigger?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        StayedTrigger?.Invoke(other);
    }
}
