using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool DraggingPuzzlePiece{get; private set;}

    void Start() 
    {   
        // Safety in case the first cover was disabled for testing purposes and then forgotten to be reenabled
        if (!GameObject.FindGameObjectWithTag("Cover1").activeSelf)
            GameObject.FindGameObjectWithTag("Cover1").SetActive(true);
    }

    void OnEnable() 
    {
        DragDrop.PuzzlePieceDraggedEvent += OnDraggingPieceEvent;
    }

    void OnDisable() 
    {
        DragDrop.PuzzlePieceDraggedEvent -= OnDraggingPieceEvent;
    }

    void Update() {
        if (Input.GetKeyDown("escape"))
            Application.Quit();
            //Später: Menü aktivieren, anderes UI deaktivieren
    }

    void OnDraggingPieceEvent(bool dragging) => DraggingPuzzlePiece = dragging;
}