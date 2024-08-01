public class SpaceData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public byte[] Preview { get; set; }
}

public class AssetData
{
    public int SpaceID { get; set; }
    public string Name { get; set; }
    public float X { get; set; }
    public float Z { get; set; }
    public float? Scale { get; set; }
    public string Model { get; set; }
    public byte[] Preview { get; set; }
}
