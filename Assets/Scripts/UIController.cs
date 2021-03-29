using UnityEngine;

public class UIController : MonoBehaviour 
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