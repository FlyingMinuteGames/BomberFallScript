using UnityEngine;
using System.Collections;

public class GuiUtils {

    public static Rect ResizeGUI(Rect _rect, bool uniformScale = false)
    {
        //Debug.Log(Screen.width);
        Vector2 scale = new Vector2(Screen.width / 800.0f, Screen.height / 600.0f);
        if (uniformScale)
            scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
        float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = _rect.height * scale.y;
        return new Rect(rectX, rectY, rectWidth, rectHeight);

    }

    public static Rect ResizeGUICenter(Rect _rect, bool uniformScale = false)
    {
        Vector2 scale = new Vector2(Screen.width / 800.0f, Screen.height / 600.0f);
        if (uniformScale)
            scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
        float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = _rect.height * scale.y;
        return new Rect(rectX, rectY, rectWidth, rectHeight);

    }
}
