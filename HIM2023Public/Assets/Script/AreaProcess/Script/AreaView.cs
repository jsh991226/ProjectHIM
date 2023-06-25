using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaView : MonoBehaviour
{
    [Header("View Items")]
    public Text titleText;
    public Text descText;
    public Image titleImage;


    public void SetContent(AreaEntity _entity)
    {
        titleText.text = _entity.Title;
        descText.text = _entity.Desc;
        titleImage.sprite = _entity.TitleImage;

    }


}
