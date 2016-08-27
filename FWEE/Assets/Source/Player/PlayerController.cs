using UnityEngine;
using System.Collections;

public enum Direction {
    Left,
    Right
}

public class PlayerController : MonoBehaviour {

    public GameObject leftArm;
    public GameObject rightArm;

    public Rigidbody2D Rigidbody {
        get {
            return GetComponent<Rigidbody2D>();
        }
    }

    // TODO: Update this to a proper state system. -Dean
    public Direction directionFacing = Direction.Left;
    public float movementSpeedOnGround = 6f;
    public float movementSpeedInAir = 20f;
    public float jumpForce = 5f;

    public float attackDelay = 1f;
    float attackTimeout = 0f;

    Vector3 leftBackArmRestPosition = new Vector3(0.5f, 1, 0);
    Vector3 leftFrontArmRestPosition = new Vector3(0.5f, 0.75f, 0);
    Vector3 rightBackArmRestPosition = new Vector3(-0.5f, 1, 0);
    Vector3 rightFrontArmRestPosition = new Vector3(-0.5f, 0.75f, 0);

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
        ResetLimbPositions();
    }

    // Update is called once per frame.
    void Update() {
        UpdateDirection();

        // Move the player left.
        if (Input.GetKey(KeyCode.A)) {
            Move(Direction.Left);
        }

        // Move the player right.
        if (Input.GetKey(KeyCode.D)) {
            Move(Direction.Right);
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
            Jump();
        }

        // Make the player attack with left arm.
        if (Input.GetMouseButtonDown(0)) {
            Attack(Direction.Left);
        }

        // Make the player attack with right arm.
        if (Input.GetMouseButtonDown(1)) {
            Attack(Direction.Right);
        }


        // Attack timeout.
        if (attackTimeout > 0f) {
            attackTimeout -= Time.deltaTime;

            if (attackTimeout <= 0f) {
                ResetLimbPositions();
                leftArm.GetComponent<CircleCollider2D>().enabled = false;
                rightArm.GetComponent<CircleCollider2D>().enabled = false;
                attackTimeout = 0;
            }
        }

    }

    // Moves the player either to the left or to the right.
    void Move(Direction direction) {
        float movement = 1;

        // Move the player depending on whether it is grounded or not.
        if (IsGrounded) {
            // Apply a velocity movement.

            // If the player is moving left, reverse the movement.
            if (direction == Direction.Left) {
                movement *= -1;
            }

            Rigidbody.velocity = new Vector2(movement * movementSpeedOnGround, Rigidbody.velocity.y);
        } else {
            // Applies a force to the movement along the function y = -((x^2)/(c/2)) + c*2
            movement = -(Mathf.Pow(Rigidbody.velocity.x, 2)/(movementSpeedInAir/2)) + (movementSpeedInAir * 2);

            // If the player is moving left, reverse the movement.
            if (direction == Direction.Left) {
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
    void Attack(Direction direction) {

        if (!IsAttacking) {
            float d = 1;

            switch (direction) {
                case Direction.Left:
                    leftArm.GetComponent<CircleCollider2D>().enabled = true;
                    if (directionFacing == Direction.Left) {
                        d *= -1;
                    }
                    leftArm.transform.Translate(0.5f * d, 0, 0);
                    break;
                case Direction.Right:
                    rightArm.GetComponent<CircleCollider2D>().enabled = true;
                    if (directionFacing == Direction.Left) {
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
            directionFacing = Direction.Left;

            ResetLimbPositions();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            directionFacing = Direction.Right;

            ResetLimbPositions();
        }
    }

    void ResetLimbPositions() {
        switch (directionFacing) {
            case Direction.Left:
                leftArm.transform.localPosition = leftFrontArmRestPosition;
                rightArm.transform.localPosition = rightBackArmRestPosition;

                leftArm.GetComponent<SpriteRenderer>().sortingOrder = 2;
                rightArm.GetComponent<SpriteRenderer>().sortingOrder = 0;
                break;
            case Direction.Right:
                rightArm.transform.localPosition = rightFrontArmRestPosition;
                leftArm.transform.localPosition = leftBackArmRestPosition;

                leftArm.GetComponent<SpriteRenderer>().sortingOrder = 0;
                rightArm.GetComponent<SpriteRenderer>().sortingOrder = 2;
                break;
        }
    }
}
