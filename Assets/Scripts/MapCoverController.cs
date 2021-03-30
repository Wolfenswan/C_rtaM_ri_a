using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class MapCoverController : MonoBehaviour
{
    [SerializeField] CartaData _data;
    [SerializeField] List<GameObject> _piecesRequired;
    [SerializeField] List<GameObject> _piecesToReveal;

    SpriteRenderer _spriteRenderer;

    void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() 
    {   
        //* CONSIDER: Probably more performant to use an event listener instead of listening each frame
        if (_piecesRequired.Count(obj => obj.activeSelf) == 0)
            StartCoroutine(FadeOutCover());
    }

    IEnumerator FadeOutCover() //* CONSIDER - write a generic utility DoOverTime-method accepting a Func<> or delegate
    {
        var elapsedTime = 0.0f;
        var color = _spriteRenderer.color;
        while (elapsedTime < _data.CoverFadeOutTime && color.a > 0)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            color.a -= Mathf.Clamp01(elapsedTime / _data.CoverFadeOutTime);
            _spriteRenderer.color = color;
        }

        foreach (var item in _piecesToReveal)
        {   
            item?.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}