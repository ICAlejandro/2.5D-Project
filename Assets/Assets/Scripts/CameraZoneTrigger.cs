using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public GameObject balconyCamera;
    private GameObject mainCamera;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && balconyCamera != null && mainCamera != null)
        {
            mainCamera.SetActive(false);
            balconyCamera.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && balconyCamera != null && mainCamera != null)
        {
            balconyCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
}