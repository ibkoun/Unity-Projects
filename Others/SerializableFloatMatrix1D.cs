using System;

/// <summary>
/// One-dimensional float matrix that can be serialized.
/// </summary>
[Serializable]
public class SerializableFloatMatrix1D : SerializableMatrix1D<float>
{
    public SerializableFloatMatrix1D() : base() { }
    //public SerializableFloatMatrix1D(int rowsCapacity, int columnsCapacity) : base(rowsCapacity, columnsCapacity) { }
}
