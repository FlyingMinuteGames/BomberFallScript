using UnityEditor;
using UnityEngine;
public class CustomMenuScript : MonoBehaviour
{

    [MenuItem("Custom/Hide Walls %h")]
    static void HideWalls()
    {
        GameObject walls = GameObject.Find("Walls");
        if (walls)
        {
            foreach (Transform child in walls.transform)
                child.GetComponent<Renderer>().enabled = false;
            Debug.Log("Hiding Walls...");
        }
    }

    [MenuItem("Custom/Show Walls %g")]
    static void ShowWalls()
    {
        GameObject walls = GameObject.Find("Walls");
        if (walls)
        {
            foreach (Transform child in walls.transform)
                child.GetComponent<Renderer>().enabled = true;
            Debug.Log("Showing Walls...");
        }
    }
	
	[MenuItem("Custom/Select Spawn %#u")]
    static void SetPlayerPosition()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = Selection.activeTransform.gameObject.transform.position;
    }
		
    [MenuItem ("Custom/Select Spawn", true)]
    static bool ValidateSetPlayerPosition () {
        return Selection.activeTransform.gameObject.CompareTag("checkpoint");
    }

}