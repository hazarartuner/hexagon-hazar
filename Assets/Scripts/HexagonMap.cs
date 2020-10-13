using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonMap
{
    public int columnCount;
    public int rowCount;
    public float cellWidth = 1f;
    public float cellHeight = 1f;
    public float cellHorizontalOffset = 0.25f;
    public float cellVerticalOffset = 0.25f;
    public int gemAssetCount;

    public event Action<Cell> OnCellInstantiated;
    public event Action<Cell> OnCellDestroyed;

    public enum CellType
    {
        Gem,
        Bomb
    };
        
    [Serializable]
    public struct Cell
    {
        public Vector3 position;
        public int columnIndex;
        public int rowIndex;
        public int assetIndex;
        public CellType cellType;
    }
    
#region Private Fields
    private Cell[,] _map;
#endregion

    public void GenerateMap()
    {
        if (_map != null)
        {
            return;
        }
        
        _map = new Cell[columnCount,rowCount];

        for (int i = 0; i < columnCount; i++)
            for (int j = 0; j < rowCount; j++)
                _map[i, j] = InstantiateCell(i, j, CellType.Gem, Random.Range(0, gemAssetCount));
    }

    public void ClearMap()
    {
        if (_map == null)
        {
            return;
        }

        for (int i = 0; i < columnCount; i++)
            for (int j = 0; j < rowCount; j++)
                DestroyCell(i, j);

        _map = null;
    }
    
    private Cell InstantiateCell(int columnIndex, int rowIndex, CellType cellType, int assetIndex)
    {
        var cell = new Cell {
            position = CalculateCellPosition(columnIndex, rowIndex),
            assetIndex = assetIndex,
            cellType = cellType,
            columnIndex = columnIndex,
            rowIndex = rowIndex
        };
        
        if (OnCellInstantiated != null)
        {
            OnCellInstantiated(cell);
        }

        return cell;
    }

    private Vector3 CalculateCellPosition(int columnIndex, int rowIndex)
    {
        return new Vector3(
            columnIndex * cellWidth - columnIndex * cellHorizontalOffset,
            -(rowIndex * cellHeight + (columnIndex % 2) * cellVerticalOffset),
            0
        );
    }

    private void DestroyCell(int columnIndex, int rowIndex)
    {
        if (!(_map[columnIndex, rowIndex].assetIndex >= 0))
        {
            return;
        }
        
        if (OnCellDestroyed != null)
        {
            OnCellDestroyed(_map[columnIndex, rowIndex]);
        }

        _map[columnIndex, rowIndex].assetIndex = -1;
    }
}