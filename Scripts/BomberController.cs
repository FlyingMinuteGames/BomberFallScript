/**
 *  BomberController
 *     
 *         
 *  Members: 

 *
 *  Authors: Cyril Basset
 **/

using UnityEngine;
using System.Collections;

public enum WorldState
{
    CENTER,
    LATERAL_X,
    LATERAL_X2,
    LATERAL_Z,
    LATERAL_Z2
}

public class BomberController : MonoBehaviour {

	delegate void Callback(BomberController me, bool enable);
    public float m_speed = 1.0f;
    public Vector3 m_force = new Vector3(0,0,0);
    private KeyCode[] key_binding = {KeyCode.Z,KeyCode.Q,KeyCode.S,KeyCode.D,KeyCode.Keypad2,KeyCode.Keypad5};
    private Callback[] action_callback = {
                                            (me,enable) => { me.m_force += enable ? Vector3.forward : Vector3.back; },
                                            (me,enable) => { me.m_force += !enable ? Vector3.right : Vector3.left; },
                                            (me,enable) => { me.m_force += !enable ? Vector3.forward : Vector3.back; },
                                            (me,enable) => { me.m_force += enable ? Vector3.right : Vector3.left; },
                                            (me,enable) => { /*Debug.Log("callback !");*/ Physics.gravity = Vector3.back*Config.CONST_GRAVITY * Config.CONST_FACTOR;},
                                            (me,enable) => { Physics.gravity = Vector3.down*Config.CONST_GRAVITY * Config.CONST_FACTOR;}
                                          };
    private Vector3[] action_binding = {
                                         new Vector3(0,0,1),
                                         new Vector3(-1,0,0),
                                         new Vector3(0,0,-1),
                                         new Vector3(1,0,0)
                                     };
    private WorldState m_state = WorldState.CENTER;
    void Start () {
        Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
	}
	

    Vector3 GetCurrentMove()
    {
        for(int i = 0, len = key_binding.Length; i < len; i++)
        {
            if (Input.GetKeyDown(key_binding[i]))
                action_callback[i](this, true);
            else if (Input.GetKeyUp(key_binding[i]))
                action_callback[i](this, false);
        }

        return m_force.normalized;
    }



	// Update is called once per frame
	void Update () 
    {
        Debug.Log("current force " + Physics.gravity);
        rigidbody.velocity = (GetCurrentMove() * m_speed);//);
        //= GetCurrentMove() * m_speed;

	}
}
