using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MiniCell : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [ShowInInspector] private List<int> _indexes = new List<int>();
    public List<int> GetIndexes()
    {
        return _indexes;
    }

    private int _colorIndex = 0;
    public int ColorIndex => _colorIndex;    

    void Awake()
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
    }


    public void SetMaterial(Material material, int colorIndex)
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.material = material;
        }
        _colorIndex = colorIndex;
    }

    public void Highlight()
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.material = MaterialsContainer.Instance.GetMaterialHighlighted(_colorIndex);
            transform.DOScale(13.5f, 0.2f);
            transform.DOLocalMoveY(0.6f, 0.2f)
                .SetEase(Ease.OutBack);
        }
    }

    public void UnHighlight()
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.material = MaterialsContainer.Instance.GetMaterial(_colorIndex);
        }
        transform.localScale = Vector3.one * 13;
        transform.localPosition = new Vector3(transform.localPosition.x, 0.53f, transform.localPosition.z);
        DOTween.Kill(transform);
    }

    public void SetIndexes(List<int> indexes)
    {
        _indexes.Clear();
        _indexes.AddRange(indexes);
    }
}
