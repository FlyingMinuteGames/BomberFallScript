using UnityEngine;
using System.Collections;

public class NetworkMgr : MonoBehaviour {

    public enum ObjectType
    { 
        OBJECT_PLAYER,
        OBJECT_BLOCK,
        OBJECT_POWERUP
    }

    public bool server;
    public int listenPort = 2500;
    public string remoteIP;
    public int playerIndex = 0;
    public GameObject networkController_tpl;
    public GameObject bomb;
    private NetworkPlayer[] _players = new NetworkPlayer[4];
    private NetworkController[] controllers = new NetworkController[4];
    private Maps current_map;
	// Use this for initialization
    Vector3 GetInitPos(int index)
    {
        if (index < 0 || index > 3)
            return new Vector3();
        Vector3 pos  = current_map.TilePosToWorldPos(new IntVector2(index != 0 ? current_map.Size.x - 1 : 0, index != 0 ? current_map.Size.y - 1 : 0));
        return pos;
    }
	void Start () {
        Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
        Application.runInBackground = true;
        if (server)
        {
            Network.InitializeSecurity();
            Network.InitializeServer(2, listenPort, true);
            
        }
        else
        {
            Debug.Log("Trying to log to " + remoteIP);
            Network.Connect(remoteIP, listenPort);
        }
        current_map = Maps.LoadMapsFromFile("map1.map");
        bomb = ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb");
	}



    void OnPlayerConnected(NetworkPlayer player)
    {
        if(current_map == null)
            return;
        if (server)
        {
            Debug.Log("A player has connected !");
            int index = Network.connections.Length -1;
            _players[index] = player;
            Network.Instantiate(networkController_tpl, GetInitPos(index), Quaternion.identity, 1);

            //controllers[index] = obj.GetComponent<NetworkController>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!server)
            UpdateCurrentMove();
	}

    delegate void Callback(NetworkMgr me, bool enable);
    public Vector3 m_force = new Vector3(0, 0, 0);
    private KeyCode[] key_binding = { KeyCode.Z, KeyCode.Q, KeyCode.S, KeyCode.D, KeyCode.Keypad2, KeyCode.Keypad5 };
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

    public void RegisterPlayer(NetworkController pl)
    {
        controllers[playerIndex++] = pl;
    }

    void UpdateCurrentMove()
    {
        bool needUpdate = false;
        for (int i = 0, len = key_binding.Length; i < len; i++)
        {
            if (Input.GetKeyDown(key_binding[i]))
            {
                action_callback[i](this, true);
                needUpdate = true;
            }
            else if (Input.GetKeyUp(key_binding[i]))
            {
                action_callback[i](this, false);
                needUpdate = true;
            } 
        }
        if(needUpdate)
            this.networkView.RPC("ServerRecvChangeMove", RPCMode.Server, Network.player, m_force.normalized);

    }

    int GetPlayerIndex(NetworkPlayer p)
    {
        for (int i = 0; i < 4; i++)
        {
            if (_players[i] == p)
                return i;
        }
        return -1;
    }

    [RPC]
    void ServerRecvChangeMove(NetworkPlayer player, Vector3 move)
    {
        if (!server)
            return;
        Debug.Log("recv ServerRecvChangeMove opcode with " + move);
        int pIndex;
        if ((pIndex = GetPlayerIndex(player)) >= 0)
        {
            controllers[pIndex].m_force = move;
            Debug.Log("update player " + pIndex);
            networkView.RPC("ClientRecvChangeMove", RPCMode.Others, pIndex, move, controllers[pIndex].transform.position);
        }
    }

    [RPC]
    void ClientRecvChangeMove(int pIndex, Vector3 move, Vector3 position)
    {
        if (server)
            return;
        Debug.Log("recv ClientRecvChangeMove opcode with " + move);
        if (pIndex >= 0)
        {
            controllers[pIndex].m_force = move;
            controllers[pIndex].transform.position = position;
            Debug.Log("update player " + pIndex);
        }

    }
    [RPC]
    void ServerRecvDropBomb(NetworkPlayer player)
    {
        if (!server)
            return;
        int pIndex;
        if ((pIndex = GetPlayerIndex(player)) >= 0)
        {
            Vector3 cpos =  controllers[pIndex].transform.position;
        }

    }

    [RPC]
    void ClientRecvDropBomb(NetworkPlayer player)
    {
        if (!server)
            return;
        int pIndex;
        if ((pIndex = GetPlayerIndex(player)) >= 0)
        {
            Vector3 cpos = controllers[pIndex].transform.position;
        }

    }

    void HandleBomb(Vector3 pos)
    {
        Network.Instantiate(bomb, pos, Quaternion.identity,0);
    }

    void HandleExplode(IntVector2 pos)
    { 
        m
    }


}
