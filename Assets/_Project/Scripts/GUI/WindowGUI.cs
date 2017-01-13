using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowGUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector2 m_OffsetFromClick;
    Vector2 m_Pos;

    private void Start()
    {
        m_Pos.x = transform.position.x;
        m_Pos.y = transform.position.y;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        m_OffsetFromClick = m_Pos - data.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Pos = eventData.position + m_OffsetFromClick;
        transform.position = m_Pos;
    }
}
