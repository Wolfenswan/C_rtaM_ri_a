using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleSlotController : MonoBehaviour//, IDropHandler
{
    [SerializeField] bool _activated; // Serialized only for debugging purposes
    [SerializeField] GameObject _assignedPiece;
    [SerializeField] CartaData _data;
    [SerializeField] float _defaultAlpha = 0.1f; //* REMINDER - once we modify alpha through a difficulty setting, add an override

    #region fields
    [SerializeField] ClickBoxController _clickBox;
    #endregion
    
    #region components
    CanvasGroup _cGroup;
    Animator _animator;
    AudioSource _audioSource;
    #endregion

    readonly int _animHash = Animator.StringToHash("Animation"); // Bit hacky: As there's only one animation for each piece, they've all been named "Animation" to make things faster to implement during the Game Jam. Now it just works.

    void Awake()
    {   
        _cGroup = GetComponent<CanvasGroup>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        //_clickBox = transform.Find("ClickBox").GetComponent<ClickBoxController>();  
        _cGroup.alpha = _defaultAlpha;
    }

    void Start() 
    {   
        if (_activated) _cGroup.alpha = 1; // Only for debugging purposes as by default a slot should not be activated on Start
    }

    private void OnEnable() 
    {
        _clickBox.OnDropEvent += ClickBox_OnDropEvent;
        if (_activated) _clickBox.OnPointerClickEvent += ClickBox_OnPointerClickEvent; // Debugging only, if the slot has been forced active
    }

    private void OnDisable() 
    {
        _clickBox.OnDropEvent -= ClickBox_OnDropEvent;
        _clickBox.OnPointerClickEvent -= ClickBox_OnPointerClickEvent; // Subscription takes place at the end of FadeInSlot()
    }

    void ActivateSlot()
    {   
        _cGroup.alpha = 1;
        Play();
        _activated = true;
    }

    void Play()
    {   
        _animator.Play(_animHash);
        if (!_audioSource.isPlaying) _audioSource.Play();
    }

    IEnumerator FadeInSlot()
    {
        var elapsedTime = 0.0f;
        while (elapsedTime < _data.SlotFadeInTime)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => !GameManager.GameIsPaused);
            elapsedTime += Time.deltaTime;
            _cGroup.alpha = Mathf.Clamp01(elapsedTime / _data.SlotFadeInTime);
        }
        Play();
        _activated = true;
        _clickBox.OnPointerClickEvent += ClickBox_OnPointerClickEvent;
        _clickBox.gameObject.AddComponent<CursorChangeTrigger>();
    }

    void ClickBox_OnPointerClickEvent()
    {
        if (_activated)
            Play();
    }

    void ClickBox_OnDropEvent(PointerEventData eventData)
    {
        if (_assignedPiece != null && eventData.pointerDrag != null && _cGroup.alpha == _defaultAlpha)
        {
            PuzzlePieceController puzzlepiece = eventData.pointerDrag.GetComponent<PuzzlePieceController>();
            if (_assignedPiece == eventData.pointerDrag)
            {   
                puzzlepiece.DroppedInSlot = true; // Alert the source piece that it has been dropped in its designated slot (it will then be disabled)
                _assignedPiece = null;
                StartCoroutine(FadeInSlot());
            }
            // } else
            // {
            //     puzzlepiece.DroppedOnSlot = false; //* CONSIDER unnecessary?
            // }
        }
    }
}
