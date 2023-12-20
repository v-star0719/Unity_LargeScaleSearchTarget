namespace Assets.Temp
{
    public interface IOrthtreeObj
    {
        float[] Pos { get; }
        float[] Size { get; } //目前没有考虑Size
        float PickValue { get; set; }
    }
}