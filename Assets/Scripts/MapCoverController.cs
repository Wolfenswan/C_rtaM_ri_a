using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapCoverController : MonoBehaviour
{
    [SerializeField] CartaData _data;
    [SerializeField] List<GameObject> _piecesRequired;
    [SerializeField] List<GameObject> _piecesToReveal;

    void Update() 
    {   
        if (_piecesRequired.Count(obj => obj.activeSelf) == 0)
        {
            StartCoroutine(FadeOutCover());
        }
    }

    IEnumerator FadeOutCover() // CONSIDER - write a generic utility DoOverTime-method accepting a Func<> or delegate
    {
        var elapsedTime = 0.0f;
        var color = GetComponent<SpriteRenderer>().color;
        while (elapsedTime < _data.CoverFadeOutTime && color.a > 0)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            color.a -= Mathf.Clamp01(elapsedTime / _data.CoverFadeOutTime);
            GetComponent<SpriteRenderer>().color = color;
        }

        foreach (var item in _piecesToReveal)
        {   
            item?.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}