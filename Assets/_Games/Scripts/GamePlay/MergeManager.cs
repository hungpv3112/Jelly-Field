using UnityEngine;

public class MergeManager : MonoBehaviour
{
    public BoardManager board;

    public void CheckMerge(int x, int y)
    {
        //Cell cell = board.cells[y, x];
        for (int i = 0; i < 4; i++)
        {
            // Color color = cell.miniCells[i].GetColor();
            // if (color == Color.clear) continue;

            // // Kiểm tra 4 hướng
            // foreach (var dir in new Vector2Int[] {
            //     new Vector2Int(0, 1), new Vector2Int(0, -1),
            //     new Vector2Int(1, 0), new Vector2Int(-1, 0)
            // })
            // {
            //     int nx = x + dir.x, ny = y + dir.y;
            //     if (nx >= 0 && nx < board.cols && ny >= 0 && ny < board.rows)
            //     {
            //         Cell neighbor = board.cells[ny, nx];
            //         for (int j = 0; j < 4; j++)
            //         {
            //             if (neighbor.miniCells[j].GetColor() == color)
            //             {
            //                 // Merge: xóa cả hai miniCell
            //                 cell.miniCells[i].SetColor(Color.clear);
            //                 neighbor.miniCells[j].SetColor(Color.clear);
            //                 // Có thể thêm hiệu ứng ở đây
            //             }
            //         }
            //     }
            // }
        }
    }
}
