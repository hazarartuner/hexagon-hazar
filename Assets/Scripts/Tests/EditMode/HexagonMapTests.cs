using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

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
                columnCount = 5,
                rowCount = 5,
                cellWidth = 2.5f,
                cellHeight = 2.5f,
                cellHorizontalOffset = 1f,
                cellVerticalOffset = 1f
            };
            
            _map.Instantiate(false);
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
    }
}
