using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    private NPCEntity _nowEntity;
    [SerializeField]
    private GameObject shopOpenText;
    [SerializeField]
    private Text shopOwnerName;
    [SerializeField]
    private Text shopCommentText;
    [SerializeField]
    private PanelManager NPCTalkUI;
    [SerializeField]
    private Text OpenBtnText;
    [SerializeField]
    private GameObject OpenBtn;


    public void NPCEvent(NPCEntity _entity)
    {
        _nowEntity = _entity;
    }
    public void SetText()
    {
        shopOwnerName.text = _nowEntity.npcName;
        OpenBtnText.text = _nowEntity.openBtnText;
        if (OpenBtnText.text == "") OpenBtn.SetActive(false);
        else OpenBtn.SetActive(true);

        StartCoroutine(Typing(shopCommentText, _nowEntity.NPCTalk(), 0.01f));
    }

    IEnumerator Typing(Text typingText, string message, float speed)
    {
        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
    }


    public void OpenShop()
    {
        NPCTalkUI.GUIToggle(false);
        _nowEntity.OpenTrigger.Invoke();
    }

    public void CloseShop()
    {
        //NPCTalkUI.GUIToggle(false);
        //_nowEntity.CloseTrigger.Invoke();
    }

    void Update()
    {
        if (NPCTalkUI.GUIStatus == false)
        {
            if (_nowEntity != null) shopOpenText.SetActive(true);
            else shopOpenText.SetActive(false);
            if (Input.GetKeyDown(KeyCode.T) && _nowEntity != null)
            {
                SetText();
                NPCTalkUI.GUIToggle(true);
            }
        }else
        {
            if (_nowEntity == null)
            {
                NPCTalkUI.GUIToggle(false);
            }
            else
            {
                shopOpenText.SetActive(false);
            }

        }

    }

    


}
