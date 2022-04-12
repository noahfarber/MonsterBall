using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    private Camera _Camera;
    private float _SmallestScreenWidth = 900f;
    private float _OriginalOrthoSize = 6f;

    // Start is called before the first frame update
    void Start()
    {
        _Camera = GetComponent<Camera>();
        float width = Screen.width >= Screen.height ? Screen.width : Screen.height;
        _Camera.orthographicSize = (width / _SmallestScreenWidth) * _OriginalOrthoSize;

        Debug.Log("Setting ortho size (" + _Camera.orthographicSize + ") Found Width: " + width + "    Screen Width: " + Screen.width + "  Screen Height: " + Screen.height);
    }
}
