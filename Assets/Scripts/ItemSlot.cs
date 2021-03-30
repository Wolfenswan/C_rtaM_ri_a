using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

//! TODO Rename to SlotController for consistency
public class ItemSlot : MonoBehaviour//, IDropHandler
{
    [SerializeField] bool _activated;
    [SerializeField] GameObject _assignedPiece;
    [SerializeField] CartaData _data;
    [SerializeField] float _defaultAlpha = 0.1f; //* REMINDER - once we modify alpha through a difficulty setting, add an override

    CanvasGroup _cGroup;
    Animator _animator;
    AudioSource _audioSource;
    Image _image;
    Camera _cam;
    ClickBoxController _clickBox;

    //public GameObject Piece{get; private set;}

    int _animHash = Animator.StringToHash("Animation");

    void Awake()
    {   
        _cGroup = GetComponent<CanvasGroup>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _image = GetComponent<Image>();
        _clickBox = transform.Find("ClickBox").GetComponent<ClickBoxController>();
        _cGroup.alpha = _defaultAlpha;
        _cam = Camera.main; //! TODO refer to GameManager or assign via SerializeField
    }

    void Start() 
    {
        if (_activated) _cGroup.alpha = 1;
    }

    private void OnEnable() 
    {
        _clickBox.OnDropEvent += ClickBox_OnDropEvent;
    }

    private void OnDisable() 
    {
        _clickBox.OnDropEvent -= ClickBox_OnDropEvent;
        _clickBox.OnPointerClickEvent -= ClickBox_OnPointerClickEvent;
    }

    // public void OnDrop(PointerEventData eventData)
    // {
    //     if (_piece != null && eventData.pointerDrag != null)
    //     {
    //         DragDrop puzzlepiece = eventData.pointerDrag.GetComponent<DragDrop>();
    //         if (_piece == eventData.pointerDrag)
    //         {
    //             puzzlepiece.DroppedOnSlot = true;  //Gibt dem ursprünglichen Objekt bekannt dass es erfolgreich gedropped wurde
    //             _piece = null;
    //             //ActivateSlot();
    //             StartCoroutine(FadeInSlot());
    //         } else
    //         {
    //             puzzlepiece.DroppedOnSlot = false;
    //         }
    //     }
    // }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     // var test = eventData.pointerCurrentRaycast;
    //     // var cursorPos = Input.mousePosition;
    //     // var cursorWorldPos = _cam.ScreenToWorldPoint(cursorPos);
    //     // print(eventData.pointerPressRaycast);
    //     if (_activated)
    //         Play();
    // }

    void ActivateSlot()
    {   
        _cGroup.alpha = 1;
        Play();
        _activated = true;

    }

    void Play()
    {   
        _animator.Play(_animHash);
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    void ClickBox_OnPointerClickEvent()
    {
        if (_activated)
            Play();
    }

    void ClickBox_OnDropEvent(PointerEventData eventData)
    {
        if (_assignedPiece != null && eventData.pointerDrag != null)
        {
            DragDrop puzzlepiece = eventData.pointerDrag.GetComponent<DragDrop>();
            if (_assignedPiece == eventData.pointerDrag)
            {
                puzzlepiece.DroppedOnSlot = true;  //Gibt dem ursprünglichen Objekt bekannt dass es erfolgreich gedropped wurde
                _assignedPiece = null;
                //ActivateSlot();
                StartCoroutine(FadeInSlot());
            } else
            {
                puzzlepiece.DroppedOnSlot = false;
            }
        }
    }

    IEnumerator FadeInSlot()
    {
        var elapsedTime = 0.0f;
        while (elapsedTime < _data.SlotFadeInTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            _cGroup.alpha = Mathf.Clamp01(elapsedTime / _data.SlotFadeInTime);
        }
        Play();
        _clickBox.OnPointerClickEvent += ClickBox_OnPointerClickEvent;
        _activated = true;
    }
}
