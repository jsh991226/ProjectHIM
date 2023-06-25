using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardEntity : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField]
    private Text nickNameText;
    [SerializeField]
    private Image iconObject;
    [SerializeField]
    List<Text> descLine;
    [SerializeField]
    private Image borderFrame;



    [Header("Icons")]
    [SerializeField]
    private Sprite imgGoblin;
    [SerializeField]
    private Sprite imgGolem;
    [SerializeField]
    private Sprite imgOgre;
    [SerializeField]
    private Sprite imgOrk;
    [SerializeField]
    private Sprite imgSkeleton;

    private Dictionary<string, Sprite> classImg;


    private string myClass;


    private void SetImg()
    {
        classImg = new Dictionary<string, Sprite>();
        classImg.Add("Goblin", imgGoblin);
        classImg.Add("Golem", imgGolem);
        classImg.Add("Ogre", imgOgre);
        classImg.Add("Ork", imgOrk);
        classImg.Add("Skeleton", imgSkeleton);

    }

    public void ReadyBorder(bool _type)
    {
        if (!_type)
        {
            Color32 color = new Color32(255, 255, 255, 255);
            borderFrame.color = color;
        } else
        {
            Color32 color = new Color32(255, 79, 0,255);
            borderFrame.color = color;
        }
    }


    public void GUISetting(string _class, string _nickname, string _owner, int _ready)
    {
        if (classImg == null) SetImg();
        gameObject.SetActive(true);
        myClass = _class;
        iconObject.sprite = classImg[myClass];
        nickNameText.text = _nickname;
        if (_owner == _nickname) nickNameText.color = new Color32(0, 203, 38, 255);
        else nickNameText.color = new Color32(0, 0, 0, 255);
        if (_ready >= 0) borderFrame.color = new Color32(255, 79, 0, 255);
        else borderFrame.color = new Color32(255, 255, 255, 255);



    }




}
