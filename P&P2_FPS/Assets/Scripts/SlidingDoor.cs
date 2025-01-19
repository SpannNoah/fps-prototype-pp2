using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorLeft;  // Parent object for the left door
    public Transform doorRight; // Parent object for the right door

    public float moveDistance = 3f;  // Distance each door moves
    public float openSpeed = 2f;     // Speed of the door's movement

    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;
    private Vector3 leftOpenPosition;
    private Vector3 rightOpenPosition;

    private bool isOpen = false;
    private bool playerInRange = false;

    private Collider leftCollider;
    private Collider rightCollider;

    void Start()
    {
        // Store initial positions
        leftClosedPosition = doorLeft.localPosition;
        rightClosedPosition = doorRight.localPosition;

        // Adjust these vectors based on intended movement direction
        leftOpenPosition = leftClosedPosition + new Vector3(0, 0, -moveDistance); // Move left door "backward" along Z-axis
        rightOpenPosition = rightClosedPosition + new Vector3(0, 0, moveDistance); // Move right door "forward" along Z-axis

        leftCollider = doorLeft.GetComponent<Collider>();
        rightCollider = doorRight.GetComponent<Collider>();
    }

    void Update()
    {
        // Toggle door state
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            Debug.Log($"Door toggled. IsOpen={isOpen}");

            leftCollider.enabled = !isOpen;
            rightCollider.enabled = !isOpen;
        }

        // Smoothly move doors to their target positions
        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, isOpen ? leftOpenPosition : leftClosedPosition, Time.deltaTime * openSpeed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, isOpen ? rightOpenPosition : rightClosedPosition, Time.deltaTime * openSpeed);

        // Debugging positions
        Debug.Log($"Left Door Position: {doorLeft.localPosition} | Right Door Position: {doorRight.localPosition}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered interaction range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left interaction range.");
        }
    }
}












