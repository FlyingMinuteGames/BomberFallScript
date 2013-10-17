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
    public static string PATH = Application.dataPath;
    /* */

    //
    public static Vector3[] _Vertices = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };
    public static Vector2[] _UV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
    public static Vector3[] _Normal = { new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0) };
    public static int[] _Indices = { 0, 2, 1, 1, 2, 3 };
    //

    private Tile[][] m_maps;
    GameObject m_gameMaps;
    private IntVector2 m_size;
    private IntVector2 m_size_2;
    private string m_name;
    public IntVector2 Size
    {
        get { return m_size;}
        set { m_size = value; m_size_2 = m_size != null? value / 2 : null; /*Generate();*/}
    }
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    

    public Maps(IntVector2 size = null)
    {
        Size = size;
        if (m_gameMaps == null)
        {
            if ((m_gameMaps = GameObject.Find("GameMaps")) == null)
                m_gameMaps = new GameObject("GameMaps");
        }
    }


    public void  Generate()
    {
        if (Size == null) return;
        Clear();
        // create virtual maps
        m_maps = new Tile[Size.x][];
        for (int i = 0; i < Size.x; i++)
        {
            m_maps[i] = new Tile[Size.y];
            for (int j = 0; j < Size.y; j++)
                m_maps[i][j] = new Tile(MapsTiles.EMPTY_TILE,new IntVector2(i,j));
        }

        //create GameObjectMaps

        if(m_gameMaps == null)
        {
            if((m_gameMaps = GameObject.Find("GameMaps")) == null)
                m_gameMaps = new GameObject("GameMaps");
        }
        generateGrid(Size);
        generateGrid(Size + new IntVector2(4, 4), "grid", true, s_grid).transform.position += new Vector3(0, 0.01f, 0) ;
        GenerateBound(Size + new IntVector2(2, 2));
    }

    public void Fill(MapsTiles type)
    {
        IntVector2 p = new IntVector2();
        for (int i = 0; i < m_size.x; i++)
        { 
            p.x = i;
            for(int j = 0; j < m_size.y; j++)
            {
                p.y = j;
                AddBlock(type, p);
            }
        }
    }

    public void AddBlock(MapsTiles type, IntVector2 pos)
    {
        if (m_maps[pos.x][pos.y] != null)
        {

            if (m_maps[pos.x][pos.y].m_type == type)
                return;
            if(m_maps[pos.x][pos.y].block != null)
                GameObject.DestroyImmediate(m_maps[pos.x][pos.y].block.gameObject);
            m_maps[pos.x][pos.y].m_type = type;
            Transform obj = null;
            ;

            Vector3 b_pos = new Vector3(pos.x + (Size.x % 2 == 0 ? 0.5f : 0) - m_size_2.x, 0.5f, pos.y + (Size.y % 2 == 0 ? 0.5f : 0f) - m_size_2.y);
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
                //Debug.Log(i + " "+ j);
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

    public IntVector2 GetTilePosition(Vector2 pos)
    {
        IntVector2 a = new IntVector2((int)(Mathf.Floor(pos.x + (m_size.x % 2 != 0 ? 0.5f : 0f))) + m_size_2.x, (int)(Mathf.Floor(pos.y + (m_size.x % 2 != 0 ? 0.5f : 0f))) + m_size_2.y);
        //a.x = Mathf.Clamp(a.x, 0, m_size.x - 1);
        //a.y = Mathf.Clamp(a.y, 0, m_size.y - 1);
        //Debug.Log("size/2 : " + m_size_2.x + " " + m_size_2.y);

        if (a.x < 0 || a.x >= m_size.x || a.y < 0 || a.y >= m_size.y)
            return null;
        return a;
    }

    public Vector3 NormalizePosition(Vector3 pos, out bool _out)
    {
        IntVector2 tpos = GetTilePosition(new Vector2(pos.x, pos.z));
        _out = false;
        if (tpos == null)
            return pos;
        pos.x = -m_size_2.x + tpos.x + (m_size.x % 2 == 0 ? 0.5f : 0f);
        pos.z = -m_size_2.y + tpos.y + (m_size.y % 2 == 0 ? 0.5f : 0f);
        _out = true;
        return pos;
    }
        
    public void LoadFromFile(string path)
    {
        Debug.Log("load " + path);
        FileStream stream = new FileStream(PATH +"\\"+ path, FileMode.Open);
        int x = stream.ReadByte();
        int y = stream.ReadByte();
        Size = new IntVector2(x, y); 
        Clear();
        Generate();
        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                int type = stream.ReadByte();
                AddBlock((MapsTiles)type, new IntVector2(i,j));
            }
        }
        stream.Close();
    }

    public void SaveToFile(string path)
    {
        if (Size == null)
            return;
        Debug.Log(PATH + "\\" + path);
        FileStream stream = new FileStream(PATH + "\\" + path, FileMode.Create);
        stream.WriteByte((byte)Size.x);
        stream.WriteByte((byte)Size.y);
        for (var i = 0; i < Size.x; i++)
        {
            for (var j = 0; j < Size.y; j++)
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
