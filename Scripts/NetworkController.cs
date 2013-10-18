/**
 *  NetworkController
 *     
 *         
 *  Members: 
 *
 *
 *  Authors: Cyril Basset
 **/

using UnityEngine;
using System.Collections;


public class NetworkController : MonoBehaviour
{

    
    public float m_speed = 1.0f;
    public float m_jumpForce = 1.0f;
    public Vector3 m_force = new Vector3(0, 0, 0);
    private Vector3 current_velocity = new Vector3();
    public float constJumpTimer = 5;
    private int _platformsCollided = 0;
    private float jumpTimer = 5;
    private bool jump = false;
    void Start()
    {
        Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
        
    }   

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        NetworkMgr.Instance.RegisterPlayer(this);
    }
   



    // Update is called once per frame
    void Update()
    {
        //);
        //= GetCurrentMove() * m_speed;

    }
    void FixedUpdate()
    {
        ConstantForce f;
        
        /*current_velocity = (m_force * m_speed) - current_velocity;
        rigidbody.velocity = (m_force * m_speed);
        if (jump)
        {
            if (jumpTimer < 0)
                jump = false;
            rigidbody.velocity += Vector3.forward * jumpTimer * m_jumpForce;
            jumpTimer -= Time.deltaTime;
        }*/
        rigidbody.AddForce(m_force * m_speed * Time.deltaTime);
    }

    public bool CanJump
    {
        get { return _platformsCollided > 0;}
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.contacts[0].normal.z > 0.5f)
        {
            _platformsCollided++;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.contacts[0].normal.z > 0.5f)
        {
            _platformsCollided--;
        }
    }

    public void Jump()
    {
        Debug.Log(Vector3.forward * m_jumpForce);
        jumpTimer = constJumpTimer;
        jump = true;
        rigidbody.AddForce(Vector3.forward * m_jumpForce,ForceMode.Impulse);
    }
}
