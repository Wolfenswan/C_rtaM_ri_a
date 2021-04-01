using UnityEngine;

public class MainMenuController : MonoBehaviour 
{
    public void ButtonExit()
    {
        Application.Quit();
    }
    
    public void ButtonGo()
    {
        gameObject.SetActive(false); // Später: Anderes UI aktivieren (evtl event an GameManager)
    }
}