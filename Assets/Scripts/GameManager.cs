using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    // No need to make the entire GameManager a Singleton and persistent across scenes; properly managing the static variables works perfectly fine for now
    public static bool DraggingPuzzlePiece{get; private set;}
    public static bool GameInProgess{get; private set;}
    public static bool GameIsPaused{get => (Time.timeScale == 0);}
    
    [SerializeField] GameObject _firstMapCover;
    [SerializeField] MenuController _menuController;
    [SerializeField] CanvasGroup _puzzlePiecesCanvasGroup;

    [SerializeField] Texture2D _hoverCursorTexture = null;
    Texture2D _defaultCursorTexture;

    public static Camera MainCamera{get; private set;}

    void Awake() 
    {   
        MainCamera = Camera.main; // Cached to avoid expensive lookups during runtime
    }

    void Start() 
    {   
        _menuController.ToggleMenuVisibility(!GameInProgess);
        _puzzlePiecesCanvasGroup.alpha = GameInProgess?1:0;

        if (!_firstMapCover.activeSelf) // Safety check in case the first cover was disabled during development
            _firstMapCover.SetActive(true);
        if (GameInProgess) // If re-starting the game after the first game, the menu won't be active and the first cover should fade out right away
            _firstMapCover.GetComponent<MapCoverController>().FadeOut();
    }

    void OnEnable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent += PuzzlePieceController_PuzzlePieceDraggedEvent; //* CONSIDER: Move into CameraController
        _menuController.StartGameEvent += MenuController_StartGameEvent;
        _menuController.TogglePauseEvent += MenuController_TogglePauseEvent;
        _menuController.ChangeLocaleEvent += MenuController_ChangeLocaleVent;
        CursorChangeTrigger.CursorChangeEvent += CursorChangeTrigger_CursorChangeEvent;
    }

    void OnDisable() 
    {
        PuzzlePieceController.PuzzlePieceDraggedEvent -= PuzzlePieceController_PuzzlePieceDraggedEvent;
        _menuController.StartGameEvent -= MenuController_StartGameEvent;
        _menuController.TogglePauseEvent -= MenuController_TogglePauseEvent;
        CursorChangeTrigger.CursorChangeEvent -= CursorChangeTrigger_CursorChangeEvent;
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

        if (!paused) ChangeCursor(false);
    }

    void ChangeCursor(bool hoverCursor)
    {   
        if (_hoverCursorTexture != null)
        {
            var texture = hoverCursor?_hoverCursorTexture:_defaultCursorTexture;
            Cursor.SetCursor(texture, Vector2.zero,CursorMode.Auto);
        }
    }

    void WinGame()
    {
        
    }

    void PuzzlePieceController_PuzzlePieceDraggedEvent(bool dragging) => DraggingPuzzlePiece = dragging;
    
    void MenuController_TogglePauseEvent(bool paused) => TogglePause(paused);

    void MenuController_ChangeLocaleVent(int localeIdx)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIdx];
    }

    void MenuController_StartGameEvent() 
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
    }

    void CursorChangeTrigger_CursorChangeEvent(bool hoverCursor) => ChangeCursor(hoverCursor);
}