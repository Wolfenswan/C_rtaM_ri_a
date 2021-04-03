using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class HintBoxController : MonoBehaviour 
{   
    public bool IsVisible{get=>_canvasGroup.alpha==1;}

    [SerializeField] LocalizedString _localizedString;
    [SerializeField] TextMeshProUGUI _textField;
    string _localizedHintText;
    CanvasGroup _canvasGroup;

    void Awake() 
    {
        _canvasGroup = GetComponent<CanvasGroup>();
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

    void ToggleHintBoxVisibility(bool enable) => _canvasGroup.alpha = enable?1:0;

    void UpdateLocalizedString(string newString)
    {   
        // If the locale changes the HintBox will be forced to display the starter hint again (in the new language)
        _localizedHintText = newString;
        _textField.text = _localizedHintText;
        ToggleHintBoxVisibility(true);
    } 

    void PuzzlePieceController_ToggleHintEvent(string hintText) 
    {   
        // Disables the hint if it's currently active or enables (and shows) the hintBox if it's not visible
        var enableHint = !(_textField.text == hintText) || (int) _canvasGroup.alpha == 0;
        if (enableHint)
            _textField.text = hintText;
        ToggleHintBoxVisibility(enableHint);
    }
}