using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Review this class.

/// <summary>
/// One-dimensional generic matrix that can be serialized.
/// </summary>
[Serializable]
public class SerializableMatrix1D<T>
{
    [SerializeField] protected int rowsCapacity, columnsCapacity, rowsCount, columnsCount;
    [SerializeField] protected List<T> matrix;

    public SerializableMatrix1D()
    {
        matrix = new List<T>();
    }
    /*public SerializableMatrix1D(int rowsCapacity, int columnsCapacity)
    {
        this.rowsCapacity = rowsCapacity;
        this.columnsCapacity = columnsCapacity;
        matrix = new List<T>();
    }*/

    public List<T> Matrix
    {
        get { return matrix; }
        set { matrix = value; }
    }

    public int RowsCapacity
    {
        get { return rowsCapacity; }
        set { rowsCapacity = value; }
    }

    public int ColumnsCapacity
    {
        get { return columnsCapacity; }
        set { columnsCapacity = value; }
    }

    public int RowsCount
    {
        get { return rowsCount; }
        set { rowsCount = value; }
    }

    public int ColumnsCount
    {
        get { return columnsCount; }
        set { columnsCount = value; }
    }

    // Operations on rows.
    public List<T> GetRow(int i)
    {
        if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false)) return matrix.GetRange(RowIndex(i, columnsCount), columnsCount);
        return null;
    }

    public void SetRow(int i, List<T> row)
    {
        if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false) && ListFitInMatrix(row, columnsCount, columnsCapacity))
        {
            int index = RowIndex(i, columnsCount);
            for (int x = 0; x < columnsCount; ++x)
            {
                matrix[index + x] = row[x];
            }
        }
    }

    public void AddRow(List<T> row)
    {
        if (rowsCount < rowsCapacity)
        {
            if (matrix.Count == 0 && ListCountInRange(row, 1, columnsCapacity, true))
            {
                matrix.AddRange(row);
                ++rowsCount;
                columnsCount += row.Count;
            }
            else if (matrix.Count > 0 && ListFitInMatrix(row, columnsCount, columnsCapacity))
            {
                matrix.AddRange(row);
                ++rowsCount;
            }
        }
    }
    public void InsertRow(int i, List<T> row)
    {
        if (rowsCount < rowsCapacity)
        {
            if (matrix.Count == 0 || i == rowsCount) AddRow(row);
            else if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false) && ListFitInMatrix(row, columnsCount, columnsCapacity))
            {
                matrix.InsertRange(RowIndex(i, columnsCount), row);
                ++rowsCount;
            }
        }
    }

    public void RemoveRow(int i)
    {
        if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false))
        {
            matrix.RemoveRange(RowIndex(i, columnsCount), columnsCount);
            --rowsCount;
            if (matrix.Count == 0) columnsCount = 0;
        }
    }

    // Operations on columns.
    public List<T> GetColumn(int j)
    {
        if (matrix.Count > 0 && IndexInRange(j, 0, columnsCount, false))
        {
            List<T> column = new List<T>();
            for (int i = 0; i < rowsCount; ++i)
            {
                column.Add(matrix[CellIndex(i, j, columnsCount)]);
            }
            return column;
        }
        return null;
    }

    public void SetColumn(int j, List<T> column)
    {
        if (matrix.Count > 0 && IndexInRange(j, 0, columnsCount, false) && ListFitInMatrix(column, rowsCount, rowsCapacity))
        {
            for (int i = 0; i < rowsCount; ++i)
            {
                matrix[CellIndex(i, j, columnsCount)] = column[i];
            }
        }
    }

    public void AddColumn(List<T> column)
    {
        if (columnsCount < columnsCapacity)
        {
            if (matrix.Count == 0 && ListCountInRange(column, 1, columnsCapacity, true))
            {
                matrix.AddRange(column);
                ++columnsCount;
                rowsCount += column.Count;
            }
            else if (matrix.Count > 0 && ListFitInMatrix(column, rowsCount, rowsCapacity))
            {
                for (int i = 0; i < rowsCount; ++i)
                {
                    matrix.Insert(CellIndex(i, columnsCount + i, columnsCount), column[i]);
                }
                ++columnsCount;
            }
        }
    }

    public void InsertColumn(int j, List<T> column)
    {
        if (columnsCount < columnsCapacity)
        {
            if (matrix.Count == 0 || j == columnsCount) AddColumn(column);
            else if (matrix.Count > 0 && IndexInRange(j, 0, columnsCount, false) && ListFitInMatrix(column, rowsCount, rowsCapacity))
            {
                for (int i = 0; i < rowsCount; ++i)
                {
                    matrix.Insert(CellIndex(i, j + i, columnsCount), column[i]);
                }
                ++columnsCount;
            }
        }
    }

    public void RemoveColumn(int j)
    {
        if (matrix.Count > 0 && IndexInRange(j, 0, columnsCount, false))
        {
            for (int i = 0; i < rowsCount; ++i)
            {
                matrix.RemoveAt(CellIndex(i, j, columnsCount));
            }
            --columnsCount;
        }
    }

    // Operations on an element.
    public T Get(int i, int j)
    {
        if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false) && IndexInRange(j, 0, columnsCount, false)) return matrix[CellIndex(i, j, columnsCount)];
        return default(T);
    }

    public void Set(int i, int j, T element)
    {
        if (matrix.Count > 0 && IndexInRange(i, 0, rowsCount, false) && IndexInRange(j, 0, columnsCount, false)) matrix[CellIndex(i, j, columnsCount)] = element;
    }

    // Extensions.
    private bool ListCountInRange(List<T> list, int lowerBound, int upperBound, bool inclusive)
    {
        return list.Count >= lowerBound && (inclusive ? list.Count <= upperBound : list.Count < upperBound);
    }

    public bool ListFitInMatrix(List<T> list, int count, int capacity)
    {
        return list.Count == count && list.Count <= capacity;
    }

    public bool IndexInRange(int index, int lowerBound, int upperBound, bool inclusive)
    {
        return index >= lowerBound && (inclusive ? index <= upperBound : index < upperBound);
    }

    public int RowIndex(int i, int columnsCount)
    {
        return i * columnsCount;
    }

    public int CellIndex(int i, int j, int columnsCount)
    {
        return i * columnsCount + j;
    }
}