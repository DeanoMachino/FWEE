using UnityEngine;

[System.Serializable]
public struct Vector2i {
    public int x;
    public int y;

    public Vector2i(int X, int Y) {
        x = X;
        y = Y;
    }
    public Vector2i(Vector2i vector) : this(vector.x, vector.y) { }
    public Vector2i(float X, float Y) : this((int)X, (int)Y) { }
    public Vector2i(Vector2 vector) : this((int)vector.x, (int)vector.y) { }
    public Vector2i(Vector3 vector) : this((int)vector.x, (int)vector.y) { }

    
    // Operator +
    public static Vector2i operator +(Vector2i v1, Vector2i v2) {
        return new Vector2i(v1.x + v2.x, v1.y + v2.y);
    }
    public static Vector2i operator +(Vector2i v1, int v) {
        return new Vector2i(v1.x + v, v1.y + v);
    }

    // Operator -
    public static Vector2i operator -(Vector2i v1, Vector2i v2) {
        return new Vector2i(v2.x - v1.x, v2.y - v1.y);
    }
    public static Vector2i operator -(Vector2i v1, int v) {
        return new Vector2i(v1.x - v, v1.y - v);
    }

    // Operator *
    public static Vector2i operator *(Vector2i v1, Vector2i v2) {
        return new Vector2i(v1.x * v2.x, v1.y * v2.y);
    }
    public static Vector2i operator *(Vector2i v1, int v) {
        return new Vector2i(v1.x * v, v1.y * v);
    }

    // Operator /
    public static Vector2i operator /(Vector2i v1, Vector2i v2) {
        return new Vector2i(v2.x / v1.x, v2.y / v1.y);
    }
    public static Vector2i operator /(Vector2i v1, int v) {
        return new Vector2i(v1.x / v, v1.y / v);
    }

    // Override ToString()
    public override string ToString() {
        return string.Format("Vector2i({0}, {1})", x, y);
    }
}
