using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnvilImgCtrl : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> imgs;
    [SerializeField]
    private float frame;

    private Image _image;
    private int imgPnt = 0;
    private int frameCnt = 0;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(++frameCnt > frame)
        {
            frameCnt = 0;
            imgPnt++;
            if (imgPnt >= imgs.Count) imgPnt = 0;
            _image.sprite = imgs[imgPnt];
        }


    }
}
