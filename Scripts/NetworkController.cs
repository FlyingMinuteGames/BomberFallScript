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
    public Vector3 m_force = new Vector3(0, 0, 0);
    private NetworkMgr netmgr;
    void Start()
    {
        Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
        
    }   

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        GameObject _netmgr = GameObject.Find("NetworkManager");
        if (_netmgr != null)
            netmgr = _netmgr.GetComponent<NetworkMgr>();
        netmgr.RegisterPlayer(this);
    }
   



    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = (m_force * m_speed);//);
        //= GetCurrentMove() * m_speed;

    }
}
