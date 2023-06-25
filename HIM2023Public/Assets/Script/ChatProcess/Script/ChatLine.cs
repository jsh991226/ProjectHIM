using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatLine : MonoBehaviour
{
    [Header("Line Object")]
    [SerializeField]
    private Text nickName;
    [SerializeField]
    private Text content;




    public void SetLine(string _nick, string _content, int _isMine)
    {
        nickName.text = _nick;
        content.text = _content;
        if (_isMine == 1) nickName.color = new Color32(168, 238, 255, 255);
        if (_isMine == 2) nickName.color = new Color32(255, 103, 103, 255);


    }

}
