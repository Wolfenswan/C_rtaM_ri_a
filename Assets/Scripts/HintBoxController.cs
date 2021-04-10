using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

public class HintBoxController : MonoBehaviour 
{   
    public bool IsVisible{get=>_canvasGroup.alpha==1;}
    public string CurrentText{get=>_localizedHintText;}

    [SerializeField] LocalizedString _localizedString;
    [SerializeField] TextMeshProUGUI _textField;
    [SerializeField] GameObject _textContainer;
    [SerializeField] GameObject _hintBoxButton;
    string _localizedHintText;
    CanvasGroup _canvasGroup;
    CanvasGroup _hintBoxButtonCanvasGroup;
    string _localizedButtonText;
    readonly float _hintBoxButtonDisabledAlpha = 0.7f;

    void Awake() 
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _hintBoxButtonCanvasGroup = _hintBoxButton.GetComponent<CanvasGroup>();
        //_hintBoxButton.SetActive(false);
        //_textField = transform.Find("HintText").GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        PuzzlePieceController.ToggleHintEvent += PuzzlePieceController_ToggleHintEvent;
        _localizedString.StringChanged += UpdateLocalizedString;
    }

    void OnDisable() 
    {
        PuzzlePieceController.ToggleHintEvent -= PuzzlePieceController_ToggleHintEvent;
        _localizedString.StringChanged -= UpdateLocalizedString;
    }

    void ToggleHintBoxVisibility(bool enable) 
    {
        //_canvasGroup.alpha = enable?1:0;
        _textContainer.SetActive(enable);

        if (enable && _textField.text != _localizedHintText || !enable)
            _hintBoxButtonCanvasGroup.alpha = 1;
        else if (enable && _textField.text == _localizedHintText)
            _hintBoxButtonCanvasGroup.alpha = _hintBoxButtonDisabledAlpha;
        //_hintBoxButton.SetActive(!enable);
    }

    void UpdateLocalizedString(string newString)
    {   
        // If the locale changes the HintBox will be forced to display the starter hint again (in the new language)
        _localizedHintText = newString;
        _textField.text = _localizedHintText;
        ToggleHintBoxVisibility(true);
    } 

    void PuzzlePieceController_ToggleHintEvent(string hintText, bool forceDisable = false) 
    {   
        if (forceDisable)
        {
            ToggleHintBoxVisibility(false);
        } else
        {
            // Disables the hint if it's currently active or enables (and shows) the hintBox if it's not visible
            var enableHint = !(_textField.text == hintText) || !_textContainer.activeSelf;
            if (enableHint)
                _textField.text = hintText;
            ToggleHintBoxVisibility(enableHint);
        }        
    }

    public void ButtonToggleHintBox()
    {   
        if (_textField.text != _localizedHintText || !_textContainer.activeSelf)
        {
            _textField.text = _localizedHintText;
            ToggleHintBoxVisibility(true);
        } else 
        {
            ToggleHintBoxVisibility(false);
        }
    }
    
}