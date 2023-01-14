using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace LearnOpenTK
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Window : GameWindow
    {
        float[] _vertices =
        {
            // positions        // colors
            0.5f,  0.5f, 0.0f, 1.0f,  0.0f, 0.0f,  // top right
            0.5f, -0.5f, 0.0f, 0.0f,  1.0f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f,  0.0f, 1.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f,  0.5f, 0.5f, // top left    
        };

        int[] _indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };

        public bool wireframeMode = false;
        public int resolution = 100_000;

        private int _vertexBufferObject;
        private int _elementBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        public void LoadIII(Mesh mesh)
        {

            _vertices = mesh.vertices;
            _indices = mesh.triangles;

            Title = mesh.name + " - McGonagle Image Viewer";

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // For Positions
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // For Colors
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

        }

        public void UnloadIII()
        {



        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {

            base.OnFileDrop(e);

            string filePath = e.FileNames[0];

            if (filePath.EndsWith(".iii"))
            {

                FileData data = Porter.fromFilePath(filePath);
                Mesh myMesh = Mesher.fromFileData(data);

                UnloadIII();
                LoadIII(myMesh);

                return;

            }
            else
            {

                FileData data = OldConverter.fromFileToIII(filePath);
                Mesh myMesh = Mesher.fromFileData(data);

                Porter.toFile(data);

                UnloadIII();
                LoadIII(myMesh);

            }

        }

        protected override void OnLoad()
        {
            base.OnLoad();
            FileData data = Porter.fromFilePath("res/Testing.iii");
            Mesh myMesh = Mesher.fromFileData(data);
            LoadIII(myMesh);

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            if (wireframeMode)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            if (input.IsKeyPressed(Keys.Equal) && input.IsKeyPressed(Keys.LeftShift))
            {

                resolution += 10_000;

                FileData data = Porter.fromFilePath("res/Testing.iii");
                Mesh myMesh = Mesher.fromFileData(data);
                LoadIII(myMesh);

            }

            if (input.IsKeyPressed(Keys.Minus))
            {

                resolution -= 10_000;

                FileData data = Porter.fromFilePath("res/Testing.iii");
                Mesh myMesh = Mesher.fromFileData(data);
                LoadIII(myMesh);

            }

            if (input.IsKeyPressed(Keys.W))
            {
                wireframeMode = !wireframeMode;
            }

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X * 2, Size.Y * 2);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);

            base.OnUnload();
        }
    }
}