/// <summary>
/// Wrapper class for the content of an octant.
/// </summary>
public class OctantContent<T>
{
    public T Value { get; set; }

    public OctantContent(T value)
    {
        Value = value;
    }
}
