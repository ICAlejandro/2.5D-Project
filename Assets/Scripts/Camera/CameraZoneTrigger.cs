using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject zoneCamera; // Renamed 'balconyCamera' to be generic and reusable

    void Start()
    {
        // Fallback: If you forget to assign the main camera, find it automatically
        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && zoneCamera != null && mainCamera != null)
        {
            mainCamera.SetActive(false);
            zoneCamera.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && zoneCamera != null && mainCamera != null)
        {
            zoneCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
}