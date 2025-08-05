namespace ConsoleRenderer
{
    public class Renderer
    {
        public static (int, int) WindowSize = (300, 150);
        public static float Time;
        private static int _renderMS = 16;
        private static char[] _colors = new char[] { ' ', '~', '<', '#', '$' };
        private static char[] _renderBuffer;

        static async Task Main(string[] args)
        {
            Init();
            
            (int, int) coord;

            while (true)
            {
                for (int y = 0; y < WindowSize.Item2; y++)
                {
                    for (int x = 0; x < WindowSize.Item1; x++)
                    {
                        coord.Item1 = x;
                        coord.Item2 = y;

                        RaymarchSphere.Shade(coord, out float fragColor);
                        DrawFragment(coord, fragColor);
                    }
                }

                Console.SetCursorPosition(0, 0);
                Console.Write(_renderBuffer);
                await Task.Delay(_renderMS);
                Time += _renderMS / 1000f;
            }
        }

        private static void Init()
        {
            Console.SetWindowSize(WindowSize.Item1, WindowSize.Item2);
            Console.SetBufferSize(WindowSize.Item1, WindowSize.Item2);
            _renderBuffer = new char[WindowSize.Item1 * WindowSize.Item2];
            Console.CursorVisible = false;
            Console.Clear();
        }

        private static void DrawFragment((int, int) coord, float color)
        {
            color = Math.Clamp(color, 0, 1);
            int colIndex = (int)Math.Round(color * (_colors.Length - 1));
            char col = _colors[colIndex];
            int index;
            index = (WindowSize.Item1 - coord.Item1 - 1) + (WindowSize.Item2 - coord.Item2 - 1) * WindowSize.Item1;
            _renderBuffer[index] = col;
        }
    }
}
