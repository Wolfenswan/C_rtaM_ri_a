using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBoxController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    // The entire point of the controller is to provide a trigger-area independent of the sprite size used in the ImageComponent of PuzzleSlotController
    
    public event Action OnPointerClickEvent;
    public event Action<PointerEventData> OnDropEvent;

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke();

    public void OnDrop(PointerEventData eventData) => OnDropEvent?.Invoke(eventData);
}