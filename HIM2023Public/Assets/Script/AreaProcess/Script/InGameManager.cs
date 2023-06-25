using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using FarrokhGames.Inventory.Examples;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class InGameManager : MonoBehaviourPunCallbacks
{
    [Header("ObjectField")]
    public DBManager dbm;
    public GameObject playerObj;
    public PlayerManager playerMgr;
    public UserData userdata;
    public PlayerInventory playerInv;
    public GameObject TimeObj;
    public Text timeText;
    public GameObject PortalObj;
    public Text portalText;

    public float bossHP;
    public Text portalTimeText;
    public GameObject bossObj;


    

    private bool isPortalTimerSet;



    [Header("SpawnPoints")]
    public List<GameObject> points;
    [Header("PlayTime Minute")]
    public int minutes;
    private bool startTimer;
    private bool isFirst;
    public bool isBossDead;
    private bool noNotify;

    private bool isChangeCheck;
    private double seconds;
    private bool isPortalCheck;
    private bool isRemovePortal;

    [SerializeField]
    private NotifyCtrl notify;
    [SerializeField]
    private ChatCtrl chatCtrl;

    private DateTime forEndTime;
    private DateTime portalTime;

    private PhotonView PV;
    public GameObject miniMapCamera;


    private string nickName;
    public Text userTmp;
    public EventController evc;
    public PanelManager loadingPnl;
    private int nowOpenedPortal;
    public QuestManager questMgr;

    [SerializeField]
    private Text playerNickText;
    [SerializeField]
    private Image playerIconImage;
    [SerializeField]
    private Image hpProg;
    [SerializeField]
    private List<Sprite> playerIcon;

    [SerializeField]
    private ReEquip leftEquip;
    [SerializeField]
    private ReEquip rightEquip;
    [SerializeField]
    private QuestManager qm;
    [SerializeField]
    private CinemaCtrl cinemaCtrl;




    // Start is called before the first frame update
    void Awake()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
    }

    private void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();
        SpawnPlayer();
        seconds = minutes * 60;
    }

    private void WaitForPlayer()
    {
        StartCoroutine(WaitForUpdate());

    }

    IEnumerator WaitForUpdate()
    {
        while (true)
        {
            Debug.LogError(" 들어와야 할 유저 : " + userdata.NowPlayerCnt + " 들어온 할 유저 : " + PhotonNetwork.PlayerList.Length);
            if (userdata.NowPlayerCnt <= PhotonNetwork.PlayerList.Length)
            {
                Debug.LogError("다 들어옴");
                int cnt = 0;
                forEndTime = DateTime.Now;
                forEndTime = forEndTime.AddSeconds(seconds);
                foreach (var playersName in PhotonNetwork.PlayerList)
                {
                    string[] _nick = (playersName+ "").Split("'");
                    object[] data = { _nick[1], cnt++};
                    evc.SendRaiseEvent(EventController.EVENTCODE.RECIVESPAWNPOINT, data, EventController.SEND_OPTION.ALL);
                }
                break;

            }
            yield return new WaitForSeconds(0.01f);
        }
    }


    public void SyncBossDead(int[] removePotal)
    {
        if (isPortalCheck) return;
        isPortalCheck = true;
        noNotify = true;
        isBossDead = true;
        notify.CastNotify("보스가 세상을 떠났습니다. 지금부터 탈출구가 개방됩니다!");
        if (bossObj.GetComponent<BossMoveCtrl>() != null)
        {
            bossObj.GetComponent<BossMoveCtrl>().HP = 0;
        }
        else if(bossObj.GetComponent<ArenaBossCtrl>() != null)
        {
            bossObj.GetComponent<ArenaBossCtrl>().HP = 0;
        }

        TimeObj.SetActive(false);
        PortalObj.SetActive(true);
        if (isRemovePortal != true )
        {
            isRemovePortal = true;
            if (points.Count > removePotal.Length)
            {
                for (int i = 0; i < removePotal.Length; i++) points.RemoveAt(removePotal[i]);
            }
        }

        portalText.text = points.Count.ToString() + "개";
        nowOpenedPortal = 0;
        ResetIndex();
        points[nowOpenedPortal].SetActive(true);
        portalTime = DateTime.Now.AddSeconds(60);
        isPortalTimerSet = true;




    }

    public void ResetIndex()
    {
        int i = 0;
        foreach (GameObject _point in points) _point.GetComponent<GetSpawnPos>().index = i++;
    }

    public void UsePortal(int _index)
    {
        int _rdm = Random.Range(0, points.Count-1);
        object[] data = { PhotonNetwork.NickName , _index, _rdm };
        evc.SendRaiseEvent(EventController.EVENTCODE.USEPORTAL, data, EventController.SEND_OPTION.OTHER);
    }

    public void SyncUsePortal(string _nick, int _index, int _rdm)
    {
        points[_index].SetActive(false);
        points.RemoveAt(_index);
        ResetIndex();
        points[_rdm].SetActive(true);
        notify.CastNotify(_nick + "이(가) 탈출에 성공 했습니다, 새로운 탈출구가 생성됩니다");
        nowOpenedPortal = _rdm;
        portalTime = DateTime.Now.AddSeconds(60);
        portalText.text = points.Count.ToString() + "개";

    }

    public void ChangePortal()
    {
        if (points.Count <= 1)
        {
            object[] noneData = {""};
            evc.SendRaiseEvent(EventController.EVENTCODE.ALLPORTALCLOSE, noneData, EventController.SEND_OPTION.ALL);
            return;
        }
        int rdm = Random.Range(0, points.Count-2);
        object[] data = { rdm };
        evc.SendRaiseEvent(EventController.EVENTCODE.CHANGEPORTAL, data, EventController.SEND_OPTION.ALL);

    }

    public void SyncAllPortalClose()
    {
        notify.CastNotify("모든 탈출구가 사라져 사망하였습니다, 로비로 돌아갑니다");
        PortalObj.SetActive(false);
        playerMgr.animator.SetTrigger("Die");
        playerMgr.isDead = true;
        playerMgr.cameraCtrl.StartCoroutine(playerMgr.cameraCtrl.CameraViewToggle());
        StartCoroutine(GoLobbyWait(2f));

    }

    public void SyncChangePortal(int rdm)
    {
        notify.CastNotify("시간초과로 기존 탈출구가 소멸되어 새로운 장소에 개방됩니다");
        points[nowOpenedPortal].SetActive(false);
        points.RemoveAt(nowOpenedPortal);
        ResetIndex();
        nowOpenedPortal = rdm;
        points[nowOpenedPortal].SetActive(true);
        portalTime = DateTime.Now.AddSeconds(60);
        portalText.text = points.Count.ToString() + "개";
        isChangeCheck = false;
    }


    private void Update()
    {
        if (playerObj != null)
        {
            if (playerObj.GetComponent<PlayerManager>().PV.IsMine)
                miniMapCamera.transform.position = new Vector3(playerObj.transform.position.x, 100, playerObj.transform.position.z);
        }

        if (isBossDead && !noNotify)
        {
            noNotify = true;
            int playerCount = 0;
            foreach (var playersName in PhotonNetwork.PlayerList)
            {
                playerCount++;
            }
            int _potalSub = points.Count - playerCount;
            int[] removePotal = new int[_potalSub];
            if (_potalSub > 0)
            {

                isRemovePortal = true;
                for (int i = 0; i < _potalSub; i++)
                {
                    int _rdmNum = Random.Range(0, points.Count);
                    Debug.LogError("지운 포탈 : " + points[_rdmNum].transform.position);
                    removePotal[i] = _rdmNum;
                    points.RemoveAt(_rdmNum);
                }
                
            }


            object[] data = { removePotal };
            evc.SendRaiseEvent(EventController.EVENTCODE.BOSSDEAD, data, EventController.SEND_OPTION.ALL);
        }



        if (startTimer ==  true && !isBossDead)
        {

            //seconds -= Time.fixedDeltaTime;
            TimeSpan _timeCal =  forEndTime - DateTime.Now;
            timeText.text = _timeCal.Minutes + "분 " + _timeCal.Seconds + "초";
            if (_timeCal.Minutes <= 0 && _timeCal.Seconds < 1 && !isFirst)
            {
                startTimer = false;
                isFirst = true;
                GameEnd();


            }
        }
        if (portalTime != null && isBossDead && isPortalTimerSet)
        {
            Debug.Log("portalTime : " + portalTime);
            TimeSpan _timeCal = portalTime - DateTime.Now;
            portalTimeText.text = Math.Truncate(_timeCal.TotalSeconds) + "초";
            if (_timeCal.Minutes <= 0 && _timeCal.Seconds < 1 && !isChangeCheck)
            {
                if (PhotonNetwork.IsMasterClient == true && PV.IsMine)
                {
                    isChangeCheck = true;
                    Debug.LogError("당신은 마스터 입니다 유저수 : " + PhotonNetwork.PlayerList.Length);
                    
                    ChangePortal();
                }

            }


        }


    }
    public void GameEnd()
    {
        playerMgr.animator.SetTrigger("Die");
        playerMgr.isDead = true;
        notify.CastNotify("제한시간 안에 탈출하지 못했습니다, 사망하여 로비로 돌아갑니다");
        playerMgr.cameraCtrl.StartCoroutine(playerMgr.cameraCtrl.CameraViewToggle());
        StartCoroutine(GoLobbyWait(2f));
    }

    IEnumerator GoLobbyWait(float _time)
    {
        yield return new WaitForSeconds(_time);
        loadingPnl.GUIToggle(true);
        PhotonNetwork.LeaveRoom();
    }


    public void SyncRecivedSpawnPoint(string _nick, int _idx )
    {
        if (userdata.Nickname != _nick) return;
        forEndTime = DateTime.Now;
        forEndTime = forEndTime.AddSeconds(seconds);
        playerObj.transform.position = points[_idx].GetComponent<GetSpawnPos>().GetPoint();
        loadingPnl.GUIToggle(false);
        notify.CastNotify("습격 장소에 도착 하였습니다, 무사히 탈출하세요!");
        playerMgr.cameraCtrl.StartCoroutine(playerMgr.cameraCtrl.CameraViewToggle());
        startTimer = true;
        questMgr.StartQuest();
    }



    public override void OnLeftRoom()
    {
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("LobbyRoom", new RoomOptions { MaxPlayers = 20 }, null);

    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameLobby");
    }



    public void SpawnPlayer()
    {
        Debug.Log("InGame 플레이어 인원 수 : " + PhotonNetwork.PlayerList.Length);
        Vector3 spawnPos = points[0].transform.position; //게임 스폰 좌표
        playerObj = PhotonNetwork.Instantiate(userdata.UserClass, spawnPos, Quaternion.identity, 0);
        playerObj.name = "PlayerIsMine";
        nickName = PhotonNetwork.NickName;
        playerObj.GetComponent<PlayerManager>().FPCamera.GetComponent<ActionController>().actionText = userTmp;
        playerMgr = playerObj.GetComponent<PlayerManager>();
        playerObj.GetComponent<PlayerManager>().actionText = userTmp;
        playerInv.dropZone = playerObj;
        playerInv.playerObj = playerObj;
        playerMgr.isGameIn = true;
        playerMgr.playerInv = playerInv;
        qm._pm = playerMgr;
        playerMgr.qm = qm;
        playerMgr.InArea = true;
        playerMgr.loadingPnl = loadingPnl;
        PV = playerObj.GetComponent<PhotonView>();
        evc.pm = playerMgr;
        playerMgr.evc = evc;
        playerMgr.chatCtrl = chatCtrl;
        playerMgr.notify = notify;
        playerNickText.text = nickName;
        playerMgr.hpProg = hpProg;
        playerMgr.leftEquip = leftEquip;
        if (cinemaCtrl != null)cinemaCtrl.playerFPC = playerMgr.FPCamera;
        playerMgr.dbm = dbm;
        if (cinemaCtrl != null) cinemaCtrl.localPlayer = playerObj.GetComponent<LivingEntity>();
        chatCtrl.localPlayer = playerObj.GetComponent<LivingEntity>();
        playerMgr.rightEquip = rightEquip;

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

        if (PhotonNetwork.IsMasterClient == true && PV.IsMine)
        {
            Debug.LogError("당신은 마스터 입니다 유저수 : " + PhotonNetwork.PlayerList.Length);
            WaitForPlayer();
        }


        nickName = PhotonNetwork.LocalPlayer.NickName;

        if (!userdata.IsGaming)
        {
            Debug.Log("로비에 입장 한 유저 : " + nickName);
        }
        else
        {
            Debug.Log("게임에 입장 한 유저 : " + nickName);
        }



    }
}
