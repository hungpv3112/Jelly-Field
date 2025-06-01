using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MiniCell[] miniCells; // 4 MiniCell, gán trong Inspector
    private Vector3 startPos;
    private Transform startParent;


    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
        startParent = transform.parent;
        transform.SetParent(transform.root); // bring to front
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // // Raycast to Cell
        // var results = new System.Collections.Generic.List<RaycastResult>();
        // EventSystem.current.RaycastAll(eventData, results);
        // foreach (var r in results)
        // {
        //     Cell cell = r.gameObject.GetComponentInParent<Cell>();
        //     if (cell != null && cell.IsEmpty())
        //     {
        //         cell.SetMiniColors(GetColors());
        //         Destroy(gameObject);
        //         // Gọi hàm merge ở đây nếu cần
        //         FindObjectOfType<MergeManager>().CheckMerge(cell.x, cell.y);
        //         return;
        //     }
        // }
        // // Nếu không thả vào ô hợp lệ, trả về vị trí cũ
        // transform.position = startPos;
        // transform.SetParent(startParent);
    }
}
