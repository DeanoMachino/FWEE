using UnityEngine;
using System.Collections;

public enum PlayerState {
    Left,
    Right,
}

public class PlayerController : MonoBehaviour {

    public GameObject head;
    public GameObject body;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftLeg;
    public GameObject rightLeg;

    public Rigidbody2D Rigidbody {
        get {
            return GetComponent<Rigidbody2D>();
        }
    }

    Animator animator {  get { return GetComponent<Animator>(); } }
    bool walking;

    // TODO: Update this to a proper state system. -Dean
    public PlayerState directionFacing = PlayerState.Left;
    public float movementSpeedOnGround = 6f;
    public float movementSpeedInAir = 20f;
    public float jumpForce = 5f;

    public float attackDelay = 1f;
    float attackTimeout = 0f;

    Vector3 headLeftRestPosition = new Vector3(-0.15f, 0.9f, 0);
    Vector3 headRightRestPosition = new Vector3(0.15f, 0.9f, 0);
    Vector3 leftBackArmRestPosition = new Vector3(0.45f, 0, 0);
    Vector3 leftFrontArmRestPosition = new Vector3(0.45f, -0.2f, 0);
    Vector3 rightBackArmRestPosition = new Vector3(-0.45f, 0, 0);
    Vector3 rightFrontArmRestPosition = new Vector3(-0.45f, -0.2f, 0);
    Vector3 frontLegRestPosition = new Vector3(0.4f, -0.8f, 0);
    Vector3 backLegRestPosition = new Vector3(-0.3f, -0.8f, 0);



    // Returns whether the player is on the ground.
    // NOTE: This works for flat pieces of ground but will not work for slopes. -Dean
    public bool IsGrounded {
        get {
            return (Rigidbody.velocity.y == 0);
        }
    }

    // Returns whether the player is currently attacking.
    public bool IsAttacking {
        get {
            return !(attackTimeout <= 0f);
        }
    }

    // Returns whether the player is still alive.
    // TODO: Implement player health. -Dean
    public bool IsDead {
        get {
            return false;
        }
    }

    // Use this for initialization.
    void Start() {
        //ResetBodyPartPositions();
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame.
    void Update() {
        UpdateDirection();
        Camera.main.transform.position = transform.position;


        // Move the player left.
        if (Input.GetKey(KeyCode.A)) {
            Move(PlayerState.Left);
        }

        // Move the player right.
        if (Input.GetKey(KeyCode.D)) {
            Move(PlayerState.Right);
        }

        if((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) ||
            Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            if(IsGrounded){
                Rigidbody.velocity = new Vector2(0, 0);
            } else {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x * 0.98f, Rigidbody.velocity.y);
            }
        }

        // Make the player jump.
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (IsGrounded)
            {
                animator.SetTrigger("Jump");
            }
            //Jump();
        }

        if(Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("DoublePunch");
        }
        // Make the player attack with left arm.
       else if (Input.GetMouseButtonDown(0)) {
            //Attack(PlayerState.Left);
            animator.SetTrigger("LeftPunch");
        }

        // Make the player attack with right arm.
       else if (Input.GetMouseButtonDown(1)) {
            //Attack(PlayerState.Right);
            animator.SetTrigger("RightPunch");
        }


        // Attack timeout.
        if (attackTimeout > 0f) {
            attackTimeout -= Time.deltaTime;

            if (attackTimeout <= 0f) {
                ResetBodyPartPositions();
                leftArm.GetComponent<CircleCollider2D>().enabled = false;
                rightArm.GetComponent<CircleCollider2D>().enabled = false;
                attackTimeout = 0;
            }
        }

    }

    // Moves the player either to the left or to the right.
    void Move(PlayerState direction) {
        float movement = 1;

        // Move the player depending on whether it is grounded or not.
        if (IsGrounded) {
            // Apply a velocity movement.

            // If the player is moving left, reverse the movement.
            if (direction == PlayerState.Left) {
                movement *= -1;
            }

            Rigidbody.velocity = new Vector2(movement * movementSpeedOnGround, Rigidbody.velocity.y);
        } else {
            // Applies a force to the movement along the function y = -((x^2)/(c/2)) + c*2
            movement = -(Mathf.Pow(Rigidbody.velocity.x, 2)/(movementSpeedInAir/2)) + (movementSpeedInAir * 2);

            // If the player is moving left, reverse the movement.
            if (direction == PlayerState.Left) {
                movement *= -1;
            }

            Rigidbody.AddForce(new Vector2(movement, 0));

            // Enforce an x velocity cap
            if (Rigidbody.velocity.x > movementSpeedOnGround) {
                Rigidbody.velocity = new Vector2(movementSpeedOnGround, Rigidbody.velocity.y);
            }
        }

    }

    // Makes the player perform a jump, so long as it is grounded and able to do so.
    void Jump() {
        if (IsGrounded) {
            // Perform a jump.
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, jumpForce);
        }
    }

    // Performs an attack, depending on which arm is making the attack.
    // TODO: Implement method to handle "super" attack. -Dean
    void Attack(PlayerState direction) {

        if (!IsAttacking) {
            float d = 1;

            // TODO: Implement actual attacks with smooth animations. -Dean
            switch (direction) {
                case PlayerState.Left:
                    leftArm.GetComponent<CircleCollider2D>().enabled = true;
                    if (directionFacing == PlayerState.Left) {
                        d *= -1;
                    }
                    leftArm.transform.Translate(0.5f * d, 0, 0);
                    break;
                case PlayerState.Right:
                    rightArm.GetComponent<CircleCollider2D>().enabled = true;
                    if (directionFacing == PlayerState.Left) {
                        d *= -1;
                    }
                    rightArm.transform.Translate(0.5f * d, 0, 0);
                    break;
            }

            attackTimeout = attackDelay;
        }
    }

    // Change the player stance to match the direction they are facing.
    void UpdateDirection() {
        if(Input.GetKeyDown(KeyCode.A)) {
            directionFacing = PlayerState.Left;

            walking = true;
            //ResetBodyPartPositions();
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            directionFacing = PlayerState.Right;

            walking = true;
            //ResetBodyPartPositions();
        }
        else
        {
            walking = false;
        }

        animator.SetBool("Moving", walking);
    }

    void ResetBodyPartPositions() {
        switch (directionFacing) {
            case PlayerState.Left:
                // Head
                head.transform.localPosition = headLeftRestPosition;
                
                // Arms
                leftArm.transform.localPosition = leftFrontArmRestPosition;
                rightArm.transform.localPosition = rightBackArmRestPosition;

                leftArm.GetComponent<SpriteRenderer>().sortingOrder = 2;
                rightArm.GetComponent<SpriteRenderer>().sortingOrder = 0;

                // Legs
                rightLeg.transform.localPosition = frontLegRestPosition;
                leftLeg.transform.localPosition = backLegRestPosition;
                break;
            case PlayerState.Right:
                // Head
                head.transform.localPosition = headRightRestPosition;

                // Arms
                rightArm.transform.localPosition = rightFrontArmRestPosition;
                leftArm.transform.localPosition = leftBackArmRestPosition;

                leftArm.GetComponent<SpriteRenderer>().sortingOrder = 0;
                rightArm.GetComponent<SpriteRenderer>().sortingOrder = 2;

                // Legs
                leftLeg.transform.localPosition = frontLegRestPosition;
                rightLeg.transform.localPosition = backLegRestPosition;
                break;
        }
    }
}
