using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject _miniCellPrefab;
    [SerializeField] private CellData _data;

    public CellData Data
    {
        get => _data;
    }

    public List<MiniCell> miniCells;
    public int x, y;


    public bool IsEmpty()
    {
        return _data.itemIds.TrueForAll(id => id < 0);
    }

    public MiniCell GetMiniCell(int index)
    {
        foreach (var miniCell in miniCells)
        {
            if (miniCell != null && miniCell.GetIndexes().Contains(index))
            {
                return miniCell;
            }
        }

        return null;
    }

    [Button]
    public List<int> GetIndexNeighbors(int index)
    {
        var (r, c) = GetRowCol(index);
        List<int> neighbors = new List<int>();

        // Kiểm tra 4 hướng: trên, dưới, trái, phải
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (Mathf.Abs(dr) + Mathf.Abs(dc) == 1) // Chỉ lấy các ô lân cận trực tiếp
                {
                    int rN = r + dr;
                    int cN = c + dc;

                    if (rN >= 0 && rN <= 1 && cN >= 0 && cN <= 1)
                    {
                        neighbors.Add(GetIndex(rN, cN));
                    }
                }
            }
        }

        return neighbors;
    }

    [Button]
    public void ClearMiniCell1(List<int> indexs)
    {
        // Tìm miniCell có chứa index cần clear
        List<MiniCell> miniCellsToClear = new List<MiniCell>();
        foreach (var index in indexs)
        {
            var miniCellToClear = GetMiniCell(index);
            if (miniCellsToClear.Contains(miniCellToClear))
            {
                // Nếu miniCell đã được thêm vào danh sách, không cần thêm lại
                continue;
            }

            miniCellsToClear.Add(miniCellToClear);
        }


        // Lấy danh sách tất cả indexes mà miniCell này sở hữu
        List<int> indexesToClear = new List<int>();
        foreach (var miniCellToClear in miniCellsToClear)
        {
            if (miniCellToClear != null)
            {
                foreach (var item in miniCellToClear.GetIndexes())
                {
                    if (!indexesToClear.Contains(item))
                    {
                        indexesToClear.Add(item);
                    }
                }

            }
        }

        // Cập nhật data.itemIds cho mỗi index bị xóa thành -1
        foreach (var idx in indexesToClear)
        {
            if (idx >= 0 && idx < _data.itemIds.Count)
            {
                _data.itemIds[idx] = -1;
            }
        }

        // Xóa miniCell
        foreach (var miniCellToClear in miniCellsToClear)
        {
            miniCellToClear.gameObject.SetActive(false);
            miniCells.Remove(miniCellToClear);
        }

        // Nếu miniCell này sở hữu cả 4 slot, không cần fill
        if (indexesToClear.Count == 4)
        {
            // Đã set tất cả thành -1 ở trên
            return;
        }

        var timeTween = 0.25f;

        void SetOneMiniCell()
        {
            
            var miniCell = miniCells[0];
            miniCell.transform.DOLocalMoveZ(0, timeTween);
            miniCell.transform.DOLocalMoveX(0, timeTween);
            var meshSlice = miniCell.GetComponent<MeshSlice>();
            DOTween.To(() => meshSlice.x2, x => meshSlice.x2 = x, -0.061f, timeTween);
            DOTween.To(() => meshSlice.x3, x => meshSlice.x3 = x, 0.061f, timeTween);
            DOTween.To(() => meshSlice.z2, x => meshSlice.z2 = x, -0.061f, timeTween);
            DOTween.To(() => meshSlice.z3, x => meshSlice.z3 = x, 0.061f, timeTween);
            var id = miniCell.GetIndexes()[0];
            _data.itemIds = new List<int>() { id, id, id, id };
            miniCell.SetIndexes(new List<int>() { 0, 1, 2, 3 });
        }

        if (indexesToClear.Count == 3)
        {
            SetOneMiniCell();
        }
        else if (indexesToClear.Count == 2)
        {
            if (miniCells.Count == 1)
            {
                SetOneMiniCell();
            }
            else
            {
                foreach (var item in indexesToClear)
                {
                    var indexNeighbors = GetIndexNeighbors(item);
                    foreach (var indexNeighbor in indexNeighbors)
                    {
                        var miniCell = GetMiniCell(indexNeighbor);
                        if (miniCell == null || miniCell.GetIndexes().Count > 1)
                        {
                            continue;
                        }
                        bool isVertical = Mathf.Abs(indexNeighbor - item) == 2;
                        if (isVertical)
                        {
                            miniCell.transform.DOLocalMoveZ(0, timeTween);
                            var meshSlice = miniCell.GetComponent<MeshSlice>();
                            DOTween.To(() => meshSlice.z2, x => meshSlice.z2 = x, -0.061f, timeTween);
                            DOTween.To(() => meshSlice.z3, x => meshSlice.z3 = x, 0.061f, timeTween);
                        }
                        else
                        {
                            miniCell.transform.DOLocalMoveX(0, timeTween);
                            var meshSlice = miniCell.GetComponent<MeshSlice>();
                            DOTween.To(() => meshSlice.x2, x => meshSlice.x2 = x, -0.061f, timeTween);
                            DOTween.To(() => meshSlice.x3, x => meshSlice.x3 = x, 0.061f, timeTween);
                        }
                        miniCell.GetIndexes().Add(item);
                        _data.itemIds[item] = miniCell.GetIndexes()[0];
                        break;
                    }
                }
            }
        }
        else
        {
            var indexNeighbors = GetIndexNeighbors(indexesToClear[0]);
            foreach (var indexNeighbor in indexNeighbors)
            {
                var miniCell = GetMiniCell(indexNeighbor);
                if (miniCell == null || miniCell.GetIndexes().Count > 1)
                {
                    continue;
                }
                bool isVertical = Mathf.Abs(indexNeighbor - indexesToClear[0]) == 2;
                if (isVertical)
                {
                    miniCell.transform.DOLocalMoveZ(0, timeTween);
                    var meshSlice = miniCell.GetComponent<MeshSlice>();
                    DOTween.To(() => meshSlice.z2, x => meshSlice.z2 = x, -0.061f, timeTween);
                    DOTween.To(() => meshSlice.z3, x => meshSlice.z3 = x, 0.061f, timeTween);
                }
                else
                {
                    miniCell.transform.DOLocalMoveX(0, timeTween);
                    var meshSlice = miniCell.GetComponent<MeshSlice>();
                    DOTween.To(() => meshSlice.x2, x => meshSlice.x2 = x, -0.061f, timeTween);
                    DOTween.To(() => meshSlice.x3, x => meshSlice.x3 = x, 0.061f, timeTween);
                }
                miniCell.GetIndexes().Add(indexesToClear[0]);
                _data.itemIds[indexesToClear[0]] = miniCell.GetIndexes()[0];
                break;
            }
        }
    }

    public void ClearMiniCell(List<MiniCell> miniCells)
    {
        List<int> indexesRemove = new List<int>();
        foreach (var miniCell in miniCells)
        {
            if (miniCell != null)
            {
                foreach (var item in miniCell.GetIndexes())
                {
                    if (!indexesRemove.Contains(item))
                    {
                        indexesRemove.Add(item);
                    }
                }
            }
        }
        ClearMiniCell1(indexesRemove);
    }

    [Button]
    public void ClearMiniCell(int index)
    {
        var (r, c) = GetRowCol(index);

        int rN, cN;

        if (RandomUtils.RangeInt(0, 2) == 0)
        {
            rN = r == 0 ? 1 : 0;
            cN = c;
        }
        else
        {
            cN = c == 0 ? 1 : 0;
            rN = r;
        }

        int neighborIndex = GetIndex(rN, cN);
        GetMiniCell(index).gameObject.SetActive(false);

        var miniCell = GetMiniCell(neighborIndex);
        bool verticel = Mathf.Abs(neighborIndex - index) == 2;

        if (verticel)
        {
            float timeTween = 0.25f;
            miniCell.transform.DOLocalMoveZ(0, timeTween);
            var meshSlice = miniCell.GetComponent<MeshSlice>();
            DOTween.To(() => meshSlice.z2, x => meshSlice.z2 = x, -0.061f, timeTween);
            DOTween.To(() => meshSlice.z3, x => meshSlice.z3 = x, 0.061f, timeTween);
        }
        else
        {
            float timeTween = 0.25f;
            miniCell.transform.DOLocalMoveX(0, timeTween);
            var meshSlice = miniCell.GetComponent<MeshSlice>();
            DOTween.To(() => meshSlice.x2, x => meshSlice.x2 = x, -0.061f, timeTween);
            DOTween.To(() => meshSlice.x3, x => meshSlice.x3 = x, 0.061f, timeTween);
        }
    }



    public (int r, int c) GetRowCol(int index)
    {
        int r = index / 2;
        int c = index % 2;
        return (r, c);
    }

    public int GetIndex(int r, int c)
    {
        if (r < 0 || r > 1 || c < 0 || c > 1)
        {
            throw new ArgumentOutOfRangeException("Row and column must be in the range [0, 1]");
        }
        return r * 2 + c;
    }

    [Button]
    public void SetUpMiniCells()
    {
        // Xóa mini cell cũ
        foreach (var item in miniCells)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        miniCells.Clear();

        var ids = _data.itemIds;
        if (ids == null || ids.Count < 4) return;

        // Nếu tất cả giống nhau
        if (ids[0] == ids[1] && ids[1] == ids[2] && ids[2] == ids[3])
        {
            var miniCell = CreateMiniCell(new Vector3(0, 0.53f, 0), new Vector4(-MeshSlice.scaleSize, MeshSlice.scaleSize, -MeshSlice.scaleSize, MeshSlice.scaleSize), ids[0]);
            miniCell.SetIndexes(new List<int>(){ 0, 1, 2, 3 });
            return;
        }

        List<int> indexNeedFill = new List<int>(){ 0, 1, 2, 3 };
        // Merge theo hàng
        if (ids[0] == ids[1])
        {
            var miniCell1 = CreateMiniCell(new Vector3(0, 0.53f, -0.53f), new Vector4(-MeshSlice.scaleSize, MeshSlice.scaleSize, -MeshSlice.baseSize, MeshSlice.baseSize), ids[0]);
            miniCell1.SetIndexes(new List<int>() { 0, 1 });
            indexNeedFill.Remove(0);
            indexNeedFill.Remove(1);
        }
        if (ids[2] == ids[3])
        {
            var miniCell1 = CreateMiniCell(new Vector3(0, 0.53f, 0.53f), new Vector4(-MeshSlice.scaleSize, MeshSlice.scaleSize, -MeshSlice.baseSize, MeshSlice.baseSize), ids[2]);
            miniCell1.SetIndexes(new List<int>(){ 2, 3 });
            indexNeedFill.Remove(2);
            indexNeedFill.Remove(3);
        }

        // Merge theo cột
        if (ids[0] == ids[2])
        {
            var miniCell1 = CreateMiniCell(new Vector3(-0.53f, 0.53f, 0), new Vector4(-MeshSlice.baseSize, MeshSlice.baseSize, -MeshSlice.scaleSize, MeshSlice.scaleSize), ids[0]);
            miniCell1.SetIndexes(new List<int>(){ 0, 2 });
            indexNeedFill.Remove(0);
            indexNeedFill.Remove(2);
        }
        if (ids[1] == ids[3])
        {
            var miniCell1 = CreateMiniCell(new Vector3(0.53f, 0.53f, 0), new Vector4(-MeshSlice.baseSize, MeshSlice.baseSize, -MeshSlice.scaleSize, MeshSlice.scaleSize), ids[1]);
            miniCell1.SetIndexes(new List<int>(){ 1, 3 });
            indexNeedFill.Remove(1);
            indexNeedFill.Remove(3);
        }
        
        foreach (var item in indexNeedFill)
        {
            int r = item / 2;
            int c = item % 2;
            var miniCell = CreateMiniCell(new Vector3(-0.53f + 1.06f * c, 0.53f, -0.53f + 1.06f * r), new Vector4(-MeshSlice.baseSize, MeshSlice.baseSize, -MeshSlice.baseSize, MeshSlice.baseSize), ids[item]);
            miniCell.SetIndexes(new List<int>() { item });
        }

        // Hàm tạo mini cell
        MiniCell CreateMiniCell(Vector3 pos, Vector4 scale, int id)
        {
            var miniCell = Instantiate(_miniCellPrefab, transform).GetComponent<MiniCell>();
            miniCell.transform.localPosition = pos;
            var meshSlice = miniCell.GetComponent<MeshSlice>();
            meshSlice.x2 = scale.x;
            meshSlice.x3 = scale.y;
            meshSlice.z2 = scale.z;
            meshSlice.z3 = scale.w;
            meshSlice.Slice();

            miniCell.SetMaterial(MaterialsContainer.Instance.GetMaterial(id));
            miniCells.Add(miniCell);
            return miniCell;
        }
    }


    [Button]
    public void GenNewData()
    {
        var maxColor = Enum.GetValues(typeof(ColorType)).Length;
        var listColorsId = new List<int>();
        for (int i = 0; i < maxColor; i++)
        {
            listColorsId.Add(i);
        }
        _data = new CellData();
        _data.itemIds = new List<int>(RandomFourWithPairDiff(maxColor));

        SetUpMiniCells();
    }

    /// <summary>
    /// Random 4 int trong khoảng [0, range), đảm bảo cặp (0,3) và (1,2) khác nhau.
    /// </summary>
    public static int[] RandomFourWithPairDiff(int range)
    {
        int[] result = new int[4];
        result[0] = RandomUtils.RangeInt(0, range);
        result[1] = RandomUtils.RangeInt(0, range);

        // random result[2] khác result[1]
        do
        {
            result[2] = RandomUtils.RangeInt(0, range);
        } while (result[2] == result[1]);

        // random result[3] khác result[0]
        do
        {
            result[3] = RandomUtils.RangeInt(0, range);
        } while (result[3] == result[0]);

        return result;
    }

}

[Serializable]
public class CellData
{
    public List<int> itemIds;

}
