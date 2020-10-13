using NUnit.Framework;
using UnityEngine;

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
                cellWidth = 2.5f,
                cellHeight = 2.5f,
                cellHorizontalOffset = 1f,
                cellVerticalOffset = 1f
            };
        }
        
        [Test]
        public void TestsCellPositionCalculator()
        {
            Assert.AreEqual(new Vector3(3, -5, 0), _map.CalculateCellPosition(2, 2));
        }

        [Test]
        public void TestsGemInstantiationInCell()
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
            
            Assert.AreEqual(cell, _map.InstantiateCell(columnIndex, rowIndex, HexagonMap.CellType.Gem, 1));
        }
    }
}
