using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.PerformanceTesting;

namespace Tests.EditMode
{
    public class HexagonMapTests
    {
        private HexagonMap _map;

        [SetUp]
        public void Setup()
        {
            _map = new HexagonMap
            {
                columnCount = 10,
                rowCount = 15,
                cellWidth = 2.5f,
                cellHeight = 2.5f,
                cellHorizontalOffset = 1f,
                cellVerticalOffset = 1f,
                gemAssetCount = 5
            };
            
            _map.Instantiate(true, 0);
        }
        
        [Test]
        public void TestsCellPositionCalculator()
        {
            Assert.AreEqual(new Vector3(3, -5, 0), _map.CalculateCellPosition(2, 2));
        }

        [Test]
        public void TestsFillingTheCell()
        {
            const int columnIndex = 2;
            const int rowIndex = 2;
            const int assetIndex = 1;

            var cell = new HexagonMap.Cell()
            {
                columnIndex = columnIndex,
                rowIndex = rowIndex,
                assetIndex = assetIndex,
                cellType = HexagonMap.CellType.Gem,
                position = _map.CalculateCellPosition(columnIndex, rowIndex),
            };
            
            Assert.AreEqual(cell, _map.SetCell(columnIndex, rowIndex, HexagonMap.CellType.Gem, 1));
        }

        [Test]
        public void TestsClearingCell()
        {
            const int columnIndex = 2;
            const int rowIndex = 2;
            const int assetIndex = 0;
            
            var cell  = new HexagonMap.Cell()
            {
                columnIndex = 2,
                rowIndex = 2,
                assetIndex = -1,
                cellType = HexagonMap.CellType.Empty,
                position = Vector3.zero,
            };
            
            _map.SetCell(columnIndex, rowIndex, HexagonMap.CellType.Gem, assetIndex);
            _map.ClearCell(columnIndex, rowIndex);
            
            Assert.AreEqual(cell, _map.GetCell(columnIndex, rowIndex));
        }

        [Test]
        public void TestsMultipleCellSet()
        {
            List<int[]> cellCoordinates = new List<int[]>() {
                new [] { 0, 0},
                new [] { 1, 0},
                new [] { 0, 1},
                new [] { 1, 1},
            };

            _map.SetCell(cellCoordinates, HexagonMap.CellType.Gem, 0);
            
            // Get created cells
            var mapCell1 = _map.GetCell(0, 0);
            var mapCell2 = _map.GetCell(1, 0);
            var mapCell3 = _map.GetCell(0, 1);
            var mapCell4 = _map.GetCell(1, 1);

            // Instantiate new cells without inserting them to the map
            var cell1 = _map.InstantiateCell(0, 0, HexagonMap.CellType.Gem, 0);
            var cell2 = _map.InstantiateCell(1, 0, HexagonMap.CellType.Gem, 0);
            var cell3 = _map.InstantiateCell(0, 1, HexagonMap.CellType.Gem, 0);
            var cell4 = _map.InstantiateCell(1, 1, HexagonMap.CellType.Gem, 0);
            
            Assert.AreEqual(cell1, mapCell1);
            Assert.AreEqual(cell2, mapCell2);
            Assert.AreEqual(cell3, mapCell3);
            Assert.AreEqual(cell4, mapCell4);
        }

        [Test]
        public void TestsDirections()
        {
            Assert.AreEqual(new Vector2Int(0, -1), HexagonMap.Top);
            Assert.AreEqual(new Vector2Int(1, 0), HexagonMap.TopRightEven);
            Assert.AreEqual(new Vector2Int(1, -1), HexagonMap.TopRightOdd);
            Assert.AreEqual(new Vector2Int(1, 1), HexagonMap.BottomRightEven);
            Assert.AreEqual(new Vector2Int(1, 0), HexagonMap.BottomRightOdd);
            Assert.AreEqual(new Vector2Int(0, 1), HexagonMap.Bottom);
            Assert.AreEqual(new Vector2Int(-1, 1), HexagonMap.BottomLeftEven);
            Assert.AreEqual(new Vector2Int(-1, 0), HexagonMap.BottomLeftOdd);
            Assert.AreEqual(new Vector2Int(-1, 0), HexagonMap.TopLeftEven);
            Assert.AreEqual(new Vector2Int(-1, -1), HexagonMap.TopLeftOdd);
        }

        [Test]
        public void TestsSiblingDirections()
        {
            var siblingDirectionsEven = new [] {
                HexagonMap.Top, HexagonMap.TopRightEven, HexagonMap.BottomRightEven, HexagonMap.Bottom, HexagonMap.BottomLeftEven, HexagonMap.TopLeftEven, HexagonMap.Top
            };
            
            var siblingDirectionsOdd = new [] {
                HexagonMap.Top, HexagonMap.TopRightOdd, HexagonMap.BottomRightOdd, HexagonMap.Bottom, HexagonMap.BottomLeftOdd, HexagonMap.TopLeftOdd, HexagonMap.Top
            };
            
            
            Assert.AreEqual(siblingDirectionsEven, HexagonMap.SiblingDirectionsEven);
            Assert.AreEqual(siblingDirectionsOdd, HexagonMap.SiblingDirectionsOdd);
        }
        
        [Test]
        public void TestsFindMatchedSiblings()
        {
            List<int[]> fillCellCoordinates = new List<int[]>() {
                new [] { 0, 1},
                new [] { 1, 0},
                new [] { 1, 1},
                new [] { 2, 1},
                new [] { 2, 2},
                new [] { 0, 2},
            };
            
            _map.SetCell(fillCellCoordinates, HexagonMap.CellType.Gem, 0);

            var matchedCells =_map.FindMatchedSiblings(1, 1);

            Assert.AreEqual(fillCellCoordinates.Count, matchedCells.Length);
            
            foreach (var cellCoordinate in fillCellCoordinates)
            {
                Assert.Contains(_map.InstantiateCell(cellCoordinate[0], cellCoordinate[1], HexagonMap.CellType.Gem, 0), matchedCells);
            }
        }
        
        [Test]
        public void TestsFindMatchedAllSiblings()
        {
            List<int[]> fillCellCoordinates = new List<int[]>() {
                new [] { 0, 1},
                new [] { 1, 0},
                new [] { 1, 1},
                new [] { 2, 1},
                new [] { 2, 2},
                new [] { 0, 2},
                new [] { 2, 0},
                new [] { 3, 0},
            };
            
            _map.SetCell(fillCellCoordinates, HexagonMap.CellType.Gem, 0);

            var matchedCells =_map.FindMatchedAllSiblings(0, 1);

            Assert.AreEqual(fillCellCoordinates.Count, matchedCells.Length);
            
            foreach (var cellCoordinate in fillCellCoordinates)
            {
                Assert.Contains(_map.InstantiateCell(cellCoordinate[0], cellCoordinate[1], HexagonMap.CellType.Gem, 0), matchedCells);
            }
        }
        
        [Test, Performance]
        public void TestsMatchingPerformance()
        {
            List<int[]> fillCellCoordinates = new List<int[]>() {
                new [] { 0, 1},
                new [] { 1, 0},
                new [] { 1, 1},
                new [] { 2, 1},
                new [] { 2, 2},
                new [] { 0, 2},
                new [] { 2, 0},
                new [] { 3, 0},
            };
            
            _map.SetCell(fillCellCoordinates, HexagonMap.CellType.Gem, 0);

            Measure.Method(() =>
            {
                for (int i = 0; i < _map.columnCount; i++)
                {
                    for (int j = 0; j < _map.rowCount; j++)
                    {
                        _map.FindMatchedAllSiblings(i, j);
                    }
                }
            }).Run();
        }
    }
}
