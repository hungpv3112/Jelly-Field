using System.Collections.Generic;
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

    public void SetIndexes(List<int> indexes)
    {
        _indexes.Clear();
        _indexes.AddRange(indexes);
    }
}
