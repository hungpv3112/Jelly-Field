using UnityEngine;

public class MaterialsContainer : Singleton<MaterialsContainer>
{
    [SerializeField] private Material[] materials;

    [SerializeField] private Material[] materialsHighlighted;

    public Material GetMaterial(int index)
    {
        if (index < 0 || index >= materials.Length)
        {
            Debug.LogError("Index out of bounds for materials array.");
            return null;
        }
        return materials[index];
    }

    public Material GetMaterialHighlighted(int index)
    {
        if (index < 0 || index >= materialsHighlighted.Length)
        {
            Debug.LogError("Index out of bounds for materials array.");
            return null;
        }
        return materialsHighlighted[index];
    }

    public int GetMaterialsCount()
    {
        return materials.Length;
    }
}
