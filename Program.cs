using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace LearnOpenTK
{
    public static class Program
    {
        private static void Main()
        {

            RomanFileHeader header = new RomanFileHeader("_r=4144.422;");

            Console.WriteLine("Is Float Value = " + header.GetFloatValue());
            Console.WriteLine("Is Channel = " + header.IsChannel());
            Console.WriteLine(header.ToString());

            RomanFile file = RomanFile.ReadFromPath("./res/200.iii");

            // var nativeWindowSettings = new NativeWindowSettings()
            // {
            //     Size = new Vector2i(800, 600),
            //     Title = "LearnOpenTK - Creating a Window",
            //     // This is needed to run on macos
            //     Flags = ContextFlags.ForwardCompatible,
            // };

            // using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            // {
            //     window.Run();
            // }
        }
    }
}