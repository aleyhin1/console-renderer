namespace ConsoleRenderer.Code
{
    using ConsoleRenderer.Code.Shaders;
    using System.Diagnostics;
    using System.Drawing;

    public class Renderer
    {
        public static (int, int) WindowSize;
        public static float Time;
        private static int _renderMS = 16;
        private static char[] _colors = new char[] { ' ', '~', '<', '#', '$' };
        private static char[] _renderBuffer;
        private static Shader[] _shaders = { RaymarchSphere.Shade };
        private static Shader _selectedShader;
        private delegate void Shader((int, int) fragCoord, out float fragColor);

        static async Task Main(string[] args)
        {
            Init();
            Stopwatch watch = new Stopwatch();
            _selectedShader = _shaders[0];
            
            (int, int) coord;

            while (true)
            {
                watch.Restart();
                HandleWindowChange();

                for (int y = 0; y < WindowSize.Item2; y++)
                {
                    for (int x = 0; x < WindowSize.Item1; x++)
                    {
                        coord.Item1 = x;
                        coord.Item2 = y;

                        _selectedShader(coord, out float fragColor);
                        DrawFragment(coord, fragColor);
                    }
                }


                Console.SetCursorPosition(0, 0);
                Console.Write(_renderBuffer,0, WindowSize.Item1 * WindowSize.Item2);

                watch.Stop();
                float waitTime = MathF.Max(_renderMS - watch.ElapsedMilliseconds, 0);
                await Task.Delay((int)waitTime);
                
                Time += (waitTime + watch.ElapsedMilliseconds) / 1000f;
            }
        }

        private static void Init()
        {
            HandleWindowSize();
            _renderBuffer = new char[Console.LargestWindowWidth * Console.LargestWindowHeight];
            Console.CursorVisible = false;
            Console.Clear();
        }
        
        private static void HandleWindowChange()
        {
            if (WindowSize.Item1 != Console.WindowWidth || WindowSize.Item2 != Console.WindowHeight)
            {
                HandleWindowSize();
            }
        }

        private static void HandleWindowSize()
        {
            WindowSize.Item1 = Console.WindowWidth;
            WindowSize.Item2 = Console.WindowHeight;
        }

        private static void DrawFragment((int, int) coord, float color)
        {
            color = Math.Clamp(color, 0, 1);
            int colIndex = (int)Math.Round(color * (_colors.Length - 1));
            char col = _colors[colIndex];
            int index;
            index = WindowSize.Item1 - coord.Item1 - 1 + (WindowSize.Item2 - coord.Item2 - 1) * WindowSize.Item1;
            _renderBuffer[index] = col;
        }
    }
}
