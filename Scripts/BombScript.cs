using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    public delegate void Callback();
    public Callback callback;
    private float timer = 4;
	// Use this for initialization
	void Start () {
        
	}

    /*IEnumerator WaitAndExplode()
    {
        Debug.Log("begin explode");
        yield return new WaitForSeconds(timer);
        if (callback != null)
            callback();
        Debug.Log("explode!");
    }*/

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        NetworkMgr.Instance.RegisterObj(this,NetworkMgr.ObjectType.OBJECT_BOMB);
    }
	
	// Update is called once per frame
	void Update () {
        if (timer <= 0)
        {
            Debug.Log("explode!");
            if (callback != null)
                callback();
        }
        else timer -= Time.deltaTime;
	}
}
