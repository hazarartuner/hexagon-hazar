using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
    }
}
