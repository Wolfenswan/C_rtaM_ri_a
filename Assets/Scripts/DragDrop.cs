using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

//! TODO Rename to XYController for consistency (DragDrop* or Piece*)
public class DragDrop : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public static event Action<bool> PuzzlePieceDraggedEvent;
    public static event Action<GameObject> HintBoxToggled; // ! Todo dedicated hintbox Controller that receives the string for the hint via event or method
    public bool DroppedOnSlot;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 defaultPos;
    
    string _hintText;
    GameObject _hintBox; // ! TODO add dedicated HintBox Controller and move relevant methods there
    CanvasGroup _hintBoxCanvasGroup;
    TextMeshProUGUI _hintBoxTextField;

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        defaultPos = rectTransform.anchoredPosition;
        defaultPos.z = 0f;
        canvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
        //defaultPos = rectTransform.position;

        
        _hintText = transform.Find("HintText").GetComponent<TextMeshProUGUI>().text;
        _hintBox = GameObject.FindGameObjectWithTag("HintBox").gameObject;
        _hintBoxCanvasGroup = _hintBox.GetComponent<CanvasGroup>();
        _hintBoxTextField = _hintBox.transform.Find("HintText").GetComponent<TextMeshProUGUI>();
        //_hintBox.SetActive(false);
        //ToggleHint(false);
    }

    private void Start() {
    }
    
    private void OnEnable() 
    {
        DragDrop.HintBoxToggled += DragDrop_HintBoxToggled; // ! Lol. -> dedicated HintBox Controller
    }

    private void OnDisable() 
    {
        DragDrop.HintBoxToggled -= DragDrop_HintBoxToggled;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("Drag");
        canvasGroup.alpha = .6f;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        rectTransform.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PuzzlePieceDraggedEvent?.Invoke(true);
        ToggleHint(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(OnDroppedCoroutine());
        PuzzlePieceDraggedEvent?.Invoke(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        var enableHint = !(_hintBoxTextField.text == _hintText) || (int) _hintBoxCanvasGroup.alpha == 0;
        ToggleHint(enableHint);
        //_hintBox.SetActive(!_hintBox.activeSelf);
        HintBoxToggled?.Invoke(_hintBox);
    }

    void ToggleHint(bool enable)
    {
        if (enable)
        {
            _hintBoxCanvasGroup.alpha = 1;
            _hintBoxTextField.text = _hintText;
        } else
        {
            _hintBoxCanvasGroup.alpha = 0;
        }
    }

    void DragDrop_HintBoxToggled(GameObject otherHintBox)
    {
        if (otherHintBox != _hintBox && _hintBox.activeSelf)
            ToggleHint(false);
            //_hintBox.SetActive(false);
    }

    IEnumerator OnDroppedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        if (DroppedOnSlot == true)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            rectTransform.anchoredPosition = defaultPos;
            
            // Dragging sometimes does funny things by setting z level at -400. This hopefully fixes that
            if (rectTransform.localPosition.z != 0f)
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x,rectTransform.localPosition.y,0f);
        }

    }

}
