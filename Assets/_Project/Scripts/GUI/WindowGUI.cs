using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowGUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    // The rectangle transform of the GUI window
    RectTransform m_Rect;

    // The offset of the click when pressed down
    Vector2 m_OffsetFromClick;

    // Position of the window
    Vector2 m_Pos;

    // Constrains the window to the sceen space
    public bool m_ConstrainToScreenSpace = true;

    private void Start()
    {
        // Get the rectangle component
        m_Rect = GetComponent<RectTransform>();

        // Set the acnhor to top left
        m_Rect.anchorMax = new Vector2(0, 1);
        m_Rect.anchorMin = new Vector2(0, 1);

        // Set the pivot to top left
        m_Rect.pivot = new Vector2(0, 1);

        m_Pos.x = m_Rect.anchoredPosition.x;
        m_Pos.y = m_Rect.anchoredPosition.y;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        m_OffsetFromClick = m_Pos - data.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Pos = eventData.position + m_OffsetFromClick;
        if(m_ConstrainToScreenSpace) ConstraintToScreenSpace();
        m_Rect.anchoredPosition = m_Pos;
    }

    void ConstraintToScreenSpace()
    {
        m_Pos.x = Mathf.Max(m_Pos.x, 0);
        m_Pos.x = Mathf.Min(m_Pos.x, Screen.width - m_Rect.rect.width);

        m_Pos.y = Mathf.Min(m_Pos.y, 0);
        m_Pos.y = Mathf.Max(m_Pos.y, -Screen.height + m_Rect.rect.height);
    }
}
