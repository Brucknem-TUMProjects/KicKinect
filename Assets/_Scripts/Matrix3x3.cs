using UnityEngine;

public class Matrix3x3
{
    Vector3 column1, column2, column3;
    bool transposed;
    
    public Matrix3x3()
    {
        transposed = false;
    }
    public Matrix3x3(bool transposed)
    {
        this.transposed = transposed;
    }
    public Matrix3x3(Vector3 column1, Vector3 column2, Vector3 column3)
    {
        this.column1 = column1;
        this.column2 = column2;
        this.column3 = column3;
    }
    public Matrix3x3(Vector3 column1, Vector3 column2, Vector3 column3, bool transposed)
    {
        this.column1 = column1;
        this.column2 = column2;
        this.column3 = column3;
        this.transposed = transposed;
    }
    public Matrix3x3(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33)
    {
        this.column1 = new Vector3(a11, a21, a31);
        this.column2 = new Vector3(a12, a22, a32);
        this.column3 = new Vector3(a13, a23, a33);
        this.transposed = false;
    }
    public Matrix3x3(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33, bool transposed)
    {
        this.column1 = new Vector3(a11, a21, a31);
        this.column2 = new Vector3(a12, a22, a32);
        this.column3 = new Vector3(a13, a23, a33);
        this.transposed = transposed;
    }

    public Matrix3x3 Clone()
    {
        return new Matrix3x3(new Vector3(column1.x, column1.y, column1.z), new Vector3(column2.x, column2.y, column2.z), new Vector3(column3.x, column3.y, column3.z), transposed);
    }
    public override string ToString()
    {
        return column1.ToString() + " | " + column2.ToString() + " | " + column3.ToString();
    }

    public void Set(int row, int column, float value)
    {
        if (transposed)
        {
            int tmp = row;
            row = column;
            column = tmp;
        }
        Vector3 tochange = (column == 0 ? column1 : column == 1 ? column2 : column == 2 ? column3 : new Vector3());
        if (row == 0)
            tochange.x = value;
        if (row == 1)
            tochange.y = value;
        if (row == 2)
            tochange.z = value;
    }
    public float Get(int row, int column)
    {
        if (transposed)
        {
            int tmp = row;
            row = column;
            column = tmp;
        }
        Vector3 toget = (column == 0 ? column1 : column == 1 ? column2 : column == 2 ? column3 : Vector3.zero);
        if (row == 0)
            return toget.x;
        if (row == 1)
            return toget.y;
        if (row == 2)
            return toget.z;
        return 0f;
    }
    public Vector3 Column(int at)
    {
        return transposed ? (
            at == 0 ? new Vector3(column1.x, column2.x, column3.x) :
            at == 1 ? new Vector3(column1.y, column2.y, column3.y) :
            at == 2 ? new Vector3(column1.z, column2.z, column3.z) :
            new Vector3()
            ) : (
            at == 0 ? column1 :
            at == 1 ? column2 :
            at == 2 ? column3 :
            new Vector3()
            );
    }
    public Vector3 Row(int at)
    {
        Matrix3x3 t = Clone();
        t.transposed = !transposed;
        return t.Column(at);
    }

    public static readonly Matrix3x3 identity = new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    public static readonly Matrix3x3 zero = new Matrix3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

    public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
    {
        Vector3 m2c1 = m2.transposed ? new Vector3(m2.column1.x, m2.column2.x, m2.column3.x) : m2.column1;
        Vector3 m2c2 = m2.transposed ? new Vector3(m2.column1.y, m2.column2.y, m2.column3.y) : m2.column1;
        Vector3 m2c3 = m2.transposed ? new Vector3(m2.column1.z, m2.column2.z, m2.column3.z) : m2.column1;

        return new Matrix3x3(m1*m2c1, m1*m2c2, m1*m2c3);
    }
    public static Vector3 operator *(Matrix3x3 m, Vector3 v)
    {
        if (!m.transposed)
            return new Vector3(
                m.column1.x * v.x + m.column2.x * v.y + m.column3.x * v.z,
                m.column1.y * v.x + m.column2.y * v.y + m.column3.y * v.z,
                m.column1.z * v.x + m.column2.z * v.y + m.column3.z * v.z
                );
        else
            return new Vector3(Vector3.Dot(m.column1, v), Vector3.Dot(m.column2, v), Vector3.Dot(m.column3, v));
    }
    public static Matrix3x3 operator *(Matrix3x3 m, float c)
    {
        Matrix3x3 r = m.Clone();
        r.column1 *= c;
        r.column2 *= c;
        r.column3 *= c;
        return r;
    }
    public static Matrix3x3 operator +(Matrix3x3 m1, Matrix3x3 m2)
    {
        float[] a = new float[9];
        for(int row = 0; row < 3; row++)
        {
            for(int column = 0; column < 3; column++)
            {
                a[3 * row + column] = m1.Get(row, column) + m2.Get(row, column);
            }
        }
        return new Matrix3x3(a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8]);
    }
}