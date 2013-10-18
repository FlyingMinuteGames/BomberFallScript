using UnityEngine;
using System.Collections;

public class Cross
{
    int x, y, z, w;
    IntVector2 center;
    public Cross(IntVector2 _center,int max_x, int max_y,int min_x, int min_y )
    {
        x = max_x;
        y = max_y;
        z = min_x;
        w = min_y;
        center = _center;
    }

    public bool IsIn(IntVector2 pos)
    {
        if ((pos.x <= x &&  pos.x >= z && pos.y == center.y) || (pos.y <= y && pos.y >= w && pos.x == center.x))
            return true;
        return false;
    }
    public override string ToString()
    {
        return "(" + center + ", " + x + ", " + y + ", " + z + ", " + w + ")";
    }
}
