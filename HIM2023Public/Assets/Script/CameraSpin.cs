using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{

    public float speed = 100f;
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 10.0f * Time.deltaTime * speed, 0));
    }
}
