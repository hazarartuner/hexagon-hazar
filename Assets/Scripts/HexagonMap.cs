using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
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

    public static readonly Vector2Int Top = new Vector2Int(0, -1);
    public static readonly Vector2Int TopRightEven = new Vector2Int(1, 0);
    public static readonly Vector2Int TopRightOdd = new Vector2Int(1, -1);
    public static readonly Vector2Int BottomRightEven = new Vector2Int(1, 1);
    public static readonly Vector2Int BottomRightOdd = new Vector2Int(1, 0);
    public static readonly Vector2Int Bottom = new Vector2Int(0, 1);
    public static readonly Vector2Int BottomLeftEven = new Vector2Int(-1, 1);
    public static readonly Vector2Int BottomLeftOdd = new Vector2Int(-1, 0);
    public static readonly Vector2Int TopLeftEven = new Vector2Int(-1, 0);
    public static readonly Vector2Int TopLeftOdd = new Vector2Int(-1, -1);

    public static readonly Vector2Int[] SiblingDirectionsEven = {
        Top, TopRightEven, BottomRightEven, Bottom, BottomLeftEven, TopLeftEven, Top
    };
    
    public static readonly Vector2Int[] SiblingDirectionsOdd = {
        Top, TopRightOdd, BottomRightOdd, Bottom, BottomLeftOdd, TopLeftOdd, Top
    };
    
    public enum CellType
    {
        Empty,
        Gem,
        Bomb,
    };

    [Serializable]
    public struct Cell
    {
        public int columnIndex;
        public int rowIndex;
        public CellType cellType;
        public int assetIndex;
        public Vector3 position;
        public bool isChecked;

        public override string ToString()
        {
            return "column: " + columnIndex + ", row: " + rowIndex + ", type: " + cellType + ", asset: " + assetIndex + ", position: " + position;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return (columnIndex + "," + rowIndex).GetHashCode();
        }
    }
    
#region Private Fields
    private Cell[,] _cells;
#endregion

    /// <summary>
    /// Instantiates the map by filling cells randomly unless "fillRandomly" parameter is false.
    /// Otherwise map will be instantiated with empty cells.
    /// </summary>
    /// <param name="fillRandomly"></param>
    /// <param name="ignoreAssetIndex"></param>
    public void Instantiate(bool fillRandomly = true, int ignoreAssetIndex = -1)
    {
        if (_cells != null)
        {
            return;
        }
        
        _cells = new Cell[columnCount,rowCount];

        for (int i = 0; i < columnCount; i++)
        for (int j = 0; j < rowCount; j++)
            if (fillRandomly)
            {
                var assetIndex = Random.Range(0, gemAssetCount);
                
                if (ignoreAssetIndex > -1)
                {
                    while (assetIndex == ignoreAssetIndex && gemAssetCount > 1)
                    {
                        assetIndex = Random.Range(0, gemAssetCount);
                    }
                }

                _cells[i, j] = SetCell(i, j, CellType.Gem, assetIndex);
            }
            else
                _cells[i, j] = SetCell(i, j);
    }

    /// <summary>
    /// Clears the cells but not destroy them.
    /// </summary>
    public void ClearMap()
    {
        if (_cells == null)
        {
            return;
        }

        for (int i = 0; i < columnCount; i++)
        for (int j = 0; j < rowCount; j++)
        {
            if (_cells[i, j].cellType == CellType.Empty)
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

        _cells = null;
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
        var cell = InstantiateCell(columnIndex, rowIndex, cellType, assetIndex);
        
        if (OnCellSet != null)
        {
            OnCellSet(cell);
        }

        _cells[columnIndex, rowIndex] = cell;

        return cell;
    }

    /// <summary>
    /// Instantiates a new cell but do not insert to the map.
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    /// <param name="cellType">type of cell</param>
    /// <param name="assetIndex">asset index of selected cell type</param>
    /// <returns>cell instance</returns>
    public Cell InstantiateCell(int columnIndex, int rowIndex, CellType cellType = CellType.Empty, int assetIndex = -1)
    {
        return new Cell {
            position = CalculateCellPosition(columnIndex, rowIndex),
            assetIndex = assetIndex,
            cellType = cellType,
            columnIndex = columnIndex,
            rowIndex = rowIndex
        };
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
        if (!(_cells[columnIndex, rowIndex].assetIndex >= 0))
        {
            return;
        }
        
        if (OnCellCleared != null)
        {
            OnCellCleared(_cells[columnIndex, rowIndex]);
        }

        _cells[columnIndex, rowIndex].assetIndex = -1;
        _cells[columnIndex, rowIndex].position = Vector3.zero;
        _cells[columnIndex, rowIndex].cellType = CellType.Empty;
    }

    /// <summary>
    /// Gets the selected cell
    /// </summary>
    /// <param name="columnIndex">column index in map</param>
    /// <param name="rowIndex">rom index in map</param>
    /// <returns></returns>
    public Cell? GetCell(int columnIndex, int rowIndex)
    {
        return _cells?[columnIndex, rowIndex];
    }
    
    /// <summary>
    /// Sets multiple cells
    /// </summary>
    /// <param name="cells">filling cells' column/row coordinates list</param>
    /// <param name="cellType">cell type to fill</param>
    /// <param name="assetIndex">asset index of requested cell type</param>
    public void SetCell(List<int[]> cells, CellType cellType, int assetIndex)
    {
        for (int i = 0, l=cells.Count; i < l; i++)
        {
            SetCell(cells[i][0], cells[i][1], cellType, assetIndex);
        }
    }

    /// <summary>
    /// Returns sibling cell's offset directions according to the column index
    /// </summary>
    /// <param name="columnIndex">Parent cell's column index</param>
    /// <returns>Offsets of sibling cells</returns>
    public Vector2Int[] GetSiblingDirections(int columnIndex)
    {
        if (columnIndex % 2 == 0)
        {
            return SiblingDirectionsOdd;
        }

        return SiblingDirectionsEven;
    }

    /// <summary>
    /// Returns all matched first level sibling cells from selected cell
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public Cell[] FindMatchedSiblings(int columnIndex, int rowIndex)
    {
        ref var currentCell = ref _cells[columnIndex, rowIndex];
        HashSet<Cell> allMatches = new HashSet<Cell>();
        HashSet<Cell> matches = new HashSet<Cell>();

        var siblingDirections = GetSiblingDirections(columnIndex);
        
        for (int i = 0, l=siblingDirections.Length; i < l; i++)
        {
            var direction = siblingDirections[i];
            var targetCellColumnIndex = columnIndex + direction.x;
            var targetCellRowIndex = rowIndex + direction.y;
            
            // if exceeds the map column, then skip to next sibling
            if (targetCellColumnIndex < 0 || targetCellColumnIndex >= columnCount)
            {
                continue;
            }
            
            // if exceeds map row, then skip to next sibling
            if (targetCellRowIndex < 0 || targetCellRowIndex >= rowCount)
            {
                continue;
            }

            var targetCell = _cells[targetCellColumnIndex, targetCellRowIndex];

            if (targetCell.isChecked)
            {
                continue;
            }
            
            if (currentCell.cellType == targetCell.cellType && currentCell.assetIndex == targetCell.assetIndex)
            {
                matches.Add(targetCell);
            }
            else
            {
                if (matches.Count >= 2)
                {
                    allMatches.Add(currentCell);
                    allMatches.AddRange(matches);
                }
                
                matches.Clear();
            }
        }

        currentCell.isChecked = true;
        
        if (matches.Count >= 2)
        {
            allMatches.Add(currentCell);
            allMatches.AddRange(matches);
        }

        return allMatches.ToArray();
    }

    /// <summary>
    /// Returns all matched sibling cells from selected cell
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="rowIndex"></param>
    /// <param name="accumulatedCells"></param>
    /// <returns></returns>
    public Cell[] FindMatchedAllSiblings(int columnIndex, int rowIndex, HashSet<Cell> accumulatedCells = null)
    {
        var matches = FindMatchedSiblings(columnIndex, rowIndex);

        if (accumulatedCells == null)
        {
            accumulatedCells = new HashSet<Cell>();
        }
        
        accumulatedCells.AddRange(matches);
        
        foreach (var match in matches)
        {
            if (!match.isChecked)
            {
                FindMatchedAllSiblings(match.columnIndex, match.rowIndex, accumulatedCells);
            }
        }

        return accumulatedCells.ToArray();
    }
}