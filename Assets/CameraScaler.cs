using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    private Camera _Camera;
    private float _SmallestScreenWidth = 2208f;
    private float _OriginalOrthoSize = 5.4f;

    // Start is called before the first frame update
    void Start()
    {
        _Camera = GetComponent<Camera>();
        _Camera.orthographicSize = (Screen.width / _SmallestScreenWidth) * _OriginalOrthoSize;
    }
}
