using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    private static int timer = 4; // in seconds;
    public delegate void Callback();
    public Callback callback;
	// Use this for initialization
	void Start () {
        StartCoroutine(WaitAndExplode());
	}

    IEnumerator WaitAndExplode()
    {
        Debug.Log("begin explode");
        yield return new WaitForSeconds(timer);
        if (callback != null)
            callback();
        Debug.Log("explode!");
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        NetworkMgr.Instance.RegisterObj(this,NetworkMgr.ObjectType.OBJECT_BOMB);
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
