using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using FarrokhGames.Inventory.Examples;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{ 
    [Header("ObjectField")]
    public GameObject DBManager;
    public GameObject playerObj;
    public PlayerManager playerMgr;
    public GameObject SelectRollPanel;
    public AreaCtrl areaCtrl;
    public PlayerInventory playerInv;
    public EventController evc;
    public GameObject miniMapCamera;

    private bool ErrorGoToLobby;

    private string nickName;
    private DBManager dbm;
    public UserData userdata;

    public Text userTmp;


    [SerializeField]
    private Text playerNickText;
    [SerializeField]
    private Image playerIconImage;
    [SerializeField]
    private Image hpProg;
    [SerializeField]
    private List<Sprite> playerIcon;




    private Vector3 spawnPos;
    private PhotonView PV;
    [SerializeField]
    private AreaStatusCtrl areaStatus;
    [SerializeField]
    private NotifyCtrl notify;
    [SerializeField]
    private ChatCtrl chatCtrl;
    [SerializeField]
    private ReEquip leftEquip;
    [SerializeField]
    private ReEquip rightEquip;


    public PlayerPhotonCtrl PPC;





    public DBManager Dbm { get => dbm; set => dbm = value; }

    private string ClassStr;

    private void Awake()
    {
        Dbm = DBManager.GetComponent<DBManager>();
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
        

    }

    private void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();

        if (userdata.UserClass == "notset")
        {
            SelectRollPanel.GetComponent<PanelManager>().GUIToggle(true);
        } else
        {
            SpawnPlayer();
        }





    }


    public bool UserCheck(string _nick)
    {
        GameObject[] _playerList = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("실행 : " + _playerList.Length);
        for (int i = 0; i < _playerList.Length; i++)
        {
            string[] _nickList = (_playerList[i].GetComponent<PhotonView>().Owner + "").Split("'");
            Debug.LogError("유저 : " + _nick + " : " + _nickList[1]);
            if (_nick == _nickList[1])
            {
                if (_playerList[i].GetComponent<PhotonView>().IsMine == false) return false;
            }

        }
        return true;
    }



    private void Update()
    {
        if (playerObj != null)
        {
            if (playerObj.GetComponent<PlayerManager>().PV.IsMine)
            miniMapCamera.transform.position = new Vector3(playerObj.transform.position.x, 100, playerObj.transform.position.z);
        }
    }


    public void SpawnPlayer()   
    {
        SelectRollPanel.GetComponent<PanelManager>().GUIToggle(false);
        if (!userdata.IsGaming) spawnPos = new Vector3(68, 23, 48); //로비 스폰 좌표
        else spawnPos = new Vector3(0, 4, 0); //게임 스폰 좌표


        playerObj = PhotonNetwork.Instantiate(userdata.UserClass, spawnPos, Quaternion.identity, 0);
        playerObj.name = "PlayerIsMine";
        playerMgr = playerObj.GetComponent<PlayerManager>();

        PV = playerObj.GetComponent<PhotonView>();

        playerInv.dropZone = playerObj;
        playerInv.playerObj = playerObj;
        playerMgr.InArea = false;
        playerMgr.playerInv = playerInv;
        evc.pm = playerMgr;
        playerMgr.evc = evc;

        nickName = PhotonNetwork.LocalPlayer.NickName;
        Debug.LogError("유저 소환 : " + nickName);
        playerMgr.chatCtrl = chatCtrl;
        playerMgr.notify = notify;

        playerObj.GetComponent<PlayerManager>().nickName = nickName;
        playerObj.GetComponent<PlayerManager>().FPCamera.GetComponent<ActionController>().actionText = userTmp;
        playerObj.GetComponent<PlayerManager>().actionText = userTmp;
        playerMgr.leftEquip = leftEquip;
        playerMgr.rightEquip = rightEquip;
        playerMgr.dbm = dbm;

        areaCtrl.PV = PV;
        areaCtrl.PPC = PPC;
        playerNickText.text = nickName;
        playerMgr.hpProg = hpProg;
        if (userdata.UserClass == "Goblin")
        {
            playerIconImage.sprite = playerIcon[0];
        }
        else if (userdata.UserClass == "Golem")
        {
            playerIconImage.sprite = playerIcon[1];
        }
        else if (userdata.UserClass == "Ogre")
        {
            playerIconImage.sprite = playerIcon[2];
        }
        else if (userdata.UserClass == "Ork")
        {
            playerIconImage.sprite = playerIcon[3];
        }
        else if (userdata.UserClass == "Skeleton")
        {
            playerIconImage.sprite = playerIcon[4];
        }

        if (!userdata.IsGaming)
        {
            Debug.Log("로비에 입장 한 유저 : " + nickName);
        }
        else
        {
            Debug.Log("게임에 입장 한 유저 : " + nickName);
        }



    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!userdata.IsGaming) return;


    }
    public override void OnLeftRoom()
    {
        Debug.Log("게임메니저 나감");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connect to master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        if (ErrorGoToLobby)
        {
            PhotonNetwork.JoinOrCreateRoom("LobbyRoom", new RoomOptions { MaxPlayers = 20 }, null);
        }
        else
        {
            Debug.Log("Joind Lobby");
            AreaEntity _entity = areaCtrl.Areas[areaCtrl.myJoinArea - 1];
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = (byte)_entity.PeopleMax;
            PhotonNetwork.JoinOrCreateRoom(areaCtrl._roomName, roomOptions, null);
        }


    }
    public override void OnJoinedRoom()
    {
        if (ErrorGoToLobby)
        {
            PhotonNetwork.LoadLevel("Title");
        }
        else
        {
            AreaEntity _entity = areaCtrl.Areas[areaCtrl.myJoinArea - 1];
            PhotonNetwork.LoadLevel(_entity.GameScene);
        }

    }

    /*
        private void OnApplicationQuit()
        {
            dbm.QueryData("DELETE FROM onlineplayer WHERE nickname = '"+ nickName + "'");
            if (nowRoom == null) return;
            Debug.Log("삭제 전 : " + nowRoom.ListToString());
            nowRoom.PlayerList.Remove(nickName);
            Debug.Log("삭제 후 : " + nowRoom.ListToString() + nowRoom.PlayerList.Count);
            if (nowRoom.PlayerList.Count <= 0)
            {
                dbm.QueryData("update roomdata set vaild = 0 where roomnum = " + nowRoom.RoomNum);
                PV.RPC("playerQuitRpc", RpcTarget.All);
            }
            *//*        if (nowRoom != null)
                    {
                        int resultPlayerCount = nowRoom.QuitPlayer(nickName);
                        if (resultPlayerCount == 0)
                        {
                            dbm.QueryData("update roomdata set vaild = 0 where roomnum = " + nowRoom.RoomNum);
                        }
                        else
                        {
                            dbm.QueryData("update roomdata set people = '" + nowRoom.ListToString() + "'where roomnum = " + nowRoom.RoomNum);
                        }
                        PV.RPC("playerQuitRpc", RpcTarget.AllBuffered, nickName);


                    }*//*
        }*/


    /*    [PunRPC]
        public void playerQuitRpc(string _nick)
        {
            if (PV.IsMine) return;
            Debug.Log("사람 나감 현재 유저 수 : " + nowRoom.PlayerList.Count);
        }*/


    /*    [PunRPC]
        public void playerQuitRpc(string _nick)
        {
            if (!PV.IsMine)
            {
                playerQuit(_nick);
            }
        }
        public void playerQuit(string _nick)
        {
            Debug.Log("Quit player Nick : " + _nick);
            nowRoom.QuitPlayer(_nick);
        }*/









}
