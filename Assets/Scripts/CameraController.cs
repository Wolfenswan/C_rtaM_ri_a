using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using NEINGames.Utilities;

public class CameraController : MonoBehaviour
{
    [SerializeField] CartaData _data;
    //[SerializeField] HintBoxController _hintBox;

    Camera _cam;
    HanseGameJam.RangeInt _zoomClamp;
    Vector3 _dragStartingPoint;

    void Awake() 
    {
        _cam = GetComponent<Camera>();
        _zoomClamp = _data.MouseZoomClampRange;
    }

    void OnEnable() 
    {
       MapCoverController.MapCoverRevealedEvent += MapCoverController_MapCoverRevealedEvent;
    }

    void OnDisable() 
    {
        MapCoverController.MapCoverRevealedEvent -= MapCoverController_MapCoverRevealedEvent;
    }

    void Update()
    {   

        if (!GameManager.GameIsPaused)
            CameraMovement();
    }

    void CameraMovement()
    {
        var zoom = Input.mouseScrollDelta.y;
        var cursorPos = Input.mousePosition;
        var cursorWorldPos = _cam.ScreenToWorldPoint(cursorPos);
        var buttonDown = Input.GetMouseButtonDown(0); //* CONSIDER - also allow middle/right mouse?
        var buttonHeld = Input.GetMouseButton(0);

        if (zoom != 0)
        {   
            var targetOrthoSize = Mathf.Clamp(_cam.orthographicSize + _data.MouseZoomSpeed * (-zoom), _zoomClamp.Min, _zoomClamp.Max);
            // Change camera position to cursor if we're zooming in
            // First check makes sure the camera wont move if min zoom value has been reached
            if (targetOrthoSize != _cam.orthographicSize && zoom > 0) MoveCameraToZoom(cursorWorldPos, zoom);
            _cam.orthographicSize = targetOrthoSize;
        }
        if (buttonDown || buttonHeld && GameManager.GameInProgess && !GameManager.DraggingPuzzlePiece && !RaycastUtilities.IsPoint2DOverElementWithTag(cursorPos, "UIPuzzlePanel")) // IsPointerOverUIElement(cursorPos)
        {
            PanCamera(buttonDown, buttonHeld, cursorWorldPos);
            //if (_hintBox.IsVisible) _hintBox.ToggleHintBoxVisibility(false); // TODO test if this is actually desireable
        }
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
    void MapCoverController_MapCoverRevealedEvent(GameObject MapCoverObject)
    {   
        // Update the max zoom range according to the revealed map cover
        _zoomClamp.Max = MapCoverObject.GetComponent<MapCoverController>().MaxZoom;
    }
}