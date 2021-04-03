using System.Collections;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

//! TODO Rename to XYController for consistency (DragDrop* or Piece*)
public class PuzzlePieceController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{  
    public static event Action<bool> PuzzlePieceDraggedEvent;
    public static event Action<string> ToggleHintEvent;
    public event Action<GameObject> PuzzlePieceSlottedEvent;
    //public static event Action<GameObject> HintBoxToggled; // // ! Todo dedicated hintbox Controller that receives the string for the hint via event or method
    
    [HideInInspector] public bool DroppedInSlot;

    #region private fields
    [SerializeField] CartaData _data;
    [SerializeField] Canvas _puzzleCanvas;
    [SerializeField] LocalizedString _localizedString;
    string _localizedHintText;
    Vector3 _defaultPos;
    bool _visible = false;
    #endregion

    #region components
    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;
    #endregion

    void Awake()
    {

        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _defaultPos = _rectTransform.anchoredPosition;
        _defaultPos.z = 0f;
    }
    
    void OnEnable() 
    {
        //PuzzlePieceController.HintBoxToggled += DragDrop_HintBoxToggled; // // ! Lol. -> dedicated HintBox Controller
        _localizedString.StringChanged += UpdateLocalizedString;
        if(!_visible)
            StartCoroutine(FadeInPiece());
    }

    void Start() 
    {
        
    }

    void OnDisable() 
    {
        //PuzzlePieceController.HintBoxToggled -= DragDrop_HintBoxToggled;
        _localizedString.StringChanged -= UpdateLocalizedString;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = .6f;
        _rectTransform.anchoredPosition += eventData.delta / _puzzleCanvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;

        _rectTransform.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PuzzlePieceDraggedEvent?.Invoke(true);
        //ToggleHintEvent?.Invoke(_localizedHintText); // TODO what hint box behaviour is desirable here: disable when dragging, enable when dragging or keep as is?
    }

    public void OnEndDrag(PointerEventData eventData)
    {   
        PuzzlePieceDraggedEvent?.Invoke(false);
        StartCoroutine(OnDroppedCoroutine());
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        //var enableHint = !(_hintBoxTextField.text == _hintText) || (int) _hintBoxCanvasGroup.alpha == 0;
        //var enableHint = !(_hintBoxTextField.text == _localizedHintText) || (int) _hintBoxCanvasGroup.alpha == 0;
        //ToggleHint(enableHint);
        ToggleHintEvent?.Invoke(_localizedHintText);
        //HintBoxToggled?.Invoke(_hintBox); // // TODO rewrite this into own HintBoxController
    }

    // void ToggleHint(bool enable) // // TODO rewrite this into own HintBoxController
    // {
    //     if (enable)
    //     {   
    //         _hintBoxCanvasGroup.alpha = 1;
    //         _hintBoxTextField.text = _localizedHintText;
    //         //_hintBoxTextField.text = _hintText;
    //     } else
    //     {
    //         _hintBoxCanvasGroup.alpha = 0;
    //     }
    // }

    // void DragDrop_HintBoxToggled(GameObject otherHintBox)
    // {
    //     if (otherHintBox != _hintBox && _hintBox.activeSelf)
    //         ToggleHint(false);
    //         //_hintBox.SetActive(false);
    // }

    void UpdateLocalizedString(string newString) => _localizedHintText = newString;

    public void FadeIn() => StartCoroutine(FadeInPiece());

    IEnumerator OnDroppedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !GameManager.GameIsPaused);
        if (DroppedInSlot) // Set from within the Slot reacting to its own OnDropEvent
        {
            PuzzlePieceSlottedEvent?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
        else  // If the puzzle piece was not dropped on the correct slot reset it
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.anchoredPosition = _defaultPos;
            // Dragging sometimes does funny things by setting z level at -400. This hopefully fixes that
            if (_rectTransform.localPosition.z != 0f)
                _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x,_rectTransform.localPosition.y,0f);
        }

    }

    IEnumerator FadeInPiece() // CONSIDER write Extension for _canvasGroup
    {
        var elapsedTime = 0.0f;
        _canvasGroup.alpha = 0f;
        while (elapsedTime < _data.CoverFadeOutTime)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => !GameManager.GameIsPaused);
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / _data.CoverFadeOutTime);
        }
        _visible = true;
    }

}
