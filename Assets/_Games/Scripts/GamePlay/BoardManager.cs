using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    private const float cellSize = 2.18f;

    public int rows = 5, cols = 5;
    public GameObject cellPrefab;
    public GameObject floorPrefab;
    public Transform boardParent;

    //[HideInInspector]
    public List<Cells> cellGird = new List<Cells>();

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        cellGird = new List<Cells>();
        for (int i = 0; i < rows; i++)
        {
            Cells cells = new Cells(cols);
            cellGird.Add(cells);
        }
        var offsetX = (cols - 1) * cellSize / 2f;
        var offsetZ = (rows - 1) * cellSize / 2f;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // GameObject cellObj = Instantiate(cellPrefab, boardParent);
                // cellObj.transform.localPosition = new Vector3(x * cellSize, y * -cellSize, 0);
                // Cell cell = cellObj.GetComponent<Cell>();
                // cell.Init(x, y, this);
                // cells[y, x] = cell;

                GameObject floor = Instantiate(floorPrefab, boardParent);
                floor.transform.localPosition = new Vector3(x * cellSize - offsetX, 0, y * cellSize - offsetZ);
            }
        }
    }

    [Button]
    public Vector3 GetPosition(int row, int col)
    {
        float offsetX = (cols - 1) * cellSize / 2f;
        float offsetZ = (rows - 1) * cellSize / 2f;
        return new Vector3(col * cellSize - offsetX, 0, row * cellSize - offsetZ);
    }

    public bool IsCellEmpty(int row, int col)
    {
        return cellGird[row].cells[col] == null;
    }

    public bool PutCell(Cell cell)
    {
        float minDist = float.MaxValue;
        int targetRow = -1, targetCol = -1;

        Vector3 cellPos = cell.transform.position;

        // Tìm ô gần nhất trong board
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 gridPos = GetPosition(row, col);
                cellPos.y = 0;
                float dist = Vector3.Distance(cellPos, gridPos);
                if (dist <= 1f && IsCellEmpty(row, col) && dist < minDist)
                {
                    minDist = dist;
                    targetRow = row;
                    targetCol = col;
                }
            }
        }

        if (targetRow != -1 && targetCol != -1)
        {
            // Đặt cell vào vị trí tìm được
            cellGird[targetRow].cells[targetCol] = cell;
            cell.transform.position = GetPosition(targetRow, targetCol);
            cell.x = targetCol;
            cell.y = targetRow;

            cellGird[targetRow].cells[targetCol] = cell;
            cell.transform.SetParent(boardParent);
            CheckMerge(new List<Cell> { cell });
            return true;
        }

        return false; // Không tìm được vị trí phù hợp
    }

    Dictionary<Cell, List<MiniCell>> miniCellToRemove = new Dictionary<Cell, List<MiniCell>>();
    Dictionary<int, List<MiniCell>> miniCellToRemoveByColor = new Dictionary<int, List<MiniCell>>();
    public void CheckMerge(List<Cell> cells)
    {
        miniCellToRemove.Clear();
        foreach (var cell in cells)
        {
            if (cell != null)
            {
                CheckMerge(cell.x, cell.y);
            }
        }

        if (miniCellToRemove.Count == 0)
        {
            return; // Không có mini cell nào cần xóa
        }

        // Thực hiện xóa các mini cell khớp màu
        List<Cell> cellsNeedCheck = new List<Cell>();
        foreach (var match in miniCellToRemove)
        {
            cellsNeedCheck.Add(match.Key);
            var indexesRemove = new List<int>();
            foreach (var miniCell in match.Value)
            {
                if (miniCell != null && miniCell.gameObject.activeSelf)
                {
                    foreach (var item in miniCell.GetIndexes())
                    {
                        if (indexesRemove.Contains(item) == false)
                        {
                            indexesRemove.Add(item);
                        }
                    }
                }
            }

            match.Key.ClearMiniCell1(indexesRemove);
            foreach (var item in match.Value)
            {
                if (item != null && item.gameObject.activeSelf)
                {
                    if (miniCellToRemoveByColor.ContainsKey(item.ColorIndex) == false)
                    {
                        miniCellToRemoveByColor[item.ColorIndex] = new List<MiniCell>();
                    }
                    if (miniCellToRemoveByColor[item.ColorIndex].Contains(item) == false)
                    {
                        miniCellToRemoveByColor[item.ColorIndex].Add(item);
                    }
                }
            }
        }

        IEnumerator CheckEmptyCells()
        {
            yield return new WaitForSeconds(0.26f); // Đợi một chút để các hiệu ứng xóa hoàn tất
                                                    // Kiểm tra xem có cell nào đã empty hoàn toàn
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Cell cell = cellGird[r].cells[c];
                    if (cell != null && cell.IsEmpty())
                    {
                        Destroy(cell.gameObject);
                        cellGird[r].cells[c] = null;
                    }
                }
            }

            if (cellsNeedCheck.Count > 0)
            {
                yield return new WaitForSeconds(0.2f);
                // Gọi lại hàm CheckMerge cho các cell cần kiểm tra
                CheckMerge(cellsNeedCheck);
            }
        }

        StartCoroutine(CheckEmptyCells());

    }


    [Button]
    public void CheckMerge(int col, int row)
    {
        Cell currentCell = cellGird[row].cells[col];
        if (currentCell == null) return;

        // Kiểm tra ô bên dưới
        if (row > 0)
        {
            Cell botCell = cellGird[row - 1].cells[col];
            if (botCell != null)
            {
                FindMatchingColors(currentCell, botCell, 0, 2, miniCellToRemove);
                FindMatchingColors(currentCell, botCell, 1, 3, miniCellToRemove);
            }
        }

        // Kiểm tra ô bên trên
        if (row < rows - 1)
        {
            Cell topCell = cellGird[row + 1].cells[col];
            if (topCell != null)
            {
                FindMatchingColors(currentCell, topCell, 2, 0, miniCellToRemove);
                FindMatchingColors(currentCell, topCell, 3, 1, miniCellToRemove);
            }
        }

        // Kiểm tra ô bên trái
        if (col > 0)
        {
            Cell leftCell = cellGird[row].cells[col - 1];
            if (leftCell != null)
            {
                FindMatchingColors(currentCell, leftCell, 0, 1, miniCellToRemove);
                FindMatchingColors(currentCell, leftCell, 2, 3, miniCellToRemove);
            }
        }

        // Kiểm tra ô bên phải
        if (col < cols - 1)
        {
            Cell rightCell = cellGird[row].cells[col + 1];
            if (rightCell != null)
            {
                FindMatchingColors(currentCell, rightCell, 1, 0, miniCellToRemove);
                FindMatchingColors(currentCell, rightCell, 3, 2, miniCellToRemove);
            }
        }

    }

    private void FindMatchingColors(Cell cell1, Cell cell2, int miniCellIndex1, int miniCellIndex2, Dictionary<Cell, List<MiniCell>> miniCellToRemove)
    {
       
        // Lấy id màu để so sánh
        var id1 = cell1.Data.itemIds[miniCellIndex1];
        var id2 = cell2.Data.itemIds[miniCellIndex2];

        if (id1 == id2)
        {
            if (miniCellToRemove.ContainsKey(cell1) == false)
            {
                miniCellToRemove[cell1] = new List<MiniCell>();
            }
            if (miniCellToRemove.ContainsKey(cell2) == false)
            {
                miniCellToRemove[cell2] = new List<MiniCell>();
            }
            if (miniCellToRemove[cell1].Contains(cell1.GetMiniCell(miniCellIndex1)) == false)
            {
                miniCellToRemove[cell1].Add(cell1.GetMiniCell(miniCellIndex1));
            }
            if (miniCellToRemove[cell2].Contains(cell2.GetMiniCell(miniCellIndex2)) == false)
            {
                miniCellToRemove[cell2].Add(cell2.GetMiniCell(miniCellIndex2));
            }

        }
    }

    private int GetMiniCellIndex(Cell cell, int cellRow, int cellCol, int adjacentRow, int adjacentCol)
    {
        // Xác định mini cell index dựa trên vị trí tương đối
        int index = -1;
        
        // Nếu ô kề bên trên
        if (adjacentRow == cellRow - 1 && adjacentCol == cellCol)
        {
            // Lấy mini cell ở hàng trên của cell hiện tại (index 0 hoặc 1)
            index = cell.GetIndex(0, cellCol % 2 == 0 ? 0 : 1);
        }
        // Nếu ô kề bên dưới
        else if (adjacentRow == cellRow + 1 && adjacentCol == cellCol)
        {
            // Lấy mini cell ở hàng dưới của cell hiện tại (index 2 hoặc 3)
            index = cell.GetIndex(1, cellCol % 2 == 0 ? 0 : 1);
        }
        // Nếu ô kề bên trái
        else if (adjacentRow == cellRow && adjacentCol == cellCol - 1)
        {
            // Lấy mini cell ở cột trái của cell hiện tại (index 0 hoặc 2)
            index = cell.GetIndex(cellRow % 2 == 0 ? 0 : 1, 0);
        }
        // Nếu ô kề bên phải
        else if (adjacentRow == cellRow && adjacentCol == cellCol + 1)
        {
            // Lấy mini cell ở cột phải của cell hiện tại (index 1 hoặc 3)
            index = cell.GetIndex(cellRow % 2 == 0 ? 0 : 1, 1);
        }
        
        return index;
    }
}

[Serializable]
public class Cells
{
    public Cell[] cells;

    public Cells(int row)
    {
        this.cells = new Cell[row];
    }
}
