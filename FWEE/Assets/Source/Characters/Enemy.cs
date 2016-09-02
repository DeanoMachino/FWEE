using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public bool walking;
    bool chasing;
    bool movingLeft;
    bool jumping;
    bool punching;

    GameObject player;

    public GameObject fist;
    Collider2D fistColdier; 
    public float speed;
    public float maxSpeed;

    float rangeBetweenPLayer;
    public float hitRange;
    Rigidbody2D body;

	// Use this for initialization
	void Start ()
    {
        fistColdier = fist.GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (!chasing)
        {
            if (walking)
            {
                if (movingLeft)
                {
                    body.AddForce(new Vector2(speed, 0));
                }
                else
                {
                    body.AddForce(new Vector2(-speed, 0));
                }
            }


        }
        else
        {
            rangeBetweenPLayer = Vector3.Distance(player.transform.position, transform.position);

            if (rangeBetweenPLayer < hitRange)
            {
                //hit player
            }
            else
            {
                if (player.transform.position.x < this.transform.position.x)
                {
                    body.AddForce(new Vector2(speed, 0));
                }
                else
                {
                    body.AddForce(new Vector2(-speed, 0));
                }
            }
        }
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Block")
        {
            movingLeft = !movingLeft;
        }
        if(punching)
        {
            if(col.gameObject.tag == "Player")
            {
                if(col.collider.Equals(fistColdier))
                {
                    //player takes damage
                }
            }
        }
    }
}
