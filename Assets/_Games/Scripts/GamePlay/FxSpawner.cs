using Sirenix.OdinInspector;
using UnityEngine;

public class FxSpawner : Singleton<FxSpawner>
{
    [SerializeField] private ParticleSystem fxPrefab;
    [SerializeField] private Color[] colors;

    [Button]
    public void SpawnFx(Vector3 position, int color)
    {
        if (fxPrefab != null)
        {
            ParticleSystem fxInstance = Instantiate(fxPrefab, position, Quaternion.identity);
            var main = fxInstance.main;
            main.startColor = colors[color];
            fxInstance.Play();
            Destroy(fxInstance.gameObject, fxInstance.main.duration);
        }
        else
        {
            Debug.LogWarning("FX Prefab is not assigned in FxSpawner.");
        }
    }
}
