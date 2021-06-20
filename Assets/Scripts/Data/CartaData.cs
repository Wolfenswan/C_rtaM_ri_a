using UnityEngine;

[CreateAssetMenu(fileName="CartaData", menuName="Data/Carta Data")]
public class CartaData : ScriptableObject 
{   
    public float MouseZoomSpeed = 0.5f;
    public NEINGames.Collections.RangeInt MouseZoomClampRange;
    public float AlphaBlendSpeed;
    public float SlotFadeInTime = 1f;
    public float CoverFadeOutTime = 1f;
    public Color PuzzlePieceGlowColor = new Color(255,255,255, 1);
}
