using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class PuzzlePieceController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{  
    public static event Action<bool> PuzzlePieceDraggedEvent;
    public static event Action<string, bool> ToggleHintEvent;
    public event Action<GameObject> PuzzlePieceSlottedEvent;
    
    [HideInInspector] public bool DroppedInSlot;

    #region private fields
    [SerializeField] CartaData _data;
    [SerializeField] Canvas _puzzleCanvas;
    [SerializeField] LocalizedString _localizedString;
    string _localizedHintText;
    Vector3 _defaultPos;
    bool _visible = false;
    Color _defaultColor;
    Color _glowColor;
    #endregion

    #region components
    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;
    Image _image;
    #endregion

    void Awake()
    {

        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
        _glowColor = _data.PuzzlePieceGlowColor;
        _defaultColor = _image.color;
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

    void OnDisable() 
    {
        //PuzzlePieceController.HintBoxToggled -= DragDrop_HintBoxToggled;
        _localizedString.StringChanged -= UpdateLocalizedString;
    }
    void ToggleGlow(bool enable)
    {
        if (enable && _image.color != _glowColor)        
            _image.color = _glowColor;
        else if (!enable && _image.color != _defaultColor)
            _image.color = _defaultColor;            
    }
    
    void UpdateLocalizedString(string newString) => _localizedHintText = newString;
    
    #region onPointer methods
    public void OnPointerEnter(PointerEventData pointerEventData) => ToggleGlow(true);
    public void OnPointerExit(PointerEventData pointerEventData) => ToggleGlow(false);

    public void OnDrag(PointerEventData eventData)
    {
        ToggleGlow(true);
        _canvasGroup.alpha = .6f;
        _rectTransform.anchoredPosition += eventData.delta / _puzzleCanvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.transform.position = GameManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        PuzzlePieceDraggedEvent?.Invoke(true);
        //ToggleHintEvent?.Invoke(_localizedHintText); // TODO what hint box behaviour is desirable here: disable when dragging, enable when dragging or keep as is?
    }

    public void OnEndDrag(PointerEventData eventData)
    {   
        ToggleGlow(false);
        PuzzlePieceDraggedEvent?.Invoke(false);
        StartCoroutine(OnDroppedCoroutine());
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        ToggleHintEvent?.Invoke(_localizedHintText, false);
    }
    #endregion

    //public void FadeIn() => StartCoroutine(FadeInPiece());

    IEnumerator OnDroppedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !GameManager.GameIsPaused);
        if (DroppedInSlot) // Set from within the Slot reacting to its own OnDropEvent
        {   
            ToggleHintEvent?.Invoke(_localizedHintText, true);
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
