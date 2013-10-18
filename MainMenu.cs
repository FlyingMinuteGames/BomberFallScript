using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

public GameObject networkManager; // Prefab

private GameObject instantiatedMaster; //Prefab instancié
private NetworkMgr scriptStartNet;

private string serverIP = "127.0.0.1";
private int serverPort = 25000;

    void OnGUI()
    {
        
        int menuSizeX  = 460;
        int menuSizeY = 150;
        float menuPosX = 20;
        float menuPosY = Screen.height/2 - menuSizeY/2;
        Rect mainMenu = new Rect(menuPosX, menuPosY, menuSizeX, menuSizeY);
        int sizeButtonX = 250;
        int sizeButtonY = 30;
 
        //Le menu de base
        GUI.BeginGroup(mainMenu, "");
        GUI.Box(new Rect(0,0,menuSizeX, menuSizeY), "");
 
        //La demande de champs d'ip pour rejoindre un serveur
        serverIP = GUI.TextField(new Rect(sizeButtonX + 30, 60, 120, 30), serverIP, 40);
 
        if ( GUI.Button(new Rect(10, 20, sizeButtonX, sizeButtonY), "Host"))
        {
            //Création du serveur
            instantiatedMaster = (GameObject)Instantiate(networkManager, Vector3.zero, Quaternion.identity);
            instantiatedMaster.name = "NetworkManager";
            scriptStartNet = instantiatedMaster.GetComponent<NetworkMgr>();
            scriptStartNet.server = true;
            scriptStartNet.listenPort = serverPort;
            Destroy(this);
        }
        if ( GUI.Button(new Rect(10, 60, sizeButtonX, sizeButtonY), "Join"))
        {
            //Rejoindre serveur
            instantiatedMaster = (GameObject)Instantiate(networkManager, Vector3.zero, Quaternion.identity);
            instantiatedMaster.name = "NetworkManager";
            scriptStartNet = instantiatedMaster.GetComponent<NetworkMgr>();
            scriptStartNet.server = false;
            scriptStartNet.remoteIP = serverIP;
            scriptStartNet.listenPort = serverPort;
            Debug.Log("Main menu remote IP "+serverIP);
            Destroy(this);

        }
        if (GUI.Button(new Rect(10, 100, sizeButtonX, sizeButtonY), "Test maps"))
        {
            Maps maps = Maps.LoadMapsFromFile("map1.map");
            Destroy(this);
        }
        
        GUI.EndGroup();
    }




	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
