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
        _map.OnCellInstantiated += HandleCellInstantiated;
        _map.OnCellDestroyed += HandleCellDestroyed;
    }

    private void Start()
    {
        GenerateMap();
    }

    private void OnDestroy()
    {
        ClearMap();
        
        _map.OnCellInstantiated -= HandleCellInstantiated;
        _map.OnCellDestroyed -= HandleCellDestroyed;
    }
#endregion

    [Button, HideInEditorMode]
    public void GenerateMap()
    {
        _map.columnCount = columnCount;
        _map.rowCount = rowCount;
        _map.cellWidth = cellWidth;
        _map.cellHeight = cellHeight;
        _map.cellHorizontalOffset = cellHorizontalOffset;
        _map.cellVerticalOffset = cellVerticalOffset;
        _map.gemAssetCount = gems.Length;

        _map.GenerateMap();
    }

    [Button, HideInEditorMode]
    private void ClearMap()
    {
        _map.ClearMap();
    }

#region Event Handlers
    private void HandleCellInstantiated(HexagonMap.Cell cell)
    {
        var instance = Instantiate(gems[cell.assetIndex], transform);
        instance.transform.localPosition = cell.position;

        _instances[cell] = instance;
    }

    private void HandleCellDestroyed(HexagonMap.Cell cell)
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
