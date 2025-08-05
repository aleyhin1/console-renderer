using ConsoleRenderer.Code;
using System.Numerics;

namespace ConsoleRenderer.Code.Shaders
{
    public class RaymarchSphere : IShaderEmulator
    {
        private const int _RAYMARCH_STEPS = 30;
        private const float _MAX_DISTANCE = 100f;
        private const float _MIN_DISTANCE = .01f;

        private static float SDFBall(Vector3 p, Vector3 ballP)
        {
            return Vector3.Distance(p, ballP) - 1;
        }

        private static float SDFFloor(Vector3 p)
        {
            return p.Y + 2;
        }

        private static float GetDistance(Vector3 p)
        {
            float result;

            Vector3 ball1Pos = new Vector3(0, MathF.Sin(Renderer.Time * 10f), 0);
            float sdfBall1 = SDFBall(p, ball1Pos);
            float sdfFloor = SDFFloor(p);
            result = MathF.Min(sdfBall1, sdfFloor);
            return result;
        }

        private static float Raymarch(Vector3 ro, Vector3 rd)
        {
            Vector3 p = ro;
            float dx = 0;

            for (int i = 0; i < _RAYMARCH_STEPS; i++)
            {
                float d = GetDistance(p);
                dx += d;

                p = ro + rd * dx;

                if (dx > _MAX_DISTANCE || d < _MIN_DISTANCE) break;
            }

            return dx;
        }

        private static Vector3 GetNormal(Vector3 p)
        {
            Vector2 s = new Vector2(0.01f, 0);
            float d = GetDistance(p);

            Vector3 normal = new Vector3(d - GetDistance(p - new Vector3(s.X, s.Y, s.Y)), d - GetDistance(p - new Vector3(s.Y, s.X, s.Y)),
                d - GetDistance(p - new Vector3(s.Y, s.Y, s.X)));
            return Vector3.Normalize(normal);
        }

        private static float GetLight(Vector3 p)
        {
            Vector3 lightPos = new Vector3(MathF.Cos(Renderer.Time * 5) * 2, 10, MathF.Sin(Renderer.Time * 5) * 2);
            Vector3 lightVec = Vector3.Normalize(lightPos - p);
            Vector3 N = GetNormal(p);
            float dif = Math.Clamp(Vector3.Dot(lightVec, N), 0, 1);

            float shadow = Raymarch(p + N * 0.2f, lightVec);

            if (shadow < (lightPos - p).Length()) dif *= .1f;
            return dif;
        }

        public static void Shade((int, int) fragCoord, out float fragColor)
        {
            Vector2 uv;
            uv.X = (float)fragCoord.Item1 / Renderer.WindowSize.Item1;
            uv.Y = (float)fragCoord.Item2 / Renderer.WindowSize.Item2;
            uv.X = uv.X * 2 - 1;
            uv.Y = uv.Y * 2 - 1;

            Vector3 ro = new Vector3(0, 2, -10f + MathF.Sin(Renderer.Time * 5f) * 5f);
            Vector3 rd = new Vector3(uv.X, uv.Y, 1);

            float d = Raymarch(ro, rd);

            Vector3 p = ro + rd * d;
            float light = GetLight(p);
            fragColor = light;
        }
    }
}
