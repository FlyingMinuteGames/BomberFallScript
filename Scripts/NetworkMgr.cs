using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum WorldState
{
    CENTER,
    LATERAL_X,
    LATERAL_X2,
    LATERAL_Z,
    LATERAL_Z2
}

public enum MoveState
{ 
    MOVE_FORWARD = 0x1,
    MOVE_BACKWARD = 0x2,
    MOVE_LEFT = 0x4,
    MOVE_RIGHT = 0x8
}

public class NetworkMgr : MonoBehaviour {

    public enum ObjectType
    { 
        OBJECT_PLAYER,
        OBJECT_BLOCK,
        OBJECT_POWERUP,
        OBJECT_BOMB
    }
    
    struct Announcement
    {
        public string[] text;
        public Color color;

        public Announcement(string[] _text,Color c)
        {
            text = _text;
            color = c;
        }

    }

    private static Announcement[] s_announcement = new Announcement[]
    {
        new Announcement(new string[]{"Le joueur","est mort","Vous êtes mort !"},Color.black),
        new Announcement(new string[]{"Changement de plan dans","secondes"},Color.red),
        new Announcement(new string[]{"Changement de plan !!!"},Color.red)
    };


    private float timer_worldchange;
    private string[] player_names =new string[]{"rouge","bleu","vert","orange" };
    private Color[] player_color = { Color.red, Color.blue, Color.green, Color.red + Color.yellow };
    public bool server;
    public int listenPort = 2500;
    public string remoteIP;
    public int playerIndex = 0;
    public GameObject networkController_tpl;
    public GameObject bomb;
    private NetworkPlayer[] _players = new NetworkPlayer[4];
    private NetworkController[] controllers = new NetworkController[4];
    private Maps current_map;
    private IDictionary<int, BombScript> m_bombs = new Dictionary<int, BombScript>();
    private MemoryStream bufferStream = new MemoryStream(); //for map download;
    private GameObject announcer;
    private int player_id = -1;
    private WorldState m_worldstate =  WorldState.CENTER;
    private int moveFlags = 0;
    public Maps Map
    {
        get { return current_map; }
    }
	// Use this for initialization
    private static NetworkMgr s_networkMgr = null;
    public static NetworkMgr Instance
    {
        get{ return s_networkMgr;}
        private set { s_networkMgr = value; }
    }

    Vector3 GetInitPos(int index)
    {
        if (index < 0 || index > 3)
            return new Vector3();
        Vector3 pos  = current_map.TilePosToWorldPos(new IntVector2(index%2 != 0 ? current_map.Size.x - 1 : 0, index > 0 && index < 3 ? current_map.Size.y - 1 : 0));
        return pos;
    }
	void Start () {
        Instance = this;
        Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
        Application.runInBackground = true;
        if (server)
        {
            Network.InitializeSecurity();
            Network.InitializeServer(4, listenPort, true);
            current_map = Maps.LoadMapsFromFile("map1.map");
        }
        else
        {
            Debug.Log("Trying to log to " + remoteIP);
            Network.Connect(remoteIP, listenPort);
        }

        announcer = GameObject.Find("Announcer");
        bomb = ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb");
        timer_worldchange = 6;
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
            Network.Instantiate(networkController_tpl, GetInitPos(index) + new Vector3(0, 0.5f), Quaternion.identity, 1);
            current_map.SaveToStream(bufferStream);
            networkView.RPC("ClientRevcMap", player, bufferStream.ToArray());
            //reset buffer
            bufferStream.Position = 0 ;
            bufferStream.SetLength(0);
            networkView.RPC("ClientRevcInfo", player, index);

            //controllers[index] = obj.GetComponent<NetworkController>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!server)
            UpdateCurrentMove();
        else
        {
            if (timer_worldchange <= 0)
            {
                PlayAnnouncement(2);
                timer_worldchange = 300 + Random.Range(0, 10);
                HandleWorldStateChanged(m_worldstate == WorldState.CENTER ?/*((WorldState)(1+Random.Range(0, 3)))*/ WorldState.LATERAL_Z : WorldState.CENTER);
            }
            else
            {
                if (timer_worldchange > 5 && (timer_worldchange - Time.deltaTime) < 5)
                {
                    PlayAnnouncement(1, 5);
                }
                timer_worldchange -= Time.deltaTime;
            }
        }
	}

    delegate int Callback(NetworkMgr me, bool enable);
    public Vector3 m_force = new Vector3(0, 0, 0);
    private KeyCode[] key_binding = { KeyCode.Z, KeyCode.S, KeyCode.Q, KeyCode.D, KeyCode.Space};
    private Callback[] action_callback = {

                                            /*(me,enable) => { me.m_force += enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.right : Vector3.left; return 1;},
                                            (me,enable) => { me.m_force += enable ? Vector3.right : Vector3.left; return 1;},*/
                                            (me,enable) => { if(me.m_worldstate != WorldState.CENTER ) return (enable ? 2 : 0); me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_FORWARD : me.moveFlags & ~(int)MoveState.MOVE_FORWARD; return 1;},
                                            (me,enable) => { if(me.m_worldstate != WorldState.CENTER ) return 0; me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_BACKWARD : me.moveFlags & ~(int)MoveState.MOVE_BACKWARD; return 1;},
                                            (me,enable) => { me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_LEFT : me.moveFlags & ~(int)MoveState.MOVE_LEFT; return 1;},
                                            (me,enable) => { me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_RIGHT : me.moveFlags & ~(int)MoveState.MOVE_RIGHT; return 1;},
                                            (me,enable) => { if(enable) me.networkView.RPC("ServerRecvDropBomb",RPCMode.Server, Network.player); return 0;}
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
        pl.renderer.material.color = player_color[playerIndex];
        controllers[playerIndex++] = pl;
    }

    void UpdateCurrentMove()
    {
        int flag = 0;
        for (int i = 0, len = key_binding.Length; i < len; i++)
        {
            if (Input.GetKeyDown(key_binding[i]))
                flag |= action_callback[i](this, true);
            else if (Input.GetKeyUp(key_binding[i]))
                flag |= action_callback[i](this, false);
        }
        m_force = Vector3.zero;
        if ((moveFlags & (int)MoveState.MOVE_FORWARD) != 0 && m_worldstate == WorldState.CENTER)
            m_force += Vector3.forward;
        if ((moveFlags & (int)MoveState.MOVE_BACKWARD) != 0 && m_worldstate == WorldState.CENTER)
            m_force -= Vector3.forward;
        if ((moveFlags & (int)MoveState.MOVE_LEFT) != 0)
            m_force += Vector3.left;
        if ((moveFlags & (int)MoveState.MOVE_RIGHT) != 0)
            m_force -= Vector3.left;

        Debug.Log(flag);
        if((flag & 1) != 0)
            this.networkView.RPC("ServerRecvChangeMove", RPCMode.Server, Network.player, m_force.normalized);
        if((flag & 2) != 0)
            this.networkView.RPC("ServerRecvJump", RPCMode.Server, Network.player);
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


    void PlayAnnouncement(int announce, int value1 = -1)
    { 
        if(server)
        {
            networkView.RPC("ClientRevcAnnouncement", RPCMode.Others, announce,value1);
        }
        string msg;
        Color c = Color.red;
        if (announce < s_announcement.Length)
        {
            Announcement ann = s_announcement[announce];
            c = ann.color;
            if (announce == 0)
            {
                int a = value1;
                if (a == player_id)
                    msg = ann.text[2];
                else msg = ann.text[0] +" "+player_names[a]+" "+ ann.text[1];
                c = player_color[a];
            }
            else if (announce == 1)
            {
                msg = ann.text[0] + " "+ value1 + " "+ann.text[1];
            }
            else msg = ann.text[0];
        }
        else msg = "[FAIL] announcement doesn't exist !";
        announcer.GetComponent<TextMesh>().text = msg;
        announcer.GetComponent<TextMesh>().color = c;
        announcer.GetComponent<Animation>().Play();
    }
    
    void HandleBomb(Vector3 pos)
    {
        IntVector2 tpos = current_map.GetTilePosition(pos.x,pos.z);
        Debug.Log("pos:"+tpos);
        pos = current_map.TilePosToWorldPos(tpos);

        Network.Instantiate(bomb, pos+new Vector3(0,0.5f), Quaternion.identity,0);
        networkView.RPC("ClientRecvDropBomb", RPCMode.Others);
    }

    public void HandleExplode(BombScript script, IntVector2 pos, int radius)
    {
        Debug.Log("ça pete !");
        current_map.ExplodeAt(pos, radius);
        networkView.RPC("ClientRecvBomb", RPCMode.Others, pos.x, pos.y, radius);
        Network.RemoveRPCs(script.gameObject.GetComponent<NetworkView>().viewID);
        Network.Destroy(script.gameObject);
    }

    public void HandleKillPlayer(Cross c)
    {
        Debug.Log(c);
        for(int i = 0, len = controllers.Length; i <len; i++)
        {
            NetworkController t = controllers[i];
            if (t == null)
                continue;
            IntVector2 tpos = current_map.GetTilePosition(t.transform.position.x, t.transform.position.z);
            Debug.Log(tpos);
            if (c.IsIn(tpos))
               PlayAnnouncement(0,i);
        }
    }

    public void HandleWorldStateChanged(WorldState state)
    {
        m_worldstate = (WorldState)state;
        if (server)
            networkView.RPC("ClientRecvChangeWorldState", RPCMode.Others, (int)state);

        switch (state)
        { 
            case WorldState.CENTER:
                Physics.gravity = Vector3.down * Config.CONST_GRAVITY * Config.CONST_FACTOR;
                break;
            case WorldState.LATERAL_X:
                Physics.gravity = Vector3.forward * Config.CONST_GRAVITY * Config.CONST_FACTOR;
                break;
            case WorldState.LATERAL_X2:
                Physics.gravity = -Vector3.back * Config.CONST_GRAVITY * Config.CONST_FACTOR;
                break;
            case WorldState.LATERAL_Z:
                Physics.gravity = Vector3.back * Config.CONST_GRAVITY * Config.CONST_FACTOR;
                break;
            case WorldState.LATERAL_Z2:
                Physics.gravity = -Vector3.back * Config.CONST_GRAVITY * Config.CONST_FACTOR;
                break;
        
        }
    }

    public void RegisterObj(Object o, ObjectType type)
    {
        
        if (type == ObjectType.OBJECT_BOMB && server)
        {
            BombScript script = ((BombScript)o);
            m_bombs[o.GetInstanceID()] = script;

            script.callback = () =>
            {
                Vector3 pos = script.transform.position;
                IntVector2 tpos = current_map.GetTilePosition(pos.x, pos.z);
                this.HandleExplode(script, tpos, 1);
            };
        }
    
    }



    /*
       Opcode parts
     */

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
            Vector3 cpos = controllers[pIndex].transform.position;
            HandleBomb(cpos);
        }

    }

    [RPC]
    void ClientRecvDropBomb()
    {
        if (!server)
            return;
    }


    [RPC]
    void ClientRecvBomb(int x, int y, int radius)
    {
        if (server)
            return;
        current_map.ExplodeAt(new IntVector2(x, y), radius);
    }

    [RPC]
    void ClientRevcMap(byte[] maps)
    {
        current_map = Maps.LoadMapsFromStream(new MemoryStream(maps));
    }

    [RPC]
    void ClientRevcInfo(int _player_id)
    {
        player_id = _player_id;
    }

    [RPC]
    void ClientRevcAnnouncement(int announce, int value1)
    {
        PlayAnnouncement(announce, value1);
    }

    [RPC]
    void ClientRecvChangeWorldState(int worldState)
    {
        HandleWorldStateChanged((WorldState)worldState);
        moveFlags &= ~((int)MoveState.MOVE_FORWARD | (int)MoveState.MOVE_BACKWARD);
        
    }

    [RPC]
    void ServerRecvJump(NetworkPlayer player)
    {
        if (!server)
            return;
        Debug.Log("recv ServerRecvJump opcode");
        int pIndex;
        if ((pIndex = GetPlayerIndex(player)) >= 0)
        {
            if (controllers[pIndex].CanJump)
            {
                controllers[pIndex].Jump();
                networkView.RPC("ClientRecvJump", RPCMode.Others, pIndex);
            }
            else Debug.Log("but he can't jump :(");
        }
    }

    [RPC]
    void ClientRecvJump(int pIndex)
    {
        Debug.Log("recv jump opcode");
        if (server)
            return;
        if (pIndex >= 0)
        {
            controllers[pIndex].Jump();
        }

    }




}
