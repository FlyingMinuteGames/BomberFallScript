using UnityEngine;
using System.Collections;

public class StartNetwork : MonoBehaviour {

    public bool server;
    public int listenPort = 2500;
    public string remoteIP;
    public GameObject _player1GO;
    public GameObject _player2GO;
    public GameObject _bombPrefab;

    private Transform _player1Transform;
    private Transform _player2Transform;

    private Vector3 _currentPlayer1Direction = Vector3.zero;
    private Vector3 _currentPlayer2Direction = Vector3.zero;

    private NetworkPlayer _player1;
    private NetworkPlayer _player2;

    public float moveSpeed = 10F;


	// Use this for initialization
	void Start () {
        Application.runInBackground = true;

        _player1GO = GameObject.Find("Player1");
        _player2GO = GameObject.Find("Player2");
        if (server)
        {
            Network.InitializeSecurity();
            Network.InitializeServer(2, listenPort, true);

        }
        else
        {
            Debug.Log("Trying to log to "+ remoteIP);
            Network.Connect(remoteIP, listenPort);
        }
            _player1Transform = _player1GO.transform;
            _player2Transform = _player2GO.transform;

	}
	
    void OnPlayerConnected(NetworkPlayer player)
    {
         if(server)
         {
             Debug.Log("A player has connected !");
             if (Network.connections.Length == 1)
             {
                 _player1 = player;
                 _player1GO.renderer.material.color = Color.red;
             }
             if (Network.connections.Length == 2)
             {
                 _player2 = player;
                 _player2GO.renderer.material.color = Color.blue;
             }
         }
    }


	// Update is called once per frame
	void Update () {

        if (Network.isServer)
        {
            MovePad(_player1Transform, _currentPlayer1Direction);
            MovePad(_player2Transform, _currentPlayer2Direction);
            networkView.RPC("UpdatePlayersPos", RPCMode.AllBuffered, _player1Transform.position, _player2Transform.position);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.networkView.RPC("ClientBeginMoveDown", RPCMode.Server, Network.player);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                this.networkView.RPC("ClientStopMove", RPCMode.Server, Network.player);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.networkView.RPC("ClientBeginMoveUp", RPCMode.Server, Network.player);
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                this.networkView.RPC("ClientStopMove", RPCMode.Server, Network.player);
            }

        }
    }

    [RPC]
    void ClientBeginMoveUp(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            if (player == _player1)
                _currentPlayer1Direction = Vector3.forward;

            if (player == _player2)
                _currentPlayer2Direction = Vector3.forward;
        }
    }
    [RPC]
    void ClientBeginMoveDown(NetworkPlayer player)
    {

        if (Network.isServer)
        {
            if (player == _player1)
                _currentPlayer1Direction = Vector3.back;

            if (player == _player2)
                _currentPlayer2Direction = Vector3.back;
        }
    }
    [RPC]
    void ClientStopMove(NetworkPlayer player)
    {
        if (Network.isServer)
        {
            if (player == _player1)
                _currentPlayer1Direction = Vector3.zero;

            if (player == _player2)
                _currentPlayer2Direction = Vector3.zero;
        }
    }

    [RPC]
    void UpdatePlayersPos(Vector3 player1Pos, Vector3 player2Pos)
    {
        if (Network.isClient)
        {
            _player1Transform.position = player1Pos;
            _player2Transform.position = player2Pos;
        }
    }


    void MovePad(Transform transform, Vector3 direction)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

}
