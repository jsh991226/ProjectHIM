using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrcIconCtrl : MonoBehaviour
{
    public Sprite orcEnable;
    public Sprite orcDisable;

    private bool isEnable;


    public void SetAble(bool _type)
    {
        isEnable = _type;
        if (isEnable) gameObject.GetComponent<Image>().sprite = orcEnable;
        else gameObject.GetComponent<Image>().sprite = orcDisable;
    }

}
