using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    static Vector3[] _Vertices = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };
    static Vector2[] _UV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
    static Vector3[] _Normal = { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
    static int[] _Indices = { 0, 2, 1, 1, 2, 3 };
    public Material m_material = null;
    public Material m_grid_shader = null;
    Vector2 size = new Vector2(10,10);
    //public GameObject stone = (GameObject)Resources.Load("Stone");
    public GameObject world = null;
    Transform t = null;
    /*void ClearWorld()
    {
        if (world == null)
            return;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in world.transform) 
            children.Add(child.gameObject);
        children.ForEach((child) =>{ DestroyImmediate(child); });
    
    }


    void GenerateBound(Vector2 size)
    {
        Vector2 size_2 = size/2;
        Vector3 init = new Vector3(-size_2.x - 1, 0, -size_2.y - 1);
        int m1 = (int)Mathf.Round(size_2.x), m2 = (int)Mathf.Round(size_2.y);
        for (int i = (int)init.x; i < m1; i++)
        {
            
            for (int j = (int)init.x; j < m2; j++)
            {
                Debug.Log(i + " "+ j);
                if ((i != (int)init.x && i != m1-1) && (j != (int)init.x && j != m2-1))
                    continue;
                Transform o = (Transform)Instantiate(t, new Vector3(i +1f, 0.5f, j +1f), new Quaternion());
                o.parent = world.transform;
            }
        }
    
    }

    GameObject generateGrid(Vector2 size,string name= "Ground",bool uvsized = false,Material mat =null)
    {
        if (world == null)
        {
            if((world = GameObject.Find("World")) == null)
                world = new GameObject("World");
        }
        
        Vector3[] vertice = (Vector3[])_Vertices.Clone();
        Vector2[] uv = (Vector2[])_UV.Clone();
        GameObject _object = new GameObject(name);
        Mesh plane = _object.AddComponent<MeshFilter>().mesh;
        _object.AddComponent<MeshRenderer>();

        for (int i = 0, len = vertice.Length; i < len; i++)
        {
            vertice[i].x = vertice[i].x * size.x;
            vertice[i].z = vertice[i].z * size.y;
            vertice[i] -= vertice[i]/2;

            if (uvsized)
            {
                uv[i].x *= size.y;
                uv[i].y *= size.x;
            }
        }
        plane.vertices = vertice;
        plane.uv = uv;
        plane.triangles = _Indices;
        plane.normals = _Normal;
        _object.renderer.material = mat == null ? m_material : mat;
        _object.transform.parent = world.transform;
        return _object;
    }*/






    [MenuItem("Window/niktamer")]
    public static void ShowWindow()
    {
        LevelEditor editor = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
    }
    
    void OnGUI()
    {
        m_material = (Material)EditorGUILayout.ObjectField("1",m_material, typeof(Material));
        t = (Transform)EditorGUILayout.ObjectField("t", t, typeof(Transform));
        m_grid_shader = (Material)EditorGUILayout.ObjectField("2", m_grid_shader, typeof(Material));
        size = EditorGUILayout.Vector2Field("Grid Size", size);
        if (GUILayout.Button("TEST editor"))
        {
        /*    ClearWorld();
            generateGrid(size);
            generateGrid(size+new Vector2(2,2),"Grid", true, m_grid_shader).transform.position += new Vector3(0, 0.01f, 0);
            GenerateBound(size);*/
        }
    }
}
