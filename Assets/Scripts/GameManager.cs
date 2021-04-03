using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool DraggingPuzzlePiece{get; private set;} //* CONSIDER: Better here or directly in CameraController?
    public static bool GameInProgess{get; private set;}
    public static bool GameIsPaused{get => (Time.timeScale == 0);}
    
    [SerializeField] GameObject _firstMapCover;
    [SerializeField] MenuController _menuController;
    [SerializeField] CanvasGroup _puzzlePiecesCanvasGroup;
    //public static Camera MainCamera{get; private set;}

    void Awake() 
    {   
        //_menuController = GameObject.FindGameObjectWithTag("UIMainMenu").GetComponent<MenuController>();
        //_puzzleUICanvasGroup = GameObject.FindGameObjectWithTag("UIPuzzlePieces").GetComponent<CanvasGroup>();
        //MainCamera = Camera.main; // Cached to avoid expensive lookups during runtime
    }

    void Start() 
    {   
        _menuController.ToggleMenuVisibility(!GameInProgess);
        _puzzlePiecesCanvasGroup.alpha = GameInProgess?1:0;
        
        if (!_firstMapCover.activeSelf)
            _firstMapCover.SetActive(true);
        if (GameInProgess)
            _firstMapCover.GetComponent<MapCoverController>().FadeOut();
    }

    void OnEnable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent += PuzzlePieceController_PuzzlePieceDraggedEvent; //* CONSIDER: Move into CameraController
        _menuController.StartGameEvent += Menu_StartGameEvent;
        _menuController.TogglePauseEvent += Menu_TogglePauseEvent;
    }

    void OnDisable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent -= PuzzlePieceController_PuzzlePieceDraggedEvent;
        _menuController.StartGameEvent -= Menu_StartGameEvent;
        _menuController.TogglePauseEvent -= Menu_TogglePauseEvent;
    }

    void Update() 
    {
        if (Input.GetKeyDown("escape") && GameInProgess && !DraggingPuzzlePiece)
            TogglePause(!GameIsPaused);
    }

    void TogglePause(bool paused)
    {   
        Time.timeScale = paused?0:1;
        _menuController.ToggleMenuVisibility(paused);
        _puzzlePiecesCanvasGroup.alpha = paused?0:1;
    }

    void PuzzlePieceController_PuzzlePieceDraggedEvent(bool dragging) => DraggingPuzzlePiece = dragging;
    
    void Menu_TogglePauseEvent(bool paused) => TogglePause(paused);

    void Menu_StartGameEvent() 
    {   
        TogglePause(false);

        if (GameInProgess)
        {   
            SceneManager.LoadScene(0); // Restart the scene but keep GameInProgress set to true to hide the menu on load
        } else 
        {   
            // No need to re-load the scene on the first menu screen
            GameInProgess = true;
            _firstMapCover.GetComponent<MapCoverController>().FadeOut();
        }
        // else if (BackToMainMenu)
        // SceneManager.LoadScene(0); + GameInProgess = false;
    }
}