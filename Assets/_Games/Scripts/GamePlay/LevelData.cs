using System;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    
}

public class BoardData
{

}

[Serializable]
public class CellUnit
{
    public Position pos;
    public CellData itemData;
}

[Serializable]
public class Position
{
    public int x;
    public int y;
}