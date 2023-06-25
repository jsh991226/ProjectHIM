using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSpawnPos : MonoBehaviour
{
    public GameObject target;
    public int index;

    public Vector3 GetPoint()
    {
        return target.transform.position;
    }
}
