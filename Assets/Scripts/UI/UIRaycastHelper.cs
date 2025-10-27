using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastHelper
{
    private static PointerEventData _eventData;
    private static List<RaycastResult> _results = new List<RaycastResult>();

    public static GameObject RaycastUI(Camera cam)
    {
        if (EventSystem.current == null)
            return null;

        if (_eventData == null)
            _eventData = new PointerEventData(EventSystem.current);

        _eventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f); // center screen (crosshair)
        _results.Clear();
        EventSystem.current.RaycastAll(_eventData, _results);

        if (_results.Count > 0)
            return _results[0].gameObject;

        return null;
    }
}
