# Roman File Format (Interpolated Image Interchange)

## Fundamental Definitions

### Points

A point is defined as an exact position in space which has no length, width, or thickness. Points can store additional information with them, such as color channel values, label information, etc.

### Triangulation

Triangles can be formed with exactly three points. From there, a triangle can be thought of as a set of i-points (interpolated points), which have estimated values based on their position relative to the three points forming the triangle. With enough triangles, any shape can be formed. And, therefore, any image can be formed from the triangles.

The triangulation system that the Roman Format uses by default is Delaunay Triangulation. The algorithm used for triangulation can be configured in the headers of each file, allowing for better triangulation systems to be used for certain images.

### Frames

A frame is a set of triangles. This scene can be in any number of dimensions (depending on which position headers are present). A scene can be thought of by drawing comparisons to a "universe" in Geometry.

All 'image' files store only a single frame. Frames are generally only used in video formats (or in contexts like layers for scientific sampling).

### Files

A file is a set of frames stored in a binary representation. The structure of a file is described in this document, under the "File Structure" section.

## Data Representation and Types

### Point Data Type

As points can contain various amounts of information (such as channels, position, and other tags), there is no simple definition.

#### Example Point

|     X = 2     |    Y = 12     |      R = 5      |       G = 4       |       B = 15      |
|---------------|---------------|-----------------|-------------------|-------------------|
|     0x3       |    0xC        |      0x5        |       0x4         |       0xF         |

##### Little Endian Notation

| 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 |
| - | - | - | - | - | - | - | - | - | - | -- | -- | -- | -- | -- | -- | -- | -- | -- | -- |
| 0 | 0 | 1 | 0 | 1 | 1 | 0 | 0 | 0 | 1 |  0 |  1 |  0 |  1 |  0 |  0 |  1 |  1 |  1 |  1 |

**Point** = 0x3C54F

This point above has two position values and three color channels. Each of these values is a percentage value (from zero to one), allowing for images to be any size.

## File Structure

```mermaid
stateDiagram-v2
    [*] --> Head
    Head --> HeaderKey
    HeaderKey --> HeaderValue
    HeaderValue --> Newline
    Head --> Body
    Newline --> Body
    Newline --> HeaderKey

```

The Roman File Format starts with the head of the document. The head is a fixed size of eight bytes. After the head comes each of the headers. The headers are all grouped together in the "headers" section of the document. The header section is terminated with two semicolons. After the header section is the body, which is terminated with the end of the file.

### Head

```raw
0x89 = Binary File
0x49 = I
0x49 = I
0x49 = I
0x0D = Carriage Return
0x0A = Line Feed
0x01 = Version
0x0A = Line Feed
```

The above binary is what defines a Roman file. Very similar to the PNG format, the Roman format uses eight bytes as a header. But, rather than the second through fourth bytes being P (0x50), N (0x4E), and G (0x47), the bytes are I (0x49), I (0x49), and I (0x49). This allows any operating system to read the first four bytes of the file and understand that the file is Roman.

The Seventh Byte (denoted with 'Version'), is the Version byte. From this byte, operating systems will be able to determine the current version of the Roman File Format that a file is using. Reading this byte will ensure no corruption when processing a file.

### Headers

Each header is defined by a sequence of bytes, then an equals sign (0x3D), the value of the header, and then a semicolon (0x3B).

The headers end when two semicolons are sequential (one after the other, ;;). If these two semicolons are found, the body will begin to be processed.

#### Header Keys

| Key ID      | Description           | Value        | Usage     | Multiple? |
| ----------- | --------------------- | ------------ | --------- | --------- |
| Title       | Title of the File     | String       | Per File  | No        |
| Author      | Author of the File    | String       | Per File  | Yes       |
| X           | X Position Component  | Integer      | Per Point | No        |
| Y           | Y Position Component  | Integer      | Per Point | No        |
| Z           | Z Position Component  | Integer      | Per Point | No        |
| _W          | White Color Channel   | Integer      | Per Point | No        |
| _R          | Red Color Channel     | Integer      | Per Point | No        |
| _G          | Green Color Channel   | Integer      | Per Point | No        |
| _B          | Blue Color Channel    | Integer      | Per Point | No        |
| _A          | Alpha Color Channel   | Integer      | Per Point | No        |

### Body
