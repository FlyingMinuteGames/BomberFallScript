using UnityEngine;
using System.Collections;

public class Tile
{
    public MapsTiles m_type;
    IntVector2 m_position;
    public Transform block;
    public Tile(MapsTiles type, IntVector2 pos)
    {
        m_type = type;
        m_position = pos;
    }
}
