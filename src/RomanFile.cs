using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class RomanFile
{

    public int version;
    public RomanFileHeader[] headers;
    public BitArray body;

    public RomanFile()
    {

        version = 1;
        headers = new RomanFileHeader[0];
        body = new BitArray(new bool[0]);

    }

    public RomanFile(int _version)
    {

        version = _version;
        headers = new RomanFileHeader[0];
        body = new BitArray(new bool[0]);

    }

    public static RomanFile ReadFromPath(string path)
    {

        // Check if the file exists
        if (!File.Exists(path)) return null;

        // Read through and parse the file
        return ReadFromStream(File.OpenRead(path));

    }

    public static RomanFile ReadFromStream(FileStream stream)
    {

        // Read 1024 Bytes at a time
        byte[] b = new byte[1024];

        // Create Roman File
        RomanFile output = new RomanFile();

        // Parsing Variables
        int state = 0;
        int readLen = 0;
        while ((readLen = stream.Read(b, 0, b.Length)) > 0)
        {

            output = ProcessRomanFile(b, readLen, output, ref state);

        }

        return output;

    }

    public static RomanFile ProcessRomanFile(byte[] data, int length, RomanFile through, ref int state)
    {

        StringBuilder key = new StringBuilder();
        StringBuilder value = new StringBuilder();

        List<RomanFileHeader> _headers = new List<RomanFileHeader>(through.headers);

        int i = 0;
        while (i < length)
        {

            switch (state)
            {

                case 0:
                    // First Byte

                    if (data[i] != 0x89) throw new Exception("First Byte Isn't 0x89");
                    state = 1;
                    i++;

                    break;
                case 1:
                    // Second Byte

                    if (data[i] != 0x49) throw new Exception("Second Byte Isn't 0x49");
                    state = 2;
                    i++;

                    break;
                case 2:
                    // Third Byte

                    if (data[i] != 0x49) throw new Exception("Third Byte Isn't 0x49");
                    state = 3;
                    i++;

                    break;
                case 3:
                    // Fourth Byte

                    if (data[i] != 0x49) throw new Exception("Fourth Byte Isn't 0x49");
                    state = 4;
                    i++;

                    break;
                case 4:
                    // Fifth Byte

                    if (data[i] != 0x0D) throw new Exception("Fifth Byte Isn't 0x0D");
                    state = 5;
                    i++;

                    break;
                case 5:
                    // Sixth Byte

                    if (data[i] != 0x0A) throw new Exception("Sixth Byte Isn't 0x0A");
                    state = 6;
                    i++;

                    break;
                case 6:
                    // Version Byte

                    through.version = data[i];
                    state = 7;
                    i++;

                    break;
                case 7:
                    // 8th Byte

                    if (data[i] != 0x0A) throw new Exception("Eighth Byte Isn't 0x0A");
                    state = 8;
                    i++;

                    break;
                case 8:
                    // Header Key

                    if (data[i] == ';' && key.Length == 0)
                    {

                        state = 10;
                        i++;
                        break;

                    }

                    if (data[i] == '=')
                    {

                        state = 9;
                        i++;
                        break;

                    }

                    key.Append((char)data[i]);
                    i++;

                    break;
                case 9:
                    // Header Value

                    if (data[i] == ';')
                    {

                        // Add the Headers to a Temp Array
                        _headers.Add(new RomanFileHeader(key.ToString(), value.ToString()));

                        // Clear Out the Buffers
                        key.Clear();
                        value.Clear();

                        state = 8;
                        i++;
                        break;

                    }

                    value.Append((char)data[i]);
                    i++;

                    break;

                default:

                    string[] headerOut = new string[_headers.Count];

                    for (int n = 0; n < headerOut.Length; n++)
                    {

                        headerOut[n] = _headers[n].ToString();

                    }

                    throw new Exception($"You found the edge of the map! (you had a version of {through.version}, and the following headers:\n{string.Join('\n', headerOut)}");

            }

        }

        through.headers = _headers.ToArray();

        return through;

    }

    public void ToFile(string path)
    {

        // Check if the file exists
        if (File.Exists(path))
        {

            // Read through and parse the file
            ToStream(File.OpenRead(path));

        }
        else
        {

            // If not, Create the File
            ToStream(File.Create(path));

        }

    }

    public void ToStream(FileStream stream)
    {

        byte[] data = ToBytes();
        stream.Write(data, 0, data.Length);

    }

    public byte[] ToBytes()
    {

        return RomanFileToBytes(this);

    }

    public static byte[] RomanFileToBytes(RomanFile input)
    {

        int length = 8;
        StringBuilder headerString = new StringBuilder();

        for (int i = 0; i < input.headers.Length; i++)
        {

            headerString.Append(input.ToString());

        }

        length += headerString.Length;

        byte[] output = new byte[length];

        // 0x89 = Binary File
        // 0x49 = I
        // 0x49 = I
        // 0x49 = I
        // 0x0D = Carriage Return
        // 0x0A = Line Feed
        // 0x1A = Version
        // 0x0A = Line Feed
        output[0] = 0x89;
        output[1] = 0x49;
        output[2] = 0x49;
        output[3] = 0x49;
        output[4] = 0x0D;
        output[5] = 0x0A;
        output[6] = (byte)input.version;
        output[7] = 0x0D;

        byte[] headerBytes = Encoding.ASCII.GetBytes(headerString.ToString());
        headerBytes.CopyTo(output, 8);

        return output;

    }

}

public class RomanFileHeader
{

    public string key;
    public string value;

    public RomanFileHeader(string input)
    {

        int i = 0;
        int state = 0;
        string buffer = "";
        while (i < input.Length)
        {

            switch (state)
            {

                case 0:

                    if (input[i] == '=')
                    {

                        state = 1;
                        key = buffer;
                        buffer = "";
                        break;

                    }

                    if (input[i] == '\\')
                    {

                        i++;
                        if (i >= input.Length) break;

                        buffer += input[i];
                        break;

                    }

                    buffer += input[i];

                    break;

                case 1:

                    if (input[i] == ';')
                    {

                        state = 2;
                        value = buffer;
                        buffer = "";
                        break;

                    }

                    if (input[i] == '\\')
                    {

                        i++;
                        if (i >= input.Length) break;

                        buffer += input[i];
                        break;

                    }

                    buffer += input[i];

                    break;

                case 2:

                    break;

            }

            i++;

        }

    }

    public RomanFileHeader(string _key, string _value)
    {

        key = _key;
        value = _value;

    }

    public bool IsChannel()
    {

        if (key.Length == 0) return false;

        return key[0] == '_';

    }

    public string GetStringValue()
    {

        return value;

    }

    public float GetFloatValue()
    {

        float sum = 0;
        float multiplier = 1.0f;

        for (int i = 0; i < value.Length; i++)
        {

            if (value[i] == '.') multiplier = 0.1f;

            if (multiplier == 1)
                sum *= 10;

            sum += (value[i] - 0x30) * multiplier;

            if (multiplier != 1)
                multiplier /= 10;

        }

        return sum;

    }

    public override string ToString()
    {

        return $"{key}={value};";

    }

}