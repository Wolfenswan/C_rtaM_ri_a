using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MenuController : MonoBehaviour 
{   
    public event Action StartGameEvent;
    public event Action<int> ChangeLocaleEvent;
    public event Action<bool> TogglePauseEvent;

    public bool MenuIsVisible{get => _menuCanvas.activeSelf;}

    [SerializeField] GameObject _menuCanvas;
    [SerializeField] GameObject _continueButton;

    void Awake() 
    {
        //_pausebutton = _menu.transform.Find("Buttons").Find("ButtonContinue").gameObject; //* Ugly. Improve when time
        _continueButton.SetActive(false);
    }

    public void ToggleMenuVisibility(bool show)
    {
        if (show && !_continueButton.activeSelf && GameManager.GameIsPaused)
            _continueButton.SetActive(true);
        _menuCanvas.SetActive(show);
    }

    public void ButtonNewGame()
    {
        StartGameEvent?.Invoke();
    }
    
    public void ButtonContinue()
    {   
        TogglePauseEvent?.Invoke(false);
    }

    public void ButtonLanguage()
    {   
        // This is very lazy but perfectly fine for now. The manual has an example for a fancier dropdown menu; a better solution should there ever be >2 language.
        // Alternatively: Implement two+ dedicated buttons with images (maybe stylized shields from the map?)
        var idx = LocalizationSettings.SelectedLocale.Identifier == LocalizationSettings.AvailableLocales.Locales[0].Identifier?1:0;
        ChangeLocaleEvent?.Invoke(idx);
    }
 
    public void ButtonExit()
    {
        Application.Quit();
    }
    
    public void ButtonLanguageSelect(int localeIdx)
    {   
        ChangeLocaleEvent?.Invoke(localeIdx);
    }
    
}