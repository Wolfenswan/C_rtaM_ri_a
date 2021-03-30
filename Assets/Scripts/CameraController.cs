using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] CartaData _data;

    Camera _cam;
    HanseGameJam.RangeInt _zoomClamp;
    Vector3 _dragStartingPoint;

    void Awake() 
    {
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        _zoomClamp = _data.MouseZoomClampRange;
    }

    void Update()
    {   

        var zoom = Input.mouseScrollDelta.y;
        var cursorPos = Input.mousePosition;
        var cursorWorldPos = _cam.ScreenToWorldPoint(cursorPos);

        var buttonDown = Input.GetMouseButtonDown(0); // CONSIDER - also allow middle/right mouse?
        var buttonHeld = Input.GetMouseButton(0);

        if (zoom != 0)
        {   
            var targetOrthoSize = Mathf.Clamp(_cam.orthographicSize + _data.MouseZoomSpeed * (-zoom), _zoomClamp.Min, _zoomClamp.Max);

            // Change camera position to cursor if we're zooming in
            // First check makes sure the camera wont move if min zoom value has been reached
            if (targetOrthoSize != _cam.orthographicSize && zoom > 0) MoveCameraToZoom(cursorWorldPos, zoom);

            // Update the zoom
            _cam.orthographicSize = targetOrthoSize;
        }

        // Panning under specific conditions
        if (buttonDown || buttonHeld && !IsPointerOverUIElement(cursorPos) && !GameManager.DraggingPuzzlePiece)
            PanCamera(buttonDown, buttonHeld, cursorWorldPos);
    }

    void MoveCameraToZoom(Vector3 cursorWorldPos, float zoom)
    {
        var scaler = (1.0f / _cam.orthographicSize * zoom);
        transform.position += (cursorWorldPos - transform.position) * scaler;
    }

    void PanCamera(bool buttonDown, bool buttonHeld, Vector3 cursorWorldPos)
    {     
        if (buttonDown)
            _dragStartingPoint= cursorWorldPos;

        if (buttonHeld)
        {
            Vector3 difference = _dragStartingPoint - cursorWorldPos;
            difference.z = 0f;
            transform.position += difference;
        }
    }

    bool IsPointerOverUIElement(Vector2 cursorPos) //* CONSIDER might make a nice extension or utility function; passing array/list of strings?
    // http://answers.unity.com/answers/1748972/view.html
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = cursorPos; //! TODO use cursorPos as argument
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count(obj => obj.gameObject.tag == "UIElement") > 0;
    }
}