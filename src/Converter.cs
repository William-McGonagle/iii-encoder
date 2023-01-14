using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class Converter
{

    public static RomanFile RasterFileToRoman(string path)
    {

        Image<Rgba32> image = Image.Load<Rgba32>(path);
        return RasterToRoman(image);

    }

    public static RomanFile RasterToRoman(Image<Rgba32> input)
    {

        // TODO: Implement Method
        return new RomanFile();

    }

    public static Image<Rgba32> RomanFileToRaster(string path)
    {

        RomanFile image = RomanFile.ReadFromPath(path);
        return RomanToRaster(image);

    }

    public static Image<Rgba32> RomanToRaster(RomanFile input)
    {

        // TODO: Implement Method
        return new Image<Rgba32>(1, 1, Rgba32.ParseHex("#ffffff"));

    }

}