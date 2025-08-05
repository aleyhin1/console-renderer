namespace ConsoleRenderer.Code
{
    public interface IShaderEmulator
    {
        public abstract static void Shade((int, int) fragCoord, out float fragColor);
    }
}
