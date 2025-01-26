using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreditScript : MonoBehaviour
{
    public float scrollSpeed = 50f;
    
    private RectTransform m_rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // get the transform component of the UI element
        m_rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // move the UI element up
        m_rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }

  
}
