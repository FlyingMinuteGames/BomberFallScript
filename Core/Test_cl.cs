using UnityEngine;
using System.Collections;

public class Test_cl {


    public Test_cl(int x, int y)
    {
        _x = x;
        _y = y;
    }


    public int _x;
    public int _y;
    public int x
    {
        get { return _x; }
        set { _x=value;}
    }

    public int y
    {
        get { return _y; }
        set { _y=value;}
    }
/*
    public static  Test_cl operator +( Test_cl a,  Test_cl b)
    {
        return new  Test_cl(a.x + b.x, a.y + b.y);
    }

    public static  Test_cl operator -( Test_cl a,  Test_cl b)
    {
        return new  Test_cl(a.x - b.x, a.y - b.y);
    }

    public static  Test_cl operator *( Test_cl a, int b)
    {
        return new  Test_cl(a.x*b,a.y*b);
    }

    public static  Test_cl operator /( Test_cl a, int b)
    {
        return b == 0 ? a : new  Test_cl(a.x / b, a.y / b);
    }*/
}
