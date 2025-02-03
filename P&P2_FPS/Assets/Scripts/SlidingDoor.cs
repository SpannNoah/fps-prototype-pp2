using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorLeft;  // Left door parent object
    public Transform doorRight; // Right door parent object

    public float moveDistance = 3f;  // Distance each door moves
    public float openSpeed = 2f;     // Speed of the door's movement

    // Movement directions (can be adjusted in Inspector)
    public Vector3 leftDoorMoveDirection = new Vector3(-1, 0, 0); // Default: Left door moves left on X-axis
    public Vector3 rightDoorMoveDirection = new Vector3(1, 0, 0); // Default: Right door moves right on X-axis

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

        // Calculate open positions based on movement directions
        leftOpenPosition = leftClosedPosition + (leftDoorMoveDirection.normalized * moveDistance);
        rightOpenPosition = rightClosedPosition + (rightDoorMoveDirection.normalized * moveDistance);

        // Get colliders
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

            // Enable or disable colliders
            leftCollider.enabled = !isOpen;
            rightCollider.enabled = !isOpen;
        }

        // Smoothly move doors to their target positions
        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, isOpen ? leftOpenPosition : leftClosedPosition, Time.deltaTime * openSpeed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, isOpen ? rightOpenPosition : rightClosedPosition, Time.deltaTime * openSpeed);

        // Debugging positions
        //Debug.Log($"Left Door Position: {doorLeft.localPosition} | Right Door Position: {doorRight.localPosition}");
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














