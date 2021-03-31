using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool DraggingPuzzlePiece{get; private set;} = false; //* CONSIDER: Better here or directly in CameraController?
    
    [SerializeField] GameObject _firstMapCover;
    //public static Camera MainCamera{get; private set;}

    void Awake() 
    {
        //MainCamera = Camera.main; // Cached to avoid expensive lookups during runtime
    }

    void Start() 
    {   
        // Safety in case the first cover was disabled for testing purposes and then forgotten to be re-enabled
        if (!_firstMapCover.activeSelf)
            _firstMapCover.SetActive(true);
    }

    void OnEnable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent += OnDraggingPieceEvent; //* CONSIDER: Move into CameraController
    }

    void OnDisable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent -= OnDraggingPieceEvent;
    }

    void Update() 
    {
        if (Input.GetKeyDown("escape"))
            Application.Quit();
            // TODO later: activate main menu instead, deactivate other UI elements
    }

    void OnDraggingPieceEvent(bool dragging) => DraggingPuzzlePiece = dragging;
}