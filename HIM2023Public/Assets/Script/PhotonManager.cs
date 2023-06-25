using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameversion = "v1.0";
    private string userId = "goonbam";
    public GameObject userPrefab;

    private void Awake()
    {
        PhotonNetwork.GameVersion = gameversion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("0. 포톤 시작");

        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("01. 포톤 서버에 연결");

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("02. 랜덤 룸 접속 실패");

        RoomOptions room = new RoomOptions();
        room.IsOpen = true;
        room.IsVisible = true;
        room.MaxPlayers = 30;
        PhotonNetwork.JoinOrCreateRoom("testroom", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");


    }



    public override void OnJoinedRoom()
    {
        Debug.Log("04. 방 참가 성공");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity, 0);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
