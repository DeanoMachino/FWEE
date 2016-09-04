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

    public GameObject orb;

	// Use this for initialization
	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        fistColdier = fist.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", walking);
        chasing = false;
        //body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(0, 0, 0, 1), 100*Time.deltaTime);
        
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
           
            float rectWidth = this.GetComponent<Collider2D>().bounds.size.x;
            float rectHeight = this.GetComponent<Collider2D>().bounds.size.y;
            Vector3 playerCenter = this.GetComponent<Collider2D>().bounds.center;
            

          
                Vector3 contactPoint = col.contacts[0].point;
                Vector3 center = collider.bounds.center;

            float cp = contactPoint.y;
            float pc = playerCenter.y;
            float l = (rectHeight / 2);
            //Debug.Log("platform hit CP:" + contactPoint.y.ToString() + " PC:" + playerCenter.y.ToString() + " L:" + (rectHeight/2) + " PC - L:" + (pc - l).ToString() );

            if((pc - l) + 0.1 > cp &&  (pc - l) - 0.1 < cp )
            {
                
            }
            else
            {
                if(playerCenter.x < contactPoint.x)
                {
                    if(!movingLeft)
                    {
                        GetComponentInChildren<Skeleton>().flipY = true;
                    }
                    movingLeft = true;
                }
                else
                {
                    if (movingLeft)
                    {
                        GetComponentInChildren<Skeleton>().flipY = false;
                    }
                    movingLeft = false;
                }
            }

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

    public void Die()
    {
        int noOrbs = Random.RandomRange(3, 10);
        int radius = 1; //noOrbs * 1 ; 
        Vector3 center = transform.position;

        
        for (int i = 0; i < noOrbs; i++)
        {
            
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad) + 4;
            pos.z = center.z;

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
            Instantiate(orb, pos, rot);

        }

        Destroy(gameObject);
    }
}
