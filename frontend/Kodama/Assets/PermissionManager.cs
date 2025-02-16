using UnityEngine;
using UnityEngine.Android;

public class CameraPermissionRequester : MonoBehaviour
{
    void Start()
    {
        // Check if the camera permission is granted
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // Request camera permission
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
}