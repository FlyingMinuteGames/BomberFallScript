using UnityEngine;
using System.Collections;

public class IntVector2 {


    public IntVector2(int x=0,int y=0)
    {
        _x = x;
        _y = y;
    }

    public int _x, _y;

    public int x
    { 
        set { _x=value;}
        get { return _x;}
    }

    public int y
    { 
        set { _y=value;}
        get { return _y;}
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a._x + b._x, a._y + b._y);
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a._x - b._x, a._y - b._y);
    }

    public static IntVector2 operator *(IntVector2 a, int b)
    {
        return new IntVector2(a._x*b,a._y*b);
    }

    public static IntVector2 operator /(IntVector2 a, int b)
    {
        return b == 0 ? a : new IntVector2(a._x / b, a._y / b);
    }

    public string ToString()
    {
        return "(" + _x + ", " + _y + ")";
    }

}
