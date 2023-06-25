using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private string networkState;
	private bool isCreate;

	private string _roomName;
	private string _roomInfo;
	private int _maxPlayerNum;

	[SerializeField]
	private DBManager dbm;

	public DBManager Dbm { get => dbm; set => dbm = value; }

	private UserData userdata;

    private void Awake()
    {
		userdata = GameObject.Find("UserData").GetComponent<UserData>();

	}

	public void ChangeRoom(string _roomName)
    {
		isCreate = false;
		this._roomName = _roomName;
		PhotonNetwork.LeaveRoom();
	}

	public void CreateRoom(string _roomName, string _roomInfo, int _maxPlayerNum)
    {
		this._roomName = _roomName;
		this._roomInfo = _roomInfo;
		this._maxPlayerNum = _maxPlayerNum;
		isCreate = true;
		PhotonNetwork.LeaveRoom();
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}
	public override void OnJoinedLobby()
	{
		if (isCreate) {
			isCreate = false;
			Debug.Log("is create");
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.IsOpen = true;
			roomOptions.IsVisible = true;
			roomOptions.MaxPlayers = (byte)_maxPlayerNum;
			string sql = "INSERT INTO roomdata(roomtitle, roomdesc, people, owner) VALUES('" + _roomName + "', '" + _roomInfo + "', '" + PhotonNetwork.LocalPlayer.NickName + "', '" + PhotonNetwork.LocalPlayer.NickName + "')";
			dbm.QueryData(sql); //RoomTable(int roomNum, string roomTitle, string roomDesc, string people, string owner, int vaild)
			List<string[]> _lastRow = dbm.GetData("SELECT * FROM roomdata ORDER BY roomnum DESC LIMIT 1");
			RoomTable _table = new RoomTable(_lastRow[0]);
			Debug.Log("Table info : " + _table.RoomNum + _table.RoomTitle + _table.RoomDesc);
			PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, null);
		} else
        {
			Debug.Log("not create ");
			PhotonNetwork.JoinRoom(_roomName);
		}
	}
	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("TestRoom");
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
}