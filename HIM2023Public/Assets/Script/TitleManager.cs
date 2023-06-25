using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviourPunCallbacks
{
    private UserData userdata;

    private string networkState;

    public void GoToLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = userdata.Nickname;
        Debug.Log("go to lobby플레이어 인원 수 : " + PhotonNetwork.PlayerList.Length);
        foreach (Player player in PhotonNetwork.PlayerList)

        {

            Debug.Log("User List : " + player.NickName);

        }


        Debug.Log("go to game");
        PhotonNetwork.JoinOrCreateRoom("LobbyRoom", new RoomOptions { MaxPlayers = 20 }, null);

    }

    void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Join to Lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.LoadLevel("GameLobby");
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
