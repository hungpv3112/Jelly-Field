using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public bool canRotate;
    public bool isLocal;

    [SerializeField] private Vector3 rotate = new Vector3(0, 0, 100);
    public Vector3 Rotate { get => rotate; set { rotate = value; } }

    private void Update()
    {
        if (canRotate)
            transform.Rotate(rotate * Time.deltaTime, isLocal ? Space.Self : Space.World);
    }

    public void StartRotate()
    {
        canRotate = true;
    }

    public void StopRotate()
    {
        canRotate = false;
    }

}
