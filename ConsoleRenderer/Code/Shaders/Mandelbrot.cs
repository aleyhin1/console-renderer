using System.Numerics;

namespace ConsoleRenderer.Code.Shaders
{
    public class Mandelbrot : IShaderEmulator
    {
        private const float ZOOM_VALUE = 8f;
        private const float ZOOM_TIME = .2f;
        private const float PANX = .741199999f;
        private const float PANY = -.1700013f;
        private const float ITERATION_COUNT = 250f;
        private const float MAX_D = 2f;

        private static Vector2 ComplexSquare(Vector2 c)
        {
            Vector2 result;
            result.X = c.X * c.X - c.Y * c.Y;
            result.Y = 2 * c.X * c.Y;
            return result;
        }

        private static Vector2 ComplexAddition(Vector2 a, Vector2 b)
        {
            Vector2 result;
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            return result;
        }

        private static float ComplexDistance(Vector2 c)
        {
            return c.Length();
        }


        public static void Shade((int, int) fragCoord, out float fragColor)
        {
            float scale = MathF.Pow(.5f, ZOOM_VALUE - MathF.Cos(Renderer.Time * ZOOM_TIME) * ZOOM_VALUE);
            
            Vector2 uv;
            uv.X = (float)fragCoord.Item1 / Renderer.WindowSize.Item1;
            uv.Y = (float)fragCoord.Item2 / Renderer.WindowSize.Item2;

            uv.X = 2f * uv.X - 1f;
            uv.Y = 2f * uv.Y - 1f;

            uv *= scale;
            uv.X *= Renderer.WindowSize.Item1 / Renderer.WindowSize.Item2;
            uv.X -= PANX;
            uv.Y -= PANY;

            Vector2 c = new Vector2(uv.X, uv.Y);
            Vector2 z = new Vector2(0, 0);
            float d;

            float i = 0f;
            
            while (i < ITERATION_COUNT)
            {
                z = ComplexSquare(z);
                z = ComplexAddition(z, c);

                d = ComplexDistance(z);
                i++;

                if (d > MAX_D) break;
            }

            float col = i / (ITERATION_COUNT);
            fragColor = col;
        }
    }
}
