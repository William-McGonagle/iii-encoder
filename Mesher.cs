using System;
using System.Collections.Generic;
using DelaunatorSharp;

public struct Mesh
{

    public string name;
    public int[] triangles;
    public float[] vertices;

}

public static class Mesher
{

    public static FileData toFileData(Mesh input)
    {

        FileData output = new FileData(input.name);
        List<Point> points = new List<Point>();
        int triangleCount = input.triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {

            float A_x = input.vertices[input.triangles[i * 3 + 0] * 6 + 0];
            float A_y = input.vertices[input.triangles[i * 3 + 0] * 6 + 1];
            float A_r = input.vertices[input.triangles[i * 3 + 0] * 6 + 3];
            float A_g = input.vertices[input.triangles[i * 3 + 0] * 6 + 4];
            float A_b = input.vertices[input.triangles[i * 3 + 0] * 6 + 5];

            float B_x = input.vertices[input.triangles[i * 3 + 1] * 6 + 0];
            float B_y = input.vertices[input.triangles[i * 3 + 1] * 6 + 1];
            float B_r = input.vertices[input.triangles[i * 3 + 1] * 6 + 3];
            float B_g = input.vertices[input.triangles[i * 3 + 1] * 6 + 4];
            float B_b = input.vertices[input.triangles[i * 3 + 1] * 6 + 5];

            float C_x = input.vertices[input.triangles[i * 3 + 2] * 6 + 0];
            float C_y = input.vertices[input.triangles[i * 3 + 2] * 6 + 1];
            float C_r = input.vertices[input.triangles[i * 3 + 2] * 6 + 3];
            float C_g = input.vertices[input.triangles[i * 3 + 2] * 6 + 4];
            float C_b = input.vertices[input.triangles[i * 3 + 2] * 6 + 5];

            if (!(A_x == B_x || B_x == C_x || C_x == A_x) && !(A_y == B_y || B_y == C_y || C_y == A_y))
            {

                points.Add(new Point(A_x, A_y, A_r, A_g, A_b));
                points.Add(new Point(B_x, B_y, B_r, B_g, B_b));
                points.Add(new Point(C_x, C_y, C_r, C_g, C_b));

            }

        }

        output.points = points.ToArray();
        return output;

    }

    public static Mesh fromFileData(FileData input)
    {

        Mesh output = new Mesh();
        output.name = input.name;

        IPoint[] points = new IPoint[input.points.Length];
        float[] vertices = new float[input.points.Length * 6];

        for (int i = 0; i < input.points.Length; i++)
        {

            points[i] = new DelaunatorSharp.Point(input.points[i].x, input.points[i].y);

            vertices[(i * 6) + 0] = input.points[i].x;
            vertices[(i * 6) + 1] = input.points[i].y;
            vertices[(i * 6) + 2] = 0;
            vertices[(i * 6) + 3] = input.points[i].r;
            vertices[(i * 6) + 4] = input.points[i].g;
            vertices[(i * 6) + 5] = input.points[i].b;

        }

        try
        {

            Delaunator delaunay = new Delaunator(points);

            output.triangles = delaunay.Triangles;
            output.vertices = vertices;

        }
        catch (System.Exception e)
        {

            Console.WriteLine(e);

            foreach (var point in points)
            {

                Console.WriteLine(point.ToString());

            }

        }

        return output;

    }

}