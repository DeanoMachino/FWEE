using UnityEngine;
using System.Collections;

public enum Direction {
    Left,
    Right
}

public class PlayerController : MonoBehaviour {

    public bool isDead = false;
    public float movementSpeed = 6f;
    public float jumpForce;
    public bool IsGrounded {
        get {
            return Physics.Raycast(transform.position, -Vector2.up, GetComponent<BoxCollider2D>().bounds.extents.y + 0.1f);
        }
    }

    public Rigidbody2D Rigidbody {
        get {
            return GetComponent<Rigidbody2D>();
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.A)) {
            Move(Direction.Left);
        }
        if (Input.GetKey(KeyCode.D)) {
            Move(Direction.Right);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
            Debug.Log("Jump");
        }


        if (IsGrounded) {
            Debug.Log("Player Grounded");
        }
	
	}

    void Move(Direction direction) {
        float movement = movementSpeed * Time.deltaTime;

        if (direction == Direction.Left) {
            movement *= -1;
        }

        transform.Translate(new Vector3(movement, 0, 0));
    }

    void Jump() {
        Rigidbody.velocity = new Vector2(0, jumpForce);
    }
}
