using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatMeter : MonoBehaviour
{
    public Image m_overheatMeter = null;
    // Start is called before the first frame update
    void Start()
    {

        GunManager.HeatIncreased += HeatIncreased;

    }

    private void OnDestroy()
    {
        GunManager.HeatIncreased -= HeatIncreased;
    }

    private void HeatIncreased(float amount)
    {
        Debug.Log("Heat Increasing by " + amount);
        m_overheatMeter.fillAmount += amount;

        if(m_overheatMeter.fillAmount >= .99)
        {
            m_overheatMeter.color = Color.red;
        }

        if(m_overheatMeter.fillAmount <= .01)
        {
            m_overheatMeter.color = Color.white;
        }
        //Mathf.Clamp01(amount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
