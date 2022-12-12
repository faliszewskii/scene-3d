namespace scene_3d.utilities
{
    public class Utilities
    {
        public int WrapI(int i, int size) => i < 0 ? i + size * (-(i + 1) / size + 1) : i % size;

    }
}
