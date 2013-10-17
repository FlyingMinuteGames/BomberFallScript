using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelEditorMenu : EditorWindow
{
    public Material m_material = null;
    public Material m_grid_shader = null;
    Vector2 size = new Vector2(10, 10);
    public GameObject world = null;
    public Maps maps = null;
    public Transform t;
    public Transform b;
    public Vector2 pos;
    MapsTiles type;
    public string path;
    [MenuItem("Window/TEST")]

    public static void ShowWindow()
    {
        LevelEditorMenu editor = (LevelEditorMenu)EditorWindow.GetWindow(typeof(LevelEditorMenu));
    }

    void OnGUI()
    {
        wantsMouseMove = true;
        m_material = (Material)EditorGUILayout.ObjectField("1", m_material, typeof(Material));
        m_grid_shader = (Material)EditorGUILayout.ObjectField("2", m_grid_shader, typeof(Material));
        t = (Transform)EditorGUILayout.ObjectField("Stone template", t, typeof(Transform));
        b = (Transform)EditorGUILayout.ObjectField("Breakable template", b, typeof(Transform));
        size = EditorGUILayout.Vector2Field("Grid Size", size);

        if (GUILayout.Button("generate maps"))
        {
            Maps.s_material = m_material;
            Maps.s_grid = m_grid_shader;
            Maps.s_stone = t;
            Maps.s_breakable = b;
            if(maps !=null)
                maps.Clear();
            maps = new Maps(new IntVector2((int)size.x, (int)size.y));
            maps.Generate();
        }
        pos = (Vector2)EditorGUILayout.Vector2Field("position : ",pos);
        type = (MapsTiles)EditorGUILayout.EnumPopup("type : ",type);
        //Debug.Log("try to add a block");
        if (GUILayout.Button("add block to maps"))
        {
            Debug.Log("try to add a block");
            if (maps != null)
            {
                Debug.Log("add "+ type + " block to maps");
                maps.AddBlock(type,new IntVector2((int)pos.x,(int)pos.y));

            }
        }

        path =  EditorGUILayout.TextField("Path :", path);
        if (GUILayout.Button("Save to"))
        {
            maps.SaveToFile(path);
        }

        if (GUILayout.Button("load to"))
        {
            maps = Maps.LoadMapsFromFile(path);
        }
        


    }
}
