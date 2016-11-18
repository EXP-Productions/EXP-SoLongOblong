using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
    public Transform m_LookAt;

	void Update () 
    {
        transform.LookAt(m_LookAt);
	}
}
