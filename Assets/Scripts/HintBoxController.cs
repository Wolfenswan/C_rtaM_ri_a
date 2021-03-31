using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class HintBoxController : MonoBehaviour 
{   
    public bool IsVisible{get=>_canvasGroup.alpha==1;}    
    public bool IsActive{get;private set;}

    [SerializeField] LocalizedString _localizedString;
    string _localizedHintText;

    TextMeshProUGUI _textField;
    CanvasGroup _canvasGroup;

    void Awake() 
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _textField = transform.Find("HintText").GetComponent<TextMeshProUGUI>();
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

    public void ToggleHintBoxVisibility(bool enable) => _canvasGroup.alpha = enable?1:0;

    void ToggleHint(string hintText, bool forced=false)
    {
        var enableHint = forced || !(_textField.text == hintText) || (int) _canvasGroup.alpha == 0;
        if (enableHint)
        {   
            _textField.text = hintText;
        }
        ToggleHintBoxVisibility(enableHint);
    }

    void UpdateLocalizedString(string newString)
    {   
        // If the locale changes the HintBox will be forced to display the starter hint again (in the new language)
        _localizedHintText = newString;
        ToggleHint(_localizedHintText, true);
    } 

    void PuzzlePieceController_ToggleHintEvent(string hintText) => ToggleHint(hintText);
}