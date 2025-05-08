using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed = 5f; // Speed of the player movement
    public float maxJumpForce = 15f; // Increased maximum force applied for jumping
    public float maxChargeTime = 2f; // Limited maximum time to charge the jump
    Rigidbody2D rb; // Reference to the Rigidbody2D component

    private bool isGrounded = true; // Check if the player is on the ground
    private bool isCharging = false; // Check if the player is charging the jump
    private float chargeTime = 0f; // Time the jump is being charged

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player GameObject
    }

    // Update is called once per frame
    void Update()
    {
        // Start charging the jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCharging)
        {
            isCharging = true;
            chargeTime = 0f; // Reset charge time
        }

        // Continue charging the jump and check for jump conditions
        if (isCharging)
        {
            chargeTime += Time.deltaTime;

            bool keyReleased = Input.GetKeyUp(KeyCode.Space);
            bool maxChargeReached = chargeTime >= maxChargeTime;

            if (maxChargeReached)
            {
                chargeTime = maxChargeTime; // Ensure chargeTime is capped at max for calculation
                PerformJump();
            }
            else if (keyReleased)
            {
                // Jump with current chargeTime (which is < maxChargeTime)
                PerformJump();
            }
        }
    }

    void PerformJump()
    {
        isCharging = false; // Stop charging state

        // Clamp chargeTime here to ensure the ratio is correct for Lerp,
        // even if it slightly exceeded maxChargeTime before this method was called.
        float currentActualChargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        float jumpForce = Mathf.Lerp(0f, maxJumpForce, currentActualChargeTime / maxChargeTime); // Calculate jump force

        // Get horizontal input for jump direction
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 jumpDirection = new Vector2(horizontalInput, 1).normalized; // Combine horizontal input with upward direction
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse); // Apply force in the calculated direction

        isGrounded = false; // Set isGrounded to false after jumping
    }

    void FixedUpdate()
    {
        // Allow horizontal movement only when the player is grounded and not charging
        if (isGrounded && !isCharging)
        {
            float moveHorizontal = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrow keys)

            if (Mathf.Abs(moveHorizontal) > 0.01f) // Only move if there's significant input
            {
                Vector2 movement = new Vector2(moveHorizontal, 0) * speed; // Create a new Vector2 for horizontal movement
                transform.Translate(movement * Time.deltaTime); // Move the player using the Rigidbody2D component
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement if no input is detected
            }
        }
    }

    // Detect collision with the ground to reset isGrounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Ensure the ground has the "Ground" tag
        {
            isGrounded = true; // Reset isGrounded when touching the ground
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement upon landing
        }
    }
}
