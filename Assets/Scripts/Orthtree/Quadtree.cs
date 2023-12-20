namespace Assets.Temp
{
    public class Quadtree : Orthtree
    {
        public Quadtree(int maxDepth, float[] minSizes) : base(maxDepth, minSizes, 2)
        {
        }

        protected override OrthtreeNode CreateNewNode()
        {
            return new QuadtreeNode(this);
        }
    }
}