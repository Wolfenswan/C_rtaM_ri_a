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
    //public static event Action<GameObject> HintBoxToggled; // // ! Todo dedicated hintbox Controller that receives the string for the hint via event or method
    
    public bool DroppedInSlot;

    #region private fields
    [SerializeField] LocalizedString _localizedString;
    string _localizedHintText;
    Canvas _canvas;
    Vector3 _defaultPos;
    #endregion

    #region components
    CanvasGroup _canvasGroup;
    RectTransform _rectTransform;
    // GameObject _hintBox; // // ! TODO add dedicated HintBox Controller and move relevant methods there
    // CanvasGroup _hintBoxCanvasGroup;
    // TextMeshProUGUI _hintBoxTextField;
    #endregion

    private void Awake()
    {

        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _defaultPos = _rectTransform.anchoredPosition;
        _defaultPos.z = 0f;
        _canvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
        //defaultPos = rectTransform.position;

        //_hintText = transform.Find("HintText").GetComponent<TextMeshProUGUI>().text;
        // _hintBox = GameObject.FindGameObjectWithTag("HintBox").gameObject;
        // _hintBoxCanvasGroup = _hintBox.GetComponent<CanvasGroup>();
        // _hintBoxTextField = _hintBox.transform.Find("HintText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
    }
    
    private void OnEnable() 
    {
        //PuzzlePieceController.HintBoxToggled += DragDrop_HintBoxToggled; // // ! Lol. -> dedicated HintBox Controller
        _localizedString.StringChanged += UpdateLocalizedString;
    }

    private void OnDisable() 
    {
        //PuzzlePieceController.HintBoxToggled -= DragDrop_HintBoxToggled;
        _localizedString.StringChanged -= UpdateLocalizedString;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = .6f;
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;

        _rectTransform.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PuzzlePieceDraggedEvent?.Invoke(true);
        ToggleHintEvent?.Invoke(_localizedHintText);
        //ToggleHint(false);
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

    IEnumerator OnDroppedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        if (DroppedInSlot) // Set from within the Slot reacting to its own OnDropEvent
            gameObject.SetActive(false);
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

}
