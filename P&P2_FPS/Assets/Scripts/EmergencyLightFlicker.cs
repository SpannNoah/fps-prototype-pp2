using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyLightFlicker : MonoBehaviour
{
    [SerializeField] private Light m_light = null;
    [SerializeField] private float m_flickerRate = .75f;

    private float m_nextFlickerTime = 0f;

    private void Start()
    {
        StartNextFlicker();
    }
    void Update()
    {
        if(Time.time >= m_nextFlickerTime)
        {
            m_light.enabled = !m_light.enabled;
            StartNextFlicker();
        }
    }

    private void StartNextFlicker()
    {
        m_nextFlickerTime = Time.time + m_flickerRate;
    }
}
