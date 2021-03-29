using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBoxController : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public event Action OnPointerClickEvent;
    public event Action<PointerEventData> OnDropEvent;

    
    ItemSlot _slot;
    GameObject _piece;

    private void Start() 
    {
        _slot = gameObject.GetComponentInParent<ItemSlot>();
        _piece = _slot.Piece;
    }

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke();

    public void OnDrop(PointerEventData eventData) => OnDropEvent?.Invoke(eventData);
        // if (_piece != null && eventData.pointerDrag != null)
        // {
        //     DragDrop puzzlepiece = eventData.pointerDrag.GetComponent<DragDrop>();
        //     if (_piece == eventData.pointerDrag)
        //     {
        //         puzzlepiece.DroppedOnSlot = true;  //Gibt dem ursprünglichen Objekt bekannt dass es erfolgreich gedropped wurde
        //         _piece = null;
        //         //ActivateSlot();
        //         print("1");
        //         StartCoroutine(_slot.FadeInSlot());
        //     } else
        //     {
        //         puzzlepiece.DroppedOnSlot = false;
        //     }
        // }
}