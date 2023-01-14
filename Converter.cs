using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class OldConverter
{

    public static FileData fromFileToIII(string path)
    {
        Image<Rgba32> image = Image.Load<Rgba32>(path);
        string name = Path.GetFileName(path);
        return fromImageToIII(image, name);
    }

    public static FileData fromImageToIII(Image<Rgba32> image, string name)
    {

        int xRes = image.Width;
        int yRes = image.Height;

        int xRatio = image.Width / xRes;
        int yRatio = image.Height / yRes;

        string _name = name.Split('.')[0] + ".iii";
        FileData output = new FileData(_name);
        List<Point> tempPoints = new List<Point>();

        for (int x = 0; x < xRes; x++)
        {

            for (int y = 0; y < yRes; y++)
            {

                float r = image[x * xRatio, y * yRatio].R / 255.0f;
                float g = image[x * xRatio, y * yRatio].G / 255.0f;
                float b = image[x * xRatio, y * yRatio].B / 255.0f;

                tempPoints.Add(new Point(
                    transformPixelX(x, xRes),
                    transformPixelY(y, yRes),
                    r,
                    g,
                    b
                ));

            }

        }

        output.points = tempPoints.ToArray();

        float gamma = 0.03f;

        output = demolish(output, gamma);

        // int n = 0;
        // bool running = true;
        // while (running)
        // {

        //     gamma += 0.01f;

        //     if (n > 5)
        //         running = false;

        //     n++;

        // }

        return output;

    }

    public static float transformPixelX(int x, int xRes)
    {

        return 2.0f * x / (float)xRes - 1;

    }

    public static float transformPixelY(int y, int yRes)
    {

        return -2.0f * y / (float)yRes + 1;

    }

    public static FileData demolish(FileData input, float gamma = 0.000003f)
    {

        Mesh myMesh = Mesher.fromFileData(input);
        int triangleCount = myMesh.triangles.Length / 3;
        float totalScore = 0;

        for (int i = 0; i < triangleCount; i++)
        {

            float score = gradeTriangle(
                myMesh.triangles[i * 3 + 0],
                myMesh.triangles[i * 3 + 1],
                myMesh.triangles[i * 3 + 2],
                myMesh.vertices
            );

            totalScore += score;

            if (score > gamma)
            {

                float A_x = myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 0];
                float A_y = myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 1];
                float A_r = myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 3];
                float A_g = myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 4];
                float A_b = myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 5];

                float B_x = myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 0];
                float B_y = myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 1];
                float B_r = myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 3];
                float B_g = myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 4];
                float B_b = myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 5];

                float C_x = myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 0];
                float C_y = myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 1];
                float C_r = myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 3];
                float C_g = myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 4];
                float C_b = myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 5];

                float x = (A_x + B_x + C_x) / 3;
                float y = (A_y + B_y + C_y) / 3;
                float r = (A_r + B_r + C_r) / 3;
                float g = (A_g + B_g + C_g) / 3;
                float b = (A_b + B_b + C_b) / 3;

                // A_x and A_y
                if (inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 0]) &&
                    inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 1]))
                {
                    myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 0] = x;
                    myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 1] = y;
                }

                myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 3] = r;
                myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 4] = g;
                myMesh.vertices[myMesh.triangles[i * 3 + 0] * 6 + 5] = b;

                // B_x and B_y
                if (inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 0]) &&
                    inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 1]))
                {
                    myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 0] = x;
                    myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 1] = y;
                }
                myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 4] = g;
                myMesh.vertices[myMesh.triangles[i * 3 + 1] * 6 + 5] = b;

                // C_x and C_y
                if (inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 0]) &&
                    inBounds(myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 1]))
                {
                    myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 0] = x;
                    myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 1] = y;
                }

                myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 3] = r;
                myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 4] = g;
                myMesh.vertices[myMesh.triangles[i * 3 + 2] * 6 + 5] = b;

            }

        }

        return Mesher.toFileData(myMesh);

    }

    public static bool inBounds(float a)
    {

        const float borderWidth = 0.005f;
        return (a < 1 - borderWidth) && (a > -1 + borderWidth);

    }

    public static float triangleSize(int a, int b, int c, float[] vertices)
    {

        float A_x = vertices[a * 6 + 0];
        float A_y = vertices[a * 6 + 1];

        float B_x = vertices[b * 6 + 0];
        float B_y = vertices[b * 6 + 1];

        float C_x = vertices[c * 6 + 0];
        float C_y = vertices[c * 6 + 1];

        float i = (float)Math.Sqrt((A_x - B_x) * (A_x - B_x) + (A_y - B_y) * (A_y - B_y));
        float j = (float)Math.Sqrt((B_x - C_x) * (B_x - C_x) + (B_y - C_y) * (B_y - C_y));
        float k = (float)Math.Sqrt((C_x - A_x) * (C_x - A_x) + (C_y - A_y) * (C_y - A_y));

        float s = (i + j + k) / 3;

        return (float)Math.Sqrt(s * (s - i) * (s - j) * (s - k));

    }

    public static float gradeTriangle(int a, int b, int c, float[] vertices)
    {

        float redAvg = (vertices[a * 6 + 3] + vertices[b * 6 + 3] + vertices[c * 6 + 3]) / 3;
        float greenAvg = (vertices[a * 6 + 4] + vertices[b * 6 + 4] + vertices[c * 6 + 4]) / 3;
        float blueAvg = (vertices[a * 6 + 5] + vertices[b * 6 + 5] + vertices[c * 6 + 5]) / 3;

        float redDist = Math.Abs(redAvg - vertices[a * 6 + 3]) + Math.Abs(redAvg - vertices[b * 6 + 3]) + Math.Abs(redAvg - vertices[c * 6 + 3]);
        float greenDist = Math.Abs(greenAvg - vertices[a * 6 + 4]) + Math.Abs(greenAvg - vertices[b * 6 + 4]) + Math.Abs(greenAvg - vertices[c * 6 + 4]);
        float blueDist = Math.Abs(blueAvg - vertices[a * 6 + 5]) + Math.Abs(blueAvg - vertices[b * 6 + 5]) + Math.Abs(blueAvg - vertices[c * 6 + 5]);

        float distance = redDist + greenDist + blueDist;

        return distance;
        // return distance * triangleSize(a, b, c, vertices);

    }

}