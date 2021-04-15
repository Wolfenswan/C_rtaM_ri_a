using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickBoxController : MonoBehaviour, IPointerClickHandler, IDropHandler//, IPointerEnterHandler, IPointerExitHandler
{
    // The entire point of the controller is to provide a trigger-area independent of the sprite size used in the ImageComponent of PuzzleSlotController
    
    public event Action OnPointerClickEvent;
    public event Action<PointerEventData> OnDropEvent;

    // Image _image;
    // Color _defaultColor;
    // Color _glowColor = new Color32(255,255,255,1);

    // void Awake() 
    // {
    //     _image = gameObject.GetComponentInParent<Image>();
    //     _defaultColor = _image.color;
    // }

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke();
    public void OnDrop(PointerEventData eventData) => OnDropEvent?.Invoke(eventData);

    // public void OnPointerEnter(PointerEventData pointerEventData) => ToggleGlow(true);
    // public void OnPointerExit(PointerEventData pointerEventData) => ToggleGlow(false);

    // void ToggleGlow(bool enable)
    // {
    //     print("glow // " + _image);
    //     if (enable && _image.color != _glowColor)        
    //         _image.color = _glowColor;
    //     else if (!enable && _image.color != _defaultColor)
    //         _image.color = _defaultColor;            
    // }
}