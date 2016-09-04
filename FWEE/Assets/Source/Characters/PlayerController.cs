using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum PlayerState {
    Left,
    Right,
}

public class PlayerController : MonoBehaviour {


    public GameObject leftArm;
    public GameObject rightArm;

    public Rigidbody2D Rigidbody {
        get {
            return GetComponent<Rigidbody2D>();
        }
    }

    AudioSource audioSource; 
    Skeleton skeleton { get { return this.GetComponentInChildren<Skeleton>(); } }
   
    Animator animator {  get { return GetComponent<Animator>(); } }
    bool walking;

    // TODO: Update this to a proper state system. -Dean
    public PlayerState directionFacing = PlayerState.Left;
    public float movementSpeedOnGround = 6f;
    public float movementSpeedInAir = 20f;
    public float jumpForce = 5f;

    public float attackDelay = 1f;
    float attackTimeout = 0f;

    bool punching = false;

    float orbManaCount = 0;
    public Text orbCountText;


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
    void Start()
    {
        //ResetBodyPartPositions();
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();


    }

    // Update is called once per frame.
    void Update()
    {
        OrbCountUpdate();
        if(Input.GetKeyDown(KeyCode.M))
        {
            skeleton.flipY = true;
        }
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);


        // Move the player left.
        if (Input.GetKey(KeyCode.A)) {
            Move(PlayerState.Left);
            walking = true;
            if(!skeleton.flipY)
            {
                skeleton.flipY = true;
            }
        }

        // Move the player right.
        if (Input.GetKey(KeyCode.D)) {
            Move(PlayerState.Right);
            walking = true;

            if(skeleton.flipY)
            {
                skeleton.flipY = false;
            }
        }

        if((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) ||
            Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            if(IsGrounded){
                Rigidbody.velocity = new Vector2(0, 0);             
                walking = false;
            }
            else {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x * 0.98f, Rigidbody.velocity.y);
            }
        }


        animator.SetBool("Moving", walking);
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

    void StartPunching()
    {
        punching = true;
    }

    void StopPunching()
    {
        punching = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {

     
        if (col.gameObject.tag == "Enemy")
        {
            
            if (punching)
            {
                col.gameObject.GetComponentInChildren<HealthBar>().TakeDamage(30);
                punching = false;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Orb")
        {
            Destroy(col.gameObject);
            orbManaCount++;
            //play sound
        }
    }

    void OrbCountUpdate()
    {
        orbCountText.text = orbManaCount.ToString();
    }

}
