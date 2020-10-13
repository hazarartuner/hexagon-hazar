using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonMap
{
    public int columnCount;
    public int rowCount;
    public float cellWidth = 2.5f;
    public float cellHeight = 2.5f;
    public float cellHorizontalOffset = 0.635f;
    public float cellVerticalOffset = 1.25f;
    public int gemAssetCount;

    public event Action<Cell> OnCellSet;
    public event Action<Cell> OnCellCleared;

    public enum CellType
    {
        Empty,
        Gem,
        Bomb,
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

    /// <summary>
    /// Instantiates the map by filling cells randomly unless "fillRandomly" parameter is false.
    /// Otherwise map will be instantiated with empty cells.
    /// </summary>
    /// <param name="fillRandomly"></param>
    public void Instantiate(bool fillRandomly = true)
    {
        if (_map != null)
        {
            return;
        }
        
        _map = new Cell[columnCount,rowCount];

        for (int i = 0; i < columnCount; i++)
        for (int j = 0; j < rowCount; j++)
            if (fillRandomly)
                _map[i, j] = SetCell(i, j, CellType.Gem, Random.Range(0, gemAssetCount));
            else
                _map[i, j] = SetCell(i, j);
    }

    /// <summary>
    /// Clears the cells but not destroy them.
    /// </summary>
    public void ClearMap()
    {
        if (_map == null)
        {
            return;
        }

        for (int i = 0; i < columnCount; i++)
        for (int j = 0; j < rowCount; j++)
        {
            if (_map[i, j].cellType == CellType.Empty)
                continue;

            ClearCell(i, j);
        }
    }

    /// <summary>
    /// Clears then destroys the map
    /// </summary>
    public void DestroyMap()
    {
        ClearMap();

        _map = null;
    }
    
    /// <summary>
    /// Instantiates cell in map. In default, this method will create empty cell
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    /// <param name="cellType">type of cell</param>
    /// <param name="assetIndex">asset index of selected cell type</param>
    /// <returns>cell instance</returns>
    public Cell SetCell(int columnIndex, int rowIndex, CellType cellType = CellType.Empty, int assetIndex = -1)
    {
        var cell = new Cell {
            position = CalculateCellPosition(columnIndex, rowIndex),
            assetIndex = assetIndex,
            cellType = cellType,
            columnIndex = columnIndex,
            rowIndex = rowIndex
        };
        
        if (OnCellSet != null)
        {
            OnCellSet(cell);
        }

        _map[columnIndex, rowIndex] = cell;

        return cell;
    }

    /// <summary>
    /// Calculates the cell position according to the map configuration.
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    /// <returns>position of cell according to the origin. You can use it as localPosition to align them by parent</returns>
    public Vector3 CalculateCellPosition(int columnIndex, int rowIndex)
    {
        return new Vector3(
            columnIndex * cellWidth - columnIndex * cellHorizontalOffset,
            -(rowIndex * cellHeight + (columnIndex % 2) * cellVerticalOffset),
            0
        );
    }

    /// <summary>
    /// Clears cell but not destroy it.
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    public void ClearCell(int columnIndex, int rowIndex)
    {
        if (!(_map[columnIndex, rowIndex].assetIndex >= 0))
        {
            return;
        }
        
        if (OnCellCleared != null)
        {
            OnCellCleared(_map[columnIndex, rowIndex]);
        }

        _map[columnIndex, rowIndex].assetIndex = -1;
        _map[columnIndex, rowIndex].position = Vector3.zero;
        _map[columnIndex, rowIndex].cellType = CellType.Empty;
    }

    /// <summary>
    /// Gets the selected cell
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    /// <returns></returns>
    public Cell? GetCell(int columnIndex, int rowIndex)
    {
        return _map?[columnIndex, rowIndex];
    }
}