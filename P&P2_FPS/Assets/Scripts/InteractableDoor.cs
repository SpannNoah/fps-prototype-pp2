using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private bool m_isLocked = false;
    [SerializeField] private GameObject m_rightPanel = null;
    [SerializeField] private GameObject m_leftPanel = null;
    [SerializeField] private float m_slideDistance = 0f;
    [SerializeField] private float m_slideSpeed = 0f;
    [SerializeField] private bool m_slideAlongZ = false;

    private Vector3 m_leftClosedPos, m_rightClosedPos = Vector3.zero;
    private Vector3 m_leftOpenPos, m_rightOpenPos = Vector3.zero;
    private float m_lerpProgress = 0f;
    private bool m_isOpen = false;
    private bool m_isMoving = false;

    private void Start()
    {
        m_leftClosedPos = m_leftPanel.transform.localPosition;
        m_rightClosedPos = m_rightPanel.transform.localPosition;

        Vector3 slideDirection;
        if(m_slideAlongZ)
        {
            slideDirection = transform.forward * m_slideDistance;
        }
        else
        {
            slideDirection = transform.right * m_slideDistance;
        }
        m_leftOpenPos = m_leftClosedPos - slideDirection;
        m_rightOpenPos = m_rightClosedPos + slideDirection;
    }

    void Update()
    {
        OpenDoor();
    }

    public void Interact()
    {
        m_isMoving = true;
        m_lerpProgress = 0f;
    }

    public string ShowInteractText()
    {
        return string.Empty;
    }
    // Update is called once per frame

    public void OpenDoor()
    {
        if (!m_isMoving || m_isOpen) return;

        m_lerpProgress += m_slideSpeed * Time.deltaTime;
        m_lerpProgress = Mathf.Clamp01(m_lerpProgress);

        m_leftPanel.transform.localPosition = Vector3.Lerp(m_leftClosedPos, m_leftOpenPos, m_lerpProgress);
        m_rightPanel.transform.localPosition = Vector3.Lerp(m_rightClosedPos, m_rightOpenPos, m_lerpProgress);

        if(m_lerpProgress >= 1f)
        {
            m_isMoving = false;
            m_isOpen = true;
        }
    }
}
