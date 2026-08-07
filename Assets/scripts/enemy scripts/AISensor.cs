using UnityEngine;

public class AISensor : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"[AI SENSOR] Player Detected! Target entered detection radius: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"[AI SENSOR] Player Lost! Target exited detection radius: {other.name}");
        }
    }
}