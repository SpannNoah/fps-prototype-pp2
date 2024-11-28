using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    int m_sensitivity = 5;
    [SerializeField]
    int m_lockVertMin = -90;
    [SerializeField]
    int m_lockVertMax = 90;
    [SerializeField]
    bool m_invertY = false;

    private float m_currentRotationX = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // locks cursor to application
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * m_sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * m_sensitivity * Time.deltaTime;

        // tie mouse y input to the x rotation
        if(m_invertY)
        {
            m_currentRotationX += mouseY;
        }
        else
        {
            m_currentRotationX -= mouseY;
        }

        // clamp camera x rotation
        m_currentRotationX = Mathf.Clamp(m_currentRotationX, m_lockVertMin, m_lockVertMax);

        // rotate camera on x-axis
        transform.localRotation = Quaternion.Euler(m_currentRotationX, 0, 0);

        // rotate the player on y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
