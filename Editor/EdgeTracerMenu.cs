using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EdgeTracerMenu : EditorWindow
{
    private Mesh m; // Mesh to edgeTracer
    public Material m_material = null; // LineRenderer Material
    public Vector3 m_Offset = new Vector3();
    public Vector3 m_Center = new Vector3();
    public Vector3 m_Scale = new Vector3(1, 1, 1);
    public bool m_AutoPlacementWithNormal = false;
    public float m_AutoPlacementFactor = 0.2f;
    public float m_Width = 0.2f;
    public float m_AngleDiscard = 0.2f;
    private List<Edge> m_edges;
    private List<Face> m_faces;

    class Face
    {
        Vector3[] vertex = new Vector3[3];

        public Face()
        {
            for (int i = 0; i < 3; i++)
                vertex[i] = new Vector3();
        }

        public Face(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            vertex[0] = new Vector3(v1.x, v1.y, v1.z);
            vertex[1] = new Vector3(v2.x, v2.y, v2.z);
            vertex[2] = new Vector3(v3.x, v3.y, v3.z);
        }

        public Vector3 normal()
        {
            Vector3 v1 = (vertex[1] - vertex[0]).normalized;
            Vector3 v2 = (vertex[2] - vertex[0]).normalized;
            return Vector3.Normalize(Vector3.Cross(v1, v2));
        }

        public Edge[] getEdges()
        {
            Edge[] _edges = new Edge[3];
            _edges[0] = new Edge(vertex[0], vertex[1]);
            _edges[0].AddFace(this);
            _edges[1] = new Edge(vertex[0], vertex[2]);
            _edges[1].AddFace(this);
            _edges[2] = new Edge(vertex[1], vertex[2]);
            _edges[2].AddFace(this);
            return _edges;
        }
    }


    class Edge
    {
        public Vector3 start;
        public Vector3 end;

        public static bool NearZero(Vector3 a)
        {
            if (Mathf.Abs(a.x) > 0.01f || Mathf.Abs(a.y) > 0.01f || Mathf.Abs(a.z) > 0.01f)
                return false;
            return true;

        }

        public static bool Compare(Edge a, Edge b)
        {
            Vector3 a1 = a.start - b.start;
            Vector3 a2 = a.end - b.end;
            Vector3 a3 = a.start - b.end;
            Vector3 a4 = a.end - b.start;

            if (NearZero(a1) && NearZero(a2) || NearZero(a3) && NearZero(a4))
                return true;
            return false;
        }

        public Face[] linkedFace = new Face[2];

        public Edge()
        { }

        public Edge(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }

        public void AddFace(Face f)
        {
            if (linkedFace[0] == null)
            {
                linkedFace[0] = f;
                return;
            }

            if (linkedFace[1] == null)
                linkedFace[1] = f;
        }
    }


    void Generate(GameObject target)
    {
        if (target == null)
            return;
        DeleteEdge(target);
        if (target.transform.gameObject.GetComponent<MeshFilter>() != null)
            m = target.transform.gameObject.GetComponent<MeshFilter>().mesh;
        else return;

        m_edges = new List<Edge>();
        m_faces = new List<Face>();
        Debug.Log("Generate Edge Renderer for mesh with " + m.vertices.Length + " vertices, " + m.triangles.Length + " triangles");
        GenerateFaces();
        GenerateEdges();
        GenerateLine(target);
        m_edges = null;
        m_faces = null;
    }

    private void GenerateFaces()
    {
        int max = m.triangles.Length / 3;
        Debug.Log(max);
        for (int i = 0; i < max; i++)
        {
            Face f = new Face(m.vertices[m.triangles[i * 3]], m.vertices[m.triangles[i * 3 + 1]], m.vertices[m.triangles[i * 3 + 2]]);
            m_faces.Add(f);
        }

    }

    private void GenerateEdges()
    {
        foreach (Face f in m_faces)
        {
            Edge[] edges = f.getEdges();
            foreach (Edge e in edges)
                AppendEdge(e);
        }
    }

    private void AppendEdge(Edge e)
    {
        Edge s = m_edges.Find(
            delegate(Edge a)
            {
                return Edge.Compare(a, e);
            });
        if (s != null)
        {
            s.AddFace(e.linkedFace[0]);
        }
        else
            m_edges.Add(e);
    }

    Vector3 CalcAutoPlacement(Edge e)
    {
        if (!m_AutoPlacementWithNormal)
            return new Vector3();
        if (e.linkedFace[1] == null)
            return e.linkedFace[0].normal() * m_AutoPlacementFactor;

        Vector3 normal = e.linkedFace[0].normal() + e.linkedFace[1].normal();
        return normal.normalized * m_AutoPlacementFactor;
    }

    private Vector3 CalcPosition(Vector3 v, Transform transform)
    {
        return transform.position + transform.rotation * (new Vector3(v.x * transform.localScale.x * m_Scale.x, v.y * transform.localScale.y * m_Scale.y, v.z * transform.localScale.z * m_Scale.z));
    }

    private void GenerateLine(GameObject target)
    {
        for (int i = 0; i < m_edges.Count; i++)
        {
            if (m_edges[i].linkedFace[1] != null)
            {
                float a = Mathf.Abs(Vector3.Dot(m_edges[i].linkedFace[0].normal(), m_edges[i].linkedFace[1].normal()));
                if (a > m_AngleDiscard)
                    continue;
            }

            GameObject edgeRenderer = new GameObject("EdgeRenderer" + i);
            LineRenderer lineRenderer = edgeRenderer.AddComponent<LineRenderer>();
            lineRenderer.SetColors(Color.black, Color.black);

            lineRenderer.SetPosition(0, CalcPosition(m_edges[i].start + m_Offset + CalcAutoPlacement(m_edges[i]),target.transform));
            lineRenderer.SetPosition(1, CalcPosition(m_edges[i].end + m_Offset + CalcAutoPlacement(m_edges[i]),target.transform));
            lineRenderer.SetWidth(m_Width, m_Width);
            edgeRenderer.transform.parent = target.transform;
            edgeRenderer.transform.localPosition = new Vector3();
            edgeRenderer.transform.localRotation = new Quaternion();
            lineRenderer.material = m_material;
        }
    }

    void DeleteEdge(GameObject target)
    {
        if (target == null)
            return;


       for(int i = 0; i < target.transform.childCount; i++)
       {
            Transform t = target.transform.GetChild(i);
            if (t.name.StartsWith("EdgeRenderer"))
            {
                GameObject.DestroyImmediate(t.gameObject);
                i--;
            }
            
       }
    }

    void UpdateMaterial(GameObject target)
    {
        foreach (Transform t in target.transform)
        {
            if(t.name.StartsWith("EdgeRenderer"))
                t.gameObject.GetComponent<LineRenderer>().material = m_material;
        }
    }



    [MenuItem("Window/EdgeTracer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EdgeTracerMenu));
    }


    void OnGUI()
    {
        m_material = (Material)EditorGUILayout.ObjectField(m_material, typeof(Material));
        float angle = (1.0f - m_AngleDiscard) * 90f;
        angle = EditorGUILayout.Slider("Angle discard",angle, 0, 90);
        m_AngleDiscard = 1.0f - angle / 90.0f;
        m_Offset = EditorGUILayout.Vector3Field("Offset",m_Offset);
        m_Center = EditorGUILayout.Vector3Field("Center", m_Center);
        m_Scale = EditorGUILayout.Vector3Field("Scale", m_Scale);
        m_AutoPlacementWithNormal = GUILayout.Toggle(m_AutoPlacementWithNormal, "Auto Placement with normal");
        m_AutoPlacementFactor = EditorGUILayout.FloatField("Auto placement factor",m_AutoPlacementFactor);
        m_Width = EditorGUILayout.FloatField("Edge width", m_Width);
        if (GUILayout.Button("Generate edge renderer"))
        {
            Generate(Selection.activeGameObject);
        }
        if (GUILayout.Button("Update material on edge"))
        {
            UpdateMaterial(Selection.activeGameObject);
        }
        if (GUILayout.Button("Delete all edge renderer"))
        {
            DeleteEdge(Selection.activeGameObject);
        }
    }

}