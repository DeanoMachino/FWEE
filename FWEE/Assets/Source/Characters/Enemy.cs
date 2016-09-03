using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public bool walking;
    bool chasing;
    public bool movingLeft;
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

    Animator animator;

    public float currentVelocity;

	// Use this for initialization
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        fistColdier = fist.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", walking);
        chasing = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        currentVelocity = body.velocity.magnitude;
        if (!chasing)
        {
            if (walking)
            {
                if (movingLeft)
                {
                    body.AddForce(new Vector2(-speed * Time.deltaTime, 0));
                }
                else
                {
                    body.AddForce(new Vector2(speed * Time.deltaTime, 0));
                }
            }


        }
        else
        {
            rangeBetweenPLayer = Vector3.Distance(player.transform.position, transform.position);

            if (rangeBetweenPLayer < hitRange)
            {
                animator.SetTrigger("RightPunch");
                punching = true;

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



        VelocityCheck();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Platform")
        {
            Collider2D collider = col.collider;
            
            bool collideFromLeft;
            bool collideFromTop;
            bool collideFromRight;
            bool collideFromBottom;
            float rectWidth = this.GetComponent<Collider2D>().bounds.size.x;
            float rectHeight = this.GetComponent<Collider2D>().bounds.size.y;
            Vector3 playerCenter = this.GetComponent<Collider2D>().bounds.center;
            

          
                Vector3 contactPoint = col.contacts[0].point;
                Vector3 center = collider.bounds.center;


            /*if (contactPoint.y > center.y && //checks that circle is on top of rectangle
                (contactPoint.x < center.x + rectWidth / 2 && contactPoint.x > center.x - rectWidth / 2))
            {
                collideFromTop = true;
                Debug.Log("hit from up");
            }
            else if (contactPoint.y < center.y &&
                (contactPoint.x < center.x + rectWidth / 2 && contactPoint.x > center.x - rectWidth / 2))
            {
                collideFromBottom = true;
                Debug.Log("hit from down");
            }
            else if (contactPoint.x > center.x &&
                (contactPoint.y < center.y + RectHeight / 2 && contactPoint.y > center.y - RectHeight / 2))
            {
                collideFromRight = true;
                movingLeft = false;
                Debug.Log("hit from right");
            ///transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (contactPoint.x < center.x &&
                (contactPoint.y < center.y + RectHeight / 2 && contactPoint.y > center.y - RectHeight / 2))
            {
                collideFromLeft = true;
            Debug.Log("hit from left");
            movingLeft = true;
                ///transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);  
            }*/
            float cp = contactPoint.y;
            float pc = playerCenter.y;
            float l = (rectHeight / 2);
            Debug.Log("platform hit CP:" + contactPoint.y.ToString() + " PC:" + playerCenter.y.ToString() + " L:" + (rectHeight/2) + " PC - L:" + (pc - l).ToString() );

            if((pc - l) + 0.1 > cp &&  (pc - l) - 0.1 < cp )
            {
                Debug.Log("floor platform");
            }
            else
            {
                if(playerCenter.x < contactPoint.x)
                {
                    movingLeft = true;
                }
                else
                {
                    movingLeft = false;
                }
            }

            /*if(contactPoint.y == (center.y + (collider.bounds.size.y / 2)))
            {
             
                if (contactPoint.x < playerCenter.x)
                {
                    movingLeft = false;

                }
                else
                {
                    movingLeft = true;
                }
            }
            else
            {
               
            }*/



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

    void VelocityCheck()
    {
        if ((body.velocity.x * body.velocity.x) > (maxSpeed * maxSpeed))
        {
            if (body.velocity.x < 0)
            {
                body.velocity = new Vector2(-maxSpeed, body.velocity.y);
            }
            else
            {
                body.velocity = new Vector2(maxSpeed, body.velocity.y);
            }
        }
    }

    void PunchingStop()
    {
        punching = false;
    }

}
