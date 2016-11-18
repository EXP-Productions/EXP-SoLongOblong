using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class ButtonClickDragEvent : MonoBehaviour, IPointerDownHandler
{
    Vector3 m_PressDownPos;
    Vector3 m_CurrentMousePos;

    bool m_Pressed = false;
    public bool m_OutputDelta = true;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    public FloatEvent OnDragUpdate;

    float m_DistFromPointerDown;
    float m_PrevDistFromPointerDown;

    void Update()
    {
        if (m_Pressed && Input.GetMouseButtonUp(0))
        {
            Unclick();
        }
        else if (m_Pressed)
        {
            m_PrevDistFromPointerDown = m_DistFromPointerDown;

            m_CurrentMousePos = Input.mousePosition;
            m_DistFromPointerDown = m_CurrentMousePos.x - m_PressDownPos.x;
            m_DistFromPointerDown /= Screen.width;

            if( m_OutputDelta )
            {
                print("Drag value: " + ( m_DistFromPointerDown - m_PrevDistFromPointerDown ) );
                OnDragUpdate.Invoke(m_DistFromPointerDown - m_PrevDistFromPointerDown);
            }
            else
            {
                print("Drag value: " + m_DistFromPointerDown);
                OnDragUpdate.Invoke(m_DistFromPointerDown);
            }
           
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + " Was click drag started.");
        m_PressDownPos = Input.mousePosition;
        m_Pressed = true;
    }


    void Unclick()
    {
        m_Pressed = false;
    }
}
