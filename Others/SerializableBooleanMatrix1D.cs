using System;

/// <summary>
/// One-dimensional boolean matrix that can be serialized.
/// </summary>
[Serializable]
public class SerializableBooleanMatrix1D : SerializableMatrix1D<bool>
{
    public SerializableBooleanMatrix1D() : base() { }
    //public SerializableBooleanMatrix1D(int rowsCapacity, int columnsCapacity) : base(rowsCapacity, columnsCapacity) { }
}
