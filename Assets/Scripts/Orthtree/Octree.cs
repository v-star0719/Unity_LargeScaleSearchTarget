namespace Assets.Temp
{
    public class Octree : Orthtree
    {
        public Octree(int maxDepth, float[] minSizes) : base(maxDepth, minSizes, 3)
        {
        }

        protected override OrthtreeNode CreateNewNode()
        {
            return new OctreeNode(this);
        }
    }
}