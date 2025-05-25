using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Button]
    public void Check10000Day(int year, int month, int day)
    {
        System.DateTime currentDate = new System.DateTime(year, month, day);

        currentDate = currentDate.AddDays(9999);
        Debug.LogError($"10000 days later: {currentDate.ToString("yyyy-MM-dd")}");
    }

}
