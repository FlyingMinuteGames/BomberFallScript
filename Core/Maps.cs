using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public enum MapsTiles
{
    EMPTY_TILE,
    SOLID_BLOCK,
    DESTRUCTIBLE_BLOCK
}
public class Maps {

    /* Public static variable */

    public static Material s_material;
    public static Material s_grid;
    public static Transform s_stone; // stone template
    public static Transform s_breakable; // stone template
    public static string PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    /* */

    //
    static Vector3[] _Vertices = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };
    static Vector2[] _UV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
    static Vector3[] _Normal = { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
    static int[] _Indices = { 0, 2, 1, 1, 2, 3 };
    //

    private Tile[][] m_maps;
    GameObject m_gameMaps;
    private IntVector2 m_size;
    private string m_name;
    public IntVector2 Size
    {
        get { return m_size;}
        private set { m_size = value; /*Generate();*/}
    }
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    

    public Maps(IntVector2 size = null)
    {
        m_size = size;
    }


    public void  Generate()
    {
        if (m_size == null) return;
        Clear();
        // create virtual maps
        m_maps = new Tile[m_size.x][];
        for(int i = 0; i < m_size.x; i++)
        {
            m_maps[i] = new Tile[m_size.y];
            for(int j = 0; j < m_size.y; j++)
                m_maps[i][j] = new Tile(MapsTiles.EMPTY_TILE,new IntVector2(i,j));
        }

        //create GameObjectMaps

        if(m_gameMaps == null)
        {
            if((m_gameMaps = GameObject.Find("GameMaps")) == null)
                m_gameMaps = new GameObject("GameMaps");
        }
        generateGrid(m_size);
        generateGrid(m_size+new IntVector2(4,4),"grid",true,s_grid);
        GenerateBound(m_size + new IntVector2(2, 2));
    }


    public void add(MapsTiles type, IntVector2 pos)
    {
        if (m_maps[pos.x][pos.y] != null)
        {

            if (m_maps[pos.x][pos.y].m_type == type)
                return;
            if(m_maps[pos.x][pos.y].block != null)
                GameObject.DestroyImmediate(m_maps[pos.x][pos.y].block.gameObject);
            m_maps[pos.x][pos.y].m_type = type;
            Transform obj = null;
            IntVector2 size_2 = m_size / 2;

            Vector3 b_pos = new Vector3(pos.x + (m_size.x % 2 == 0 ? 0.5f : 1f) - size_2.x, 0.5f, pos.y + (m_size.y % 2 == 0 ? 0.5f : 1f) - size_2.y);
            switch (type)
            { 
                case MapsTiles.SOLID_BLOCK:
                    obj = (Transform)Transform.Instantiate(s_stone, b_pos, new Quaternion());
                    break;
                case MapsTiles.DESTRUCTIBLE_BLOCK:
                    obj = (Transform)Transform.Instantiate(s_breakable, b_pos, new Quaternion());
                    break;
            }
            if (obj != null)
                obj.parent = m_gameMaps.transform;
            
            m_maps[pos.x][pos.y].block = obj;
        }
    }

    GameObject generateGrid(IntVector2 size,string name= "Ground",bool uvsized = false,Material mat =null)
    {       
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
        _object.renderer.material = mat == null ? s_material : mat;
        _object.transform.parent = m_gameMaps.transform;
        return _object;
    }

    void GenerateBound(IntVector2 size)
    {
        IntVector2 size_2 = size/ 2;
        Vector3 init = new Vector3(-size_2.x - 1, 0, -size_2.y - 1);
        int m1 = size.x%2 == 0 ? size_2.x : size_2.x+1, m2 = size.y%2 == 0 ? size_2.y : size_2.y+1;
        for (int i = -size_2.x; i < m1; i++)
        {

            for (int j = -size_2.y; j < m2; j++)
            {
                Debug.Log(i + " "+ j);
                if ((i != -size_2.x && i != m1 - 1) && (j != -size_2.y && j != m2 - 1))
                    continue;
                Transform o = (Transform)GameObject.Instantiate(s_stone, new Vector3(i + (size.x % 2 == 0 ? 0.5f : 0f), 0.5f, j+(size.y % 2 == 0 ? 0.5f : 0f)), new Quaternion());
                o.parent = m_gameMaps.transform;
            }
        }
    
    }


    public void Clear()
    {
        
        m_maps = null;
        if (m_gameMaps == null)
            return;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in m_gameMaps.transform)
            children.Add(child.gameObject);
        children.ForEach((child) => { Transform.DestroyImmediate(child); });
    }
    

    public void LoadFromFile(string path)
    {
        FileStream stream = new FileStream(PATH +"\\"+ path, FileMode.Open);
        int x = stream.ReadByte();
        int y = stream.ReadByte();
        m_size = new IntVector2(x, y); 
        Clear();
        Generate();
        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                int type = stream.ReadByte();
                add((MapsTiles)type, new IntVector2(i,j));
            }
        }
        stream.Close();
    }

    public void SaveToFile(string path)
    {
        if (m_size == null)
            return;
        Debug.Log(PATH + "\\" + path);
        FileStream stream = new FileStream(PATH + "\\" + path, FileMode.Create);
        stream.WriteByte((byte)m_size.x);
        stream.WriteByte((byte)m_size.y);
        for (var i = 0; i < m_size.x; i++)
        {
            for (var j = 0; j < m_size.y; j++)
            {
                stream.WriteByte((byte)m_maps[i][j].m_type);
            }
        }
        stream.Close();
   } 

    public static Maps LoadMapsFromFile(string path)
    {
        Maps m = new Maps();
        m.LoadFromFile(path);
        return m;
    }
}
