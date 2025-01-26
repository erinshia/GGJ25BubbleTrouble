using UnityEngine;

public class MouseCapture : MonoBehaviour
{
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
