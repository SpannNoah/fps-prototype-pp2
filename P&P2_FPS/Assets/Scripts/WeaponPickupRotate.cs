using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    [Header("Bobbing Settings")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float bobOffset = Mathf.Sin(Time.time * bobSpeed + timeOffset) * bobHeight;
        transform.position = startPosition + new Vector3(0, bobOffset, 0);
    }
}
