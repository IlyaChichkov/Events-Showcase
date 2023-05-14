using UnityEngine;

public static class StringToVectorParser
{
    public static Vector2 ParseToVector2(string str)
    {
        Debug.Log(str);
        string[] components = str.Split(',');
        if (components.Length != 2) throw new System.FormatException("Invalid Vector2 string format");

        float x, y;
        if (!float.TryParse(components[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x)
            || !float.TryParse(components[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y))
            throw new System.FormatException("Invalid Vector2 string format");


        Debug.Log(x);
        Debug.Log(y);
        return new Vector2(x, y);
    }

    public static Vector3 ParseToVector3(string str)
    {
        string[] components = str.Split(',');
        if (components.Length != 3) throw new System.FormatException("Invalid Vector3 string format");

        float x, y, z;
        if (!float.TryParse(components[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x)
            || !float.TryParse(components[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y)
            || !float.TryParse(components[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out z))
            throw new System.FormatException("Invalid Vector2 string format");

        return new Vector3(x, y, z);
    }

    public static Vector2Int ParseToVector2Int(string str)
    {
        string[] components = str.Split(',');
        if (components.Length != 2) throw new System.FormatException("Invalid Vector2Int string format");

        int x, y;
        if (!int.TryParse(components[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out x)
            || !int.TryParse(components[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out y))
            throw new System.FormatException("Invalid Vector2 string format");

        return new Vector2Int(x, y);
    }

    public static Vector3Int ParseToVector3Int(string str)
    {
        string[] components = str.Split(',');
        if (components.Length != 3) throw new System.FormatException("Invalid Vector3 string format");

        int x, y, z;
        if (!int.TryParse(components[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out x)
            || !int.TryParse(components[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out y)
            || !int.TryParse(components[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out z))
            throw new System.FormatException("Invalid Vector2 string format");

        return new Vector3Int(x, y, z);
    }
}