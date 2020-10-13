using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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

#region Private Fields
    private readonly HexagonMap _map = new HexagonMap();
    private readonly Dictionary<HexagonMap.Cell, GameObject> _instances = new Dictionary<HexagonMap.Cell, GameObject>();
#endregion

#region Lifecycle Methods
    private void Awake()
    {
        _map.OnCellSet += HandleCellSet;
        _map.OnCellCleared += HandleCellCleared;
    }

    private void Start()
    {
        InstantiateMap();
    }

    private void OnDestroy()
    {
        DestroyMap();
        
        _map.OnCellSet -= HandleCellSet;
        _map.OnCellCleared -= HandleCellCleared;
    }
#endregion

    [Button, HideInEditorMode]
    public void InstantiateMap()
    {
        _map.columnCount = columnCount;
        _map.rowCount = rowCount;
        _map.cellWidth = cellWidth;
        _map.cellHeight = cellHeight;
        _map.cellHorizontalOffset = cellHorizontalOffset;
        _map.cellVerticalOffset = cellVerticalOffset;
        _map.gemAssetCount = gems.Length;

        _map.Instantiate();
    }

    [Button, HideInEditorMode]
    private void DestroyMap()
    {
        _map.DestroyMap();
    }

#region Event Handlers
    private void HandleCellSet(HexagonMap.Cell cell)
    {
        var instance = Instantiate(gems[cell.assetIndex], transform);
        instance.transform.localPosition = cell.position;

        _instances[cell] = instance;
    }

    private void HandleCellCleared(HexagonMap.Cell cell)
    {
        var instance = _instances[cell];
        
        if (instance != null)
        {
            DestroyImmediate(instance);
            _instances[cell] = null;
            _instances.Remove(cell);
        }
    }
#endregion
}
