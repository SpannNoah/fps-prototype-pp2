using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlRoomMonitor : MonoBehaviour, IInteractable
{
    public Portal m_portal;
    public TextMeshPro m_interactText;
    public void Interact()
    {
        m_portal.gameObject.SetActive(true);
    }

    public string ShowInteractText()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_interactText.gameObject.SetActive(false);
        }
    }
}
