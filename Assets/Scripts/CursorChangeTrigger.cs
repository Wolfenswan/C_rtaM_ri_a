 using System;
 using UnityEngine;
 using UnityEngine.EventSystems;

public class CursorChangeTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<bool> CursorChangeEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorChangeEvent?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorChangeEvent?.Invoke(false);
    }
}