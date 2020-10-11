using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [FoldoutGroup("Map Settings", true)]
    public int columnCount;
    [FoldoutGroup("Map Settings", true)]
    public int rowCount;
    [FoldoutGroup("Map Settings", true)]
    public float cellWidth = 1f;
    [FoldoutGroup("Map Settings", true)]
    public float cellHeight = 1f;
    [FoldoutGroup("Map Settings", true)]
    public float cellHorizontalOffset = 0.25f;
    [FoldoutGroup("Map Settings", true)]
    public float cellVerticalOffset = 0.25f;


    [FoldoutGroup("Prefabs", true), Required]
    public GameObject bomb;
    [FoldoutGroup("Prefabs", true)]
    public GameObject[] gems;
    
    public enum InstanceType { Empty, Gem, Bomb }

    [Serializable]
    public struct Cell
    {
        public GameObject instance;
        public Transform transform;
        public InstanceType type;
        public int gemIndex;
        public int bombIndex;
    }
    
#region Private Fields
    private Transform _transform;
    private Cell[,] _map;
    private int _gemAssetCount;
#endregion

    private void Start()
    {
        _transform = transform;
        _gemAssetCount = gems.Length;
        
        GenerateMap();
    }

    [Button]
    public void GenerateMap()
    {
        ClearMap();

        _gemAssetCount = gems.Length;
        _map = new Cell[columnCount,rowCount];

        for (int i = 0; i < columnCount; i++)
            for (int j = 0; j < rowCount; j++)
                _map[i, j] = GenerateGemInstance(i, j);
    }

    [Button]
    private void ClearMap()
    {
        if (_map == null)
        {
            return;
        }

        for (int i = 0; i < columnCount; i++)
            for (int j = 0; j < rowCount; j++)
                DestroyCellInstance(i, j);

        _map = null;
    }

    private Cell GenerateGemInstance(int columnIndex, int rowIndex, int gemIndex = -1)
    {
        if (gemIndex < 0)
        {
            gemIndex = Random.Range(0, _gemAssetCount);
        }
        
        var instance = Instantiate(gems[gemIndex], _transform);

        instance.transform.localPosition = CalculateCellPosition(columnIndex, rowIndex);
        
        return new Cell {
                instance = instance, 
                transform = instance.transform, 
                type = InstanceType.Gem, 
                gemIndex = gemIndex
            };
    }

    private Vector3 CalculateCellPosition(int columnIndex, int rowIndex)
    {
        return new Vector3(
            columnIndex * cellWidth - columnIndex * cellHorizontalOffset,
            -(rowIndex * cellHeight + (columnIndex % 2) * cellVerticalOffset),
            0
        );
    }

    private void DestroyCellInstance(int columnIndex, int rowIndex)
    {
        if (!(_map[columnIndex, rowIndex].type > 0))
        {
            return;
        }
        
        DestroyImmediate(_map[columnIndex, rowIndex].instance);
                    
        _map[columnIndex, rowIndex].instance = null;
        _map[columnIndex, rowIndex].transform = null;
        _map[columnIndex, rowIndex].gemIndex = -1;
        _map[columnIndex, rowIndex].bombIndex = -1;
    }
}
