using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("disablestart");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("disable Update");
    }

    public void DisableMethod()
    {
        Debug.Log("Run Method");
    }
}
