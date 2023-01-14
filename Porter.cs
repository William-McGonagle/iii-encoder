using System;
using System.Collections.Generic;
using System.IO;

public struct FileData
{

    public string name;
    public Point[] points;

    public FileData(string _name)
    {

        name = _name;
        points = new Point[0];

    }

}

public struct Point
{

    public float x;
    public float y;
    public float r;
    public float g;
    public float b;
    public float a;

    public Point(float _x, float _y, float _r, float _g, float _b)
    {

        x = _x;
        y = _y;
        r = _r;
        g = _g;
        b = _b;
        a = 1;

    }

}

public static class Porter
{

    public static FileData fromFilePath(string path)
    {

        byte[] readText = File.ReadAllBytes(path);
        string filename = Path.GetFileName(path);

        return fromBinary(readText, filename);

    }

    public static FileData fromBinary(byte[] input)
    {

        return fromBinary(input, "Unnamed File");

    }

    public static FileData fromBinary(byte[] input, string name)
    {

        FileData output = new FileData();
        output.name = name;

        if (input[0] != 0x89) throw new Exception("Invalid Head.");
        if (input[1] != 0x49) throw new Exception("Invalid Head.");
        if (input[2] != 0x49) throw new Exception("Invalid Head.");
        if (input[3] != 0x49) throw new Exception("Invalid Head.");
        if (input[4] != 0x0D) throw new Exception("Invalid Head.");
        if (input[5] != 0x0A) throw new Exception("Invalid Head.");
        if (input[6] != 0x1A) throw new Exception("Invalid Head.");
        if (input[7] != 0x0A) throw new Exception("Invalid Head.");

        Dictionary<string, string> headers = new Dictionary<string, string>();

        bool findingHeaders = true;
        bool turn = true;
        int i = 0;
        string headerBuffer = "";
        string valueBuffer = "";
        while (findingHeaders && i < input.Length)
        {

            if (turn)
            {

                if (input[i] == '=')
                {

                    turn = false;
                    i++;
                    continue;

                }

                headerBuffer += input[i];
                i++;
                continue;

            }
            else
            {

                if (input[i] == ';')
                {

                    turn = true;
                    headers[headerBuffer] = valueBuffer;
                    headerBuffer = "";
                    valueBuffer = "";
                    i++;

                    if (input[i] == ';')
                    {

                        i++;
                        break;

                    }

                    continue;

                }

                valueBuffer += input[i];
                i++;
                continue;

            }

        }

        List<Point> points = new List<Point>();

        while (i < input.Length)
        {

            byte[] x = new byte[sizeof(float)];
            byte[] y = new byte[sizeof(float)];

            x[0] = input[i + 0];
            x[1] = input[i + 1];
            x[2] = input[i + 2];
            x[3] = input[i + 3];

            i += sizeof(float);

            y[0] = input[i + 0];
            y[1] = input[i + 1];
            y[2] = input[i + 2];
            y[3] = input[i + 3];

            i += sizeof(float);

            float _x = BitConverter.ToSingle(x, 0);
            float _y = BitConverter.ToSingle(y, 0);

            float r = input[i + 0] / 255.0f;
            float g = input[i + 1] / 255.0f;
            float b = input[i + 2] / 255.0f;

            i += sizeof(byte);
            i += sizeof(byte);
            i += sizeof(byte);

            points.Add(new Point(_x, _y, r, g, b));

        }

        output.points = points.ToArray();

        return output;

    }

    public static void toFile(FileData input)
    {

        byte[] binary = toBinary(input);
        string path = "res/" + input.name;

        File.WriteAllBytes(path, binary);

    }

    public static byte[] toBinary(FileData input)
    {

        // HEAD
        // iii 
        // HEADERS
        // POINTS

        string[] headers = {
            "X",
            "Y",
            "R",
            "G",
            "B",
        };
        string[] values = {
            "0",
            "1",
            "2",
            "3",
            "4",
        };

        int size = 0;
        size += 4; // 0x89 0x49 0x49 0x49
        size += 2; // 0x0D 0x0A
        size += 2; // 0x1A 0x0A

        for (int i = 0; i < headers.Length; i++)
        {

            size += headers[i].Length;
            size++;
            size += values[i].Length;
            size++;

        }

        // For header ending
        size++;

        // For Points
        for (int i = 0; i < input.points.Length; i++)
        {

            size += sizeof(float); // X
            size += sizeof(float); // Y
            size += sizeof(byte); // R
            size += sizeof(byte); // G
            size += sizeof(byte); // B

            // size += sizeof(byte); // remove soon

        }

        byte[] output = new byte[size];

        // File Head - Hardcoded
        output[0] = 0x89;
        output[1] = 0x49;
        output[2] = 0x49;
        output[3] = 0x49;
        output[4] = 0x0D;
        output[5] = 0x0A;
        output[6] = 0x1A;
        output[7] = 0x0A;

        // Output the Headers
        // Format is as follows
        //      HEADER=VALUE;HEADER=VALUE;HEADER=VALUE;
        int index = 8;
        for (int i = 0; i < headers.Length; i++)
        {

            for (int j = 0; j < headers[i].Length; j++)
            {

                output[index] = (byte)headers[i][j];
                index++;

            }

            output[index] = (byte)'=';
            index++;

            for (int j = 0; j < values[i].Length; j++)
            {

                output[index] = (byte)values[i][j];
                index++;

            }

            output[index] = (byte)';';
            index++;

        }

        output[index] = (byte)';';
        index++;

        for (int i = 0; i < input.points.Length; i++)
        {

            byte[] x = BitConverter.GetBytes(input.points[i].x);
            byte[] y = BitConverter.GetBytes(input.points[i].y);

            byte r = (byte)((int)(input.points[i].r * 255));
            byte g = (byte)((int)(input.points[i].g * 255));
            byte b = (byte)((int)(input.points[i].b * 255));

            output[index + 0] = x[0];
            output[index + 1] = x[1];
            output[index + 2] = x[2];
            output[index + 3] = x[3];

            index += sizeof(float);

            output[index + 0] = y[0];
            output[index + 1] = y[1];
            output[index + 2] = y[2];
            output[index + 3] = y[3];

            index += sizeof(float);

            output[index + 0] = r;
            output[index + 1] = g;
            output[index + 2] = b;

            index += sizeof(byte);
            index += sizeof(byte);
            index += sizeof(byte);

            // output[index + 0] = (byte)';'; // remove soon
            // index += 1;

        }

        return output;

    }

}