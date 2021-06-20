using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class HintBoxController : MonoBehaviour 
{   
    [SerializeField] LocalizedString _localizedHintString;
    [SerializeField] LocalizedString _localizedWonString;
    [SerializeField] TextMeshProUGUI _textField;
    [SerializeField] GameObject _textContainer;
    [SerializeField] GameObject _hintBoxButton;
    string _currentLocalizedText;
    CanvasGroup _canvasGroup;
    CanvasGroup _hintBoxButtonCanvasGroup;
    readonly float _hintBoxButtonDisabledAlpha = 0.7f;

    void Awake() 
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _hintBoxButtonCanvasGroup = _hintBoxButton.GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        PuzzlePieceController.ToggleHintEvent += PuzzlePieceController_ToggleHintEvent;
        MapCoverController.GameWonEvent += MapCoverController_GameWonEvent;
        _localizedHintString.StringChanged += UpdateLocalizedString;
    }

    void OnDisable() 
    {
        PuzzlePieceController.ToggleHintEvent -= PuzzlePieceController_ToggleHintEvent;
        MapCoverController.GameWonEvent -= MapCoverController_GameWonEvent;
        _localizedHintString.StringChanged -= UpdateLocalizedString;
    }

    void ToggleHintBoxVisibility(bool enable) 
    {
        _textContainer.SetActive(enable);

        if (enable && _textField.text != _currentLocalizedText) // Keep showing the hint canvas if only the hint text was changed
            _hintBoxButtonCanvasGroup.alpha = 1;
        else if (enable && _textField.text == _currentLocalizedText)
            _hintBoxButtonCanvasGroup.alpha = _hintBoxButtonDisabledAlpha;
    }

    void UpdateLocalizedString(string newString)
    {   
        _currentLocalizedText = newString;
        _textField.text = _currentLocalizedText;
        ToggleHintBoxVisibility(true);  // If the locale changes the HintBox will be forced to display the starter hint again (in the new language)
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

    void MapCoverController_GameWonEvent()
    {
        _textField.text = _localizedWonString.GetLocalizedString();
        ToggleHintBoxVisibility(true);
    }

    public void ButtonToggleHintBox()
    {   
        if (_textField.text != _currentLocalizedText || !_textContainer.activeSelf)
        {
            _textField.text = _currentLocalizedText;
            ToggleHintBoxVisibility(true);
        } else 
        {
            ToggleHintBoxVisibility(false);
        }
    }
    
}