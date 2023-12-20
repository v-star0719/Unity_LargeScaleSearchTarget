namespace Assets.Temp
{
    public class Binatree : Orthtree
    {
        public Binatree(int maxDepth, float[] minSizes) : base(maxDepth, minSizes, 1)
        {
        }

        protected override OrthtreeNode CreateNewNode()
        {
            return new BinatreeNode(this);
        }
    }
}