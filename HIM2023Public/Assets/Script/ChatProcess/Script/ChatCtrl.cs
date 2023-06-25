using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ChatCtrl : MonoBehaviourPunCallbacks
{
    [Header("Chat Objects")]
    [SerializeField]
    private GameObject ChatContents;
    [SerializeField]
    private InputField chatInput;
    [SerializeField]
    private GameObject chatLine;
    [SerializeField]
    private EcoCtrl eco;
    [SerializeField]
    private Scrollbar ScrollbarVertical;
    [SerializeField]
    private BossMoveCtrl bossCtrl;
    [SerializeField]
    private ArenaBossCtrl arenaBossCtrl;
    [SerializeField]
    private QuestManager questMgr;

    public LivingEntity localPlayer;


    private bool isChat = false;
    public bool IsChat { get => isChat; set => isChat = value; }


    public EventController evc;



    private void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();
    }





    private void SendChat()
    {
        string _nick = PhotonNetwork.NickName;
        string _content = chatInput.text;
        chatInput.text = "";
        GameObject _chatsLine = Instantiate(chatLine);
        _chatsLine.transform.SetParent(ChatContents.transform);
        _chatsLine.GetComponent<ChatLine>().SetLine(_nick, _content, 1);
        object[] data = { _nick, _content };
        evc.SendRaiseEvent(EventController.EVENTCODE.RECIVECHAT, data, EventController.SEND_OPTION.OTHER);

    }

    public void SyncSendChat(string _nick, string _content)
    {
        GameObject _chatsLine = Instantiate(chatLine);
        _chatsLine.transform.SetParent(ChatContents.transform);
        _chatsLine.GetComponent<ChatLine>().SetLine(_nick, _content, 0);

    }

    public void ChatNotify(string _content)
    {
        GameObject _chatsLine = Instantiate(chatLine);
        _chatsLine.transform.SetParent(ChatContents.transform);
        _chatsLine.GetComponent<ChatLine>().SetLine("[알림] ", _content, 2);

    }



    private bool CommandCheck(string _command)
    {
        if (!chatInput.text.Contains(_command)) return false;
        if (chatInput.text.Substring(1, _command.Length).ToUpper() != _command.ToUpper()) return false;
        return true;
    }
    private void CommandMoney(string _cmdLine)
    {
        string[] _args = _cmdLine.Split(" ");
        eco.Money(_args);

    }


    void Update()
    {
        ScrollbarVertical.value = 0;

        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (isChat)
            {
                if (chatInput.text.Length > 0)
                {
                    isChat = false;
                    chatInput.DeactivateInputField();
                    if (chatInput.text.Substring(0, 1) == "/") //CommandLine
                    {
                        if (CommandCheck("money"))
                        {
                            string _args = "";
                            try
                            {
                                _args += chatInput.text.Substring(7);
                            }
                            catch
                            {
                                _args += "";
                            }
                            CommandMoney(_args);
                            chatInput.text = "";
                            return;
                        }
                        if (CommandCheck("killboss"))
                        {
                            if (bossCtrl != null) bossCtrl.HP = 0;
                            else if (arenaBossCtrl != null) arenaBossCtrl.HP = 0;
                            chatInput.text = "";
                            return;
                        }
                        if (CommandCheck("clearquest"))
                        {
                            questMgr.ClearQuest();
                            chatInput.text = "";
                            return;
                        }
                        if (CommandCheck("god"))
                        {
                            if (localPlayer.isGod == true)
                            {
                                localPlayer.isGod = false;
                                ChatNotify("무적 상태가 비활성화 되었습니다.");
                            }else
                            {
                                localPlayer.isGod = true;
                                ChatNotify("무적 상태가 활성화 되었습니다.");
                            }

                            chatInput.text = "";
                            return;
                        }



                        ChatNotify("존재 하지 않는 명령어 입니다.");
                        chatInput.text = "";
                        return;
                    }



                    SendChat();

                }
                else
                {
                    isChat = false;
                    chatInput.DeactivateInputField();
                    return;
                }


            }
            else
            {
                isChat = true;
                chatInput.ActivateInputField();
                return;


            }
        }
    }
}
