using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemQueueManager : MonoBehaviour
{
    [SerializeField] private Cell _cellPrefab;

    public List<ItemQueue> itemQueues;


    private Cell _currentCell;
    private Vector3 _posClicked;
    private Vector3 _posStarted;
    private float _zOffset = 0f; // Offset to move the cell up when clicked
    private ItemQueue _currentItemQueue;
    private float _mul = 1.3f;

    void Awake()
    {
        foreach (var itemQueue in itemQueues)
        {
            GenNewCell(itemQueue);
        }
    }

    private void GenNewCell(ItemQueue itemQueue)
    {
        if (itemQueue.partent != null)
        {
            var cell = Instantiate(_cellPrefab, itemQueue.partent.transform);
            cell.transform.localPosition = Vector3.zero; // Set initial position to zero
            cell.GenNewData();
            itemQueue.cell = cell;
        }
        else
        {
            Debug.LogWarning("Parent is null for ItemQueue: " + itemQueue);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastToFindItemQueue();
            if (_currentCell != null)
            {
                _zOffset = 0;
                _posClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _posStarted = _currentCell.transform.position;
                DOTween.To(() => _zOffset, 
                    z => _zOffset = z,
                    2, 0.25f)
                    .SetEase(Ease.OutQuad);
            }
        }

        if (Input.GetMouseButton(0) && _currentCell != null)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = _posStarted + (mousePosition - _posClicked) * _mul;
            pos.y = 1;
            pos.z += _zOffset; // Ensure the z position is zero
            _currentCell.transform.position = pos;
        }

        if (Input.GetMouseButtonUp(0) && _currentCell != null)
        {
            if (BoardManager.Instance.PutCell(_currentCell))
            {
                _currentCell = null;
                GenNewCell(_currentItemQueue); // Generate a new cell for the first item queue
            }
            else
            {
                _currentCell.transform.DOLocalMove(Vector3.zero, 0.25f)
                    .SetEase(Ease.OutQuad);
            }

            if (_currentCell != null)
            {
                _currentCell.transform.localPosition = Vector3.zero; // Reset position to zero when mouse button is released
            }
            _currentItemQueue = null; // Clear the current item queue reference
            _currentCell = null;
            _zOffset = 0f; // Reset the offset when the mouse button is released
        }
    }

    void RaycastToFindItemQueue()
    {
        _posClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            foreach (var itemQueue in itemQueues)
            {
                if (itemQueue.partent != null && hit.transform.gameObject == itemQueue.partent)
                {
                    _currentCell = itemQueue.cell;
                    _currentItemQueue = itemQueue;
                    break;
                }
            }
        }
    }
}

[Serializable]
public class ItemQueue
{
    public GameObject partent;

    public Cell cell;

}
