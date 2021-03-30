using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBoxController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public event Action OnPointerClickEvent;
    public event Action<PointerEventData> OnDropEvent;

    // ItemSlot _slot;
    // GameObject _piece;

    // private void Start() 
    // {
    //     _slot = gameObject.GetComponentInParent<ItemSlot>();
    //     _piece = _slot.Piece;
    // }

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke();

    public void OnDrop(PointerEventData eventData) => OnDropEvent?.Invoke(eventData);
}