using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapCoverController : MonoBehaviour
{
    public static event Action<GameObject> MapCoverRevealedEvent;
    public static event Action GameWonEvent;

    [SerializeField] CartaData _data;
    [SerializeField] int _maxZoomAfterReveal = 20;
    [SerializeField] List<GameObject> _piecesRequired;
    [SerializeField] List<GameObject> _piecesToReveal;
    [SerializeField] bool _finalCover;

    public int MaxZoom{get=>_maxZoomAfterReveal;}

    SpriteRenderer _spriteRenderer;
    bool _fading = false;

    void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() 
    {
        foreach (var piece in _piecesRequired)
        {
            piece.GetComponent<PuzzlePieceController>().PuzzlePieceSlottedEvent += PuzzlePiece_PuzzlePieceSlottedEvent;
        }
    }

    public void FadeOut()
    {
        if (!_fading)
        {
            _fading = true;
            StartCoroutine(FadeOutCover());
        }
    }

    void PuzzlePiece_PuzzlePieceSlottedEvent(GameObject piece)
    {
        var controller = piece.GetComponent<PuzzlePieceController>();
        controller.PuzzlePieceSlottedEvent -= PuzzlePiece_PuzzlePieceSlottedEvent;
        if (_piecesRequired.Count(obj => !obj.GetComponent<PuzzlePieceController>().DroppedInSlot) == 0)
        {
            if (!_finalCover)
                FadeOut();
            else
                GameWonEvent?.Invoke();
        }
            
    }

    IEnumerator FadeOutCover() // CONSIDER - write a generic utility DoOverTime-method accepting a Func<> or delegate
    {
        var elapsedTime = 0.0f;
        var color = _spriteRenderer.color;

        MapCoverRevealedEvent?.Invoke(gameObject);

        foreach (var item in _piecesToReveal)
        {   
            item?.SetActive(true);
        }
        while (elapsedTime < _data.CoverFadeOutTime)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => !GameManager.GameIsPaused);
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / _data.CoverFadeOutTime);
            _spriteRenderer.color = color;
        }

        gameObject.SetActive(false);
    }
}