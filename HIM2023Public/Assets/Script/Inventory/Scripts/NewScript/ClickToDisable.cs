using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToDisable : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public void GUIDisable()
    {
        target.SetActive(false);
    }
}
