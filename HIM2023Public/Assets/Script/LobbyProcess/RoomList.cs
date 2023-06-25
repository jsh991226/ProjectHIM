using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Text roomMasterText;
    [SerializeField]
    private Text roomInfoText;
    [SerializeField]
    private Text peopleNumText;
    [SerializeField]
    private Button joinBtn;

    private RoomTable _roomTable;

    private NetworkManager netMgr;


    public void SetData(RoomTable _roomTable, NetworkManager _netMgr)
    {

        this._roomTable = _roomTable;
        this.netMgr = _netMgr;
        this._roomTable.Dbm = netMgr.Dbm;
        roomNameText.text = "���̸�: " + _roomTable.RoomTitle;
        roomMasterText.text = "����: " + _roomTable.Owner;
        roomInfoText.text = "������: " + _roomTable.RoomDesc;
        peopleNumText.text = "�ο���: " + _roomTable.NowPlayer + " / "; //_roomTable.MaxPlayer ���� ����
        joinBtn.onClick.AddListener(() => OnEnterRoom());
    }

    public void OnEnterRoom()
    {
        _roomTable.JoinPlayer(PhotonNetwork.LocalPlayer.NickName);
        netMgr.ChangeRoom(_roomTable.RoomTitle);
    }
 
}