using UnityEngine;
using System.Collections.Generic;
using Game.EventManagement;
using System;

public class CursorManager : MonoBehaviour
{
    [System.Serializable]
    public class CursorData
    {
        public Texture2D cursorTexture;
        // The hotspot is the point in the cursor texture that will be used as the click point. For example, if you have a crosshair cursor, you might want the hotspot to be at the center of the crosshair.
        public Vector2 hotspot;
        // Cursor will change when this signal is raised. This allows for a decoupled way to change the cursor from other parts of the code.
        public GameEvent cursorChangeSignal;
    }

    public List<CursorData> cursorDataList;

    private void OnEnable()
    {
        foreach (var cursorData in cursorDataList)
        {
            EventManager.StartListening(cursorData.cursorChangeSignal, (EventParam param) => ChangeCursor(cursorData));
        }
    }
    private void OnDisable()
    {
        foreach (var cursorData in cursorDataList)
        {
            EventManager.StopListening(cursorData.cursorChangeSignal, (EventParam param) => ChangeCursor(cursorData));
        }
    }

    private void ChangeCursor(CursorData cursorData)
    {
        Cursor.SetCursor(cursorData.cursorTexture, cursorData.hotspot, CursorMode.Auto);
    }
}
