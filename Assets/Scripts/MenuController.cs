using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MenuController : MonoBehaviour 
{   
    public event Action StartGameEvent;
    public event Action<bool> TogglePauseEvent;

    public bool MenuIsVisible{get => _menu.activeSelf;}

    [SerializeField] GameObject _menu;

    GameObject _pausebutton;

    void Awake() 
    {
        _pausebutton = _menu.transform.Find("Buttons").Find("ButtonContinue").gameObject;
        _pausebutton.SetActive(false);
    }

    public void ToggleMenuVisibility(bool show)
    {
        if (show && !_pausebutton.activeSelf && GameManager.GameIsPaused)
            _pausebutton.SetActive(true);
        _menu.SetActive(show);
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
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idx];
    }
 
    public void ButtonExit()
    {
        Application.Quit();
    }
    
}