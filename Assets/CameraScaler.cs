using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField] private static Camera _Camera;
    private static float _SmallestScreenWidth = 2688f;
    private static float _OriginalOrthoSize = 6f;

    // Start is called before the first frame update
    void Start()
    {
        _Camera = Camera.main;
        StaticUpdateOrtho();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Screen/UpdateOrthoSize %#o", false, 0)]
#endif
    static void StaticUpdateOrtho()
    {
        if (_Camera == null)
        {
            _Camera = Camera.main;
        }

        float width = Screen.width >= Screen.height ? Screen.width : Screen.height;
        _Camera.orthographicSize = (width / _SmallestScreenWidth) * _OriginalOrthoSize;
        Debug.Log("Setting ortho size (" + _Camera.orthographicSize + ") Found Width: " + width + "    Screen Width: " + Screen.width + "  Screen Height: " + Screen.height);
    }

    void Reset()
    {
        StaticUpdateOrtho();
    }
}
