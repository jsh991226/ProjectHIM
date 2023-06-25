
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class AreaCtrl : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [Header("Area List")]
    public List<AreaEntity> Areas;


    [Header("Area Object")]
    [SerializeField]
    private GameObject AreaView;
    [SerializeField]
    private GameObject panelObject;
    [SerializeField]
    private GameObject gridPanel;
    [SerializeField]
    private AreaStatusCtrl areaStatus;
    [SerializeField]
    private PanelManager panelStatus;
    [SerializeField]
    private DBManager dbm;
    [SerializeField]
    private NotifyCtrl notify;
    [SerializeField]
    private AreaInfoCtrl infoCtrl;
    [SerializeField]
    private Text readyBtnText;
    [SerializeField]
    private Text errorMsgText;


    public PhotonView PV;

    public GameManager gameManager;

    public List<GameObject> panels;

    public int myJoinArea;

    private string userNick;

    public PlayerPhotonCtrl PPC;

    private bool AreaReady = false;

    private bool AreaListOpen = false;

    private string networkState;

    private bool isCreate;

    public string _roomName;

    private UserData userdata;

    public EventController evc;

    public PanelManager loadingPnl;







    private void Awake()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

    }


    void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();
        foreach (AreaEntity _entity in Areas)
        {
            GameObject _panel = GameObject.Instantiate(panelObject);
            _panel.transform.SetParent(gridPanel.transform, false);
            _panel.GetComponent<AreaPanel>().SetContent(_entity, gameObject, AreaView, areaStatus);
            panels.Add(_panel);
        }
    }

    public void OnLoad()
    {
        AreaListOpen = true;
        List<string[]> result = dbm.GetData("select * from area where vaild=1");
        dbm.ListToDebug(result);
        int cnt = 0;
        foreach (AreaEntity _entity in Areas)
        {
            _entity.PeopleNow = 0;
            _entity.PlayerList.Clear();
            _entity.readyPlayer.Clear();
            panels[cnt].GetComponent<AreaPanel>().SetPeople(0, -1);
            cnt++;
        }
        cnt = 0;
        foreach (string[] rows in result)
        {
            List<string> _playerList = new List<string>();
            string[] playerList = GetPlayerList(rows[2]);
            for (int i = 0; i < playerList.Length; i++)
            {
                Debug.Log(i + " : " + playerList[i]);
                _playerList.Add(playerList[i]);
            }
            Areas[cnt].Owner = rows[3];
            Areas[cnt].PeopleNow = _playerList.Count;
            Areas[cnt].PlayerList = _playerList;
            panels[Int32.Parse(rows[1])-1].GetComponent<AreaPanel>().SetPeople(playerList.Length, myJoinArea);
            cnt++;
        }

    }

    public void OnClose()
    {
        AreaListOpen = false;
    }





    public void JoinArea(AreaEntity _entity, string _nick)
    {
        List<string[]> result = dbm.GetData("select count(*) from area where vaild=1 and areaId = '"+ _entity.Id + "'");
        if (Int32.Parse(result[0][0]) > 1)
        {
            Debug.LogError("area vaild column > 1 / area id : " + _entity.Id);
            return;
        }
        if (Int32.Parse(result[0][0]) == 0) //create Area
        {
            string _sql = "INSERT INTO area (areaid, waitPlayer, owner) VALUES ('" + _entity.Id + "', '" + _nick + "/', '" + _nick + "')";
            dbm.QueryData(_sql);
            _entity.AddPlayer(_nick);
            _entity.Owner = _nick;
        }
        else //join Area 
        {
            string _sql = "select waitPlayer, readyPlayer, owner from area where vaild=1 and areaId = '" + _entity.Id + "'";
            result = dbm.GetData(_sql);
            _sql = "update area set waitPlayer = '" + result[0][0]+_nick+ "/' where vaild=1 and areaId = '" + _entity.Id +"'";
            dbm.QueryData(_sql);
            foreach (string name in GetPlayerList(result[0][1])) {
                _entity.readyPlayer.Add(name);
            }
            _entity.Owner = result[0][2];
            _entity.AddPlayer(_nick);
        }
        myJoinArea = _entity.Id;
        userNick = _nick;
        AreaReady = false; //레디 초기화
        readyBtnText.text = "준비 완료";
        notify.CastNotify(_entity.Title + " 작업에 참여 하였습니다");
        object[] data = { myJoinArea, userNick };
        evc.SendRaiseEvent(EventController.EVENTCODE.JOINAREA, data, EventController.SEND_OPTION.OTHER);


    }



    public void SyncJoinArea(int _id, string _nick)
    {
        if (AreaListOpen) OnLoad();

        if (myJoinArea != _id) return;
        AreaEntity _entity = Areas[myJoinArea - 1];
        _entity.AddPlayer(_nick);
        notify.CastNotify(_nick + "님께서" + _entity.Title + " 작업에 참여 하였습니다");
        areaStatus.SyncGUISetting();


    }


    public void SyncExitArea(int _id, string _nick, string _owner)
    {
        if (AreaListOpen) OnLoad();
        if (myJoinArea != _id) return;
        AreaEntity _entity = Areas[myJoinArea - 1];
        _entity.RemovePlayer(_nick);
        _entity.readyPlayer.Remove(_nick);
        if (_entity.Owner != _owner && _owner == userNick && AreaReady)
        {
            string _sql = "select readyPlayer from area where vaild=1 and areaId = '" + _entity.Id + "'";
            List<string[]> result = dbm.GetData(_sql);
            string _templist = result[0][0];
            string readyPlayerStr;
            readyPlayerStr = _templist.Replace(userNick + "/", "");
            _sql = "update area set readyPlayer = '" + readyPlayerStr + "' where vaild=1 and areaId = '" + _entity.Id + "'";
            dbm.QueryData(_sql);
        }
        _entity.readyPlayer.Remove(_owner);
        _entity.Owner = _owner; //본인이 오너이면 오너 처리 해줘야함

        notify.CastNotify(_nick + "님께서" + _entity.Title + " 작업에서 퇴장 하였습니다");
        areaStatus.SyncGUISetting();


    }

    public void SyncReadyPlayer(int _id, string _nick, bool _type)
    {
        if (myJoinArea != _id) return;
        AreaEntity _entity = Areas[myJoinArea - 1];
        _entity.readyPlayer.Clear();
        string _sql = "select readyPlayer from area where vaild=1 and areaId = '" + _entity.Id + "'";
        List<string[]> result = dbm.GetData(_sql);
        foreach (string name in GetPlayerList(result[0][0])) {
            _entity.readyPlayer.Add(name);
        }
        areaStatus.SyncGUISetting();
    }


    public void ExitArea()
    {
        AreaEntity _entity = Areas[myJoinArea - 1];
        notify.CastNotify(_entity.Title + " 작업에서 퇴장 하였습니다");
        infoCtrl.SetDownSize();
        ActiveExitArea();

    }

    private void OnApplicationQuit() //강제종료 이벤트
    {
        if (myJoinArea > 0) ActiveExitArea();


    }




    public void ActiveReadyToggle()
    {
        AreaEntity _entity = Areas[myJoinArea - 1];
        string _sql = "select readyPlayer from area where vaild=1 and areaId = '" + _entity.Id + "'";
        List<string[]> result = dbm.GetData(_sql);
        string _templist = result[0][0];
        string readyPlayerStr;
        if (!AreaReady)
        {
            AreaReady = true;
            readyBtnText.text = "준비 취소";
            areaStatus.playerCards[Areas[myJoinArea - 1].PlayerList.IndexOf(userNick)].GetComponent<PlayerCardEntity>().ReadyBorder(true);
            readyPlayerStr = _templist + userNick + "/";
            _entity.readyPlayer.Add(userNick);

        }
        else
        {
            AreaReady = false;
            readyBtnText.text = "준비 완료";
            areaStatus.playerCards[Areas[myJoinArea - 1].PlayerList.IndexOf(userNick)].GetComponent<PlayerCardEntity>().ReadyBorder(false);
            readyPlayerStr = _templist.Replace(userNick + "/", "");
            _entity.readyPlayer.Remove(userNick);

        }


        _sql = "update area set readyPlayer = '" + readyPlayerStr + "' where vaild=1 and areaId = '" + _entity.Id + "'";
        dbm.QueryData(_sql);
        object[] data = { myJoinArea, userNick, AreaReady };
        evc.SendRaiseEvent(EventController.EVENTCODE.READYPLAYER, data, EventController.SEND_OPTION.OTHER);

    }






    private void ActiveExitArea()
    {
        AreaEntity _entity = Areas[myJoinArea - 1];
        myJoinArea = -1;
        try
        {
            string _owner = _entity.Owner;
            if (userNick == _entity.Owner)
            {
                foreach (string _fornick in _entity.PlayerList)
                {
                    if (_fornick != userNick)
                    {
                        _owner = _fornick;
                        break;
                    }
                }
            }
            string _sql = "select waitPlayer, readyPlayer from area where vaild=1 and areaId = '" + _entity.Id + "'";
            List<string[]> result = dbm.GetData(_sql);
            string _playerString = result[0][0];
            string playerString = _playerString.Replace(userNick + "/", "");
            string _readyPlayerString = result[0][1];
            string readyPlayer = _readyPlayerString.Replace(userNick + "/", "");
            panelStatus.GUIToggle(false);
            if (GetPlayerList(playerString).Length <= 0)
            {
                _sql = "update area set vaild = 0 where vaild=1 and areaId = '" + _entity.Id + "'";
                dbm.QueryData(_sql);
                Debug.Log("방 삭제");
                return;
            }else
            {
                object[] data = { _entity.Id, userNick, _owner };
                _sql = "update area set waitPlayer = '" + playerString + "', readyPlayer = '" + readyPlayer + "', owner = '" + _owner + "' where vaild=1 and areaId = '" + _entity.Id + "'";
                dbm.QueryData(_sql);
                evc.SendRaiseEvent(EventController.EVENTCODE.EXITAREA, data, EventController.SEND_OPTION.OTHER);
            }



        }
        catch (Exception e)
        {
            Debug.Log("[Area Error] : " + e);

        }

    }



    public void StartArea()
    {
        AreaEntity _entity = Areas[myJoinArea - 1];
        if (_entity.Owner == userNick)
        {
            string _sql = "select id, waitPlayer, readyPlayer from area where vaild=1 and areaId = '" + _entity.Id + "'";
            List<string[]> result = dbm.GetData(_sql);
            string[] waitPlayer = GetPlayerList(result[0][1]);
            string[] readyPlayer = GetPlayerList(result[0][2]);
            if (waitPlayer.Length < _entity.PeopleMin)
            {
                errorMsgText.text = "최소 인원이 모이지 않았습니다";
                return;
            }
            if (waitPlayer.Length == readyPlayer.Length+1) // Owner는 대기가 없어 +1 해줘야함
            {
                //start
                Debug.LogError("[Area Start] Move To Area");
                _sql = "update area set vaild = '2' where vaild=1 and areaId = '" + _entity.Id + "'";
                dbm.QueryData(_sql);

                MoveArea(Int32.Parse(result[0][0]));

            } else
            {
                errorMsgText.text = "모든 인원이 준비 되지 않았습니다";
                return;
            }


        }
        else
        {
            Debug.LogError("[Area Error] : player is not Owner");
        }


    }


    private void MoveArea(int _id)
    {
        AreaEntity _entity = Areas[myJoinArea - 1];
        string _roomName = _entity.Title + "_" + _id;
        object[] data = { myJoinArea, _roomName };
        this._roomName = _roomName;
        loadingPnl.GUIToggle(true);
        userdata.NowPlayerCnt = _entity.PeopleNow;

        evc.SendRaiseEvent(EventController.EVENTCODE.MOVEAREA, data, EventController.SEND_OPTION.OTHER);
        StartCoroutine(CorMoveArea());


    }

 

    private IEnumerator CorMoveArea()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("코루틴 끝");
        PhotonNetwork.LeaveRoom(); 

    }


    public void SyncMoveArea(int _id, string _roomName)
    {
        Debug.LogError("text : " + _id + " : " + _roomName);
        if (_id == myJoinArea && AreaReady)
        {
            loadingPnl.GUIToggle(true);
            userdata.NowPlayerCnt = Areas[_id- 1].PeopleNow;
            isCreate = false;
            this._roomName = _roomName;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.ConnectUsingSettings();

        }
        else
        {
            if (AreaListOpen) OnLoad();
        }

    }







    void Update()
    {
        string curNetworkState = PhotonNetwork.NetworkClientState.ToString();
        if (networkState != curNetworkState)
        {
            networkState = curNetworkState;
            print(networkState);
        }
    }


    public string[] GetPlayerList(string playerString)
    {
        string[] _temp = playerString.Split('/');
        string[] result = new string[_temp.Length-1];
        for (int i = 0; i < result.Length; i++) result[i] = _temp[i];
        return result;
    }


}
