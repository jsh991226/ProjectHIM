using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateButtonManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject createPanel;

    [Header("InputField")]
    [SerializeField]
    private InputField roomNameText;
    [SerializeField]
    private InputField roomInfoText;
    [SerializeField]
    private InputField numberText;
    [SerializeField]
    private GameObject networkManager;
    [SerializeField]
    private DBManager dbm;

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    [SerializeField]
    private GameObject roomListPanel;
    [SerializeField]
    private Transform roomListContainer;


    private string roomName;
    private string roomInfo;
    private int maxPlayerNum;
    private NetworkManager netMgr;



    private void Awake()
    {
        netMgr = networkManager.GetComponent<NetworkManager>();
    }

    [System.Obsolete]
    public void OnRoomCreateButton()
    {
        if (createPanel.active == true)
        {
            createPanel.SetActive(false);
        } else
        {
            createPanel.SetActive(true);
        }


    }

    public void OnCreateButton()
    {
        netMgr.CreateRoom(roomNameText.text, roomInfoText.text, int.Parse(numberText.text));
    }


    
    public void GetRoomList()
    {

        string sql = "select * from roomdata where vaild = 1";
        List<string[]> roomList = dbm.GetData(sql);
        RemoveAllChild(roomListContainer);
        for ( int i = 0; i < roomList.Count; i++)
        {
            RoomTable _tempRoom = new RoomTable(roomList[i]);
            GameObject _room = Instantiate(roomListPanel, roomListContainer);
            _room.GetComponent<RoomList>().SetData(_tempRoom, netMgr);
        }
        Debug.Log("방 개수 확인 : " + roomList.Count + "개");

    }

    private void RemoveAllChild( Transform parent)
    {
        Transform[] childList = parent.GetComponentsInChildren<Transform>();
        for (int i = 1; i < childList.Length; i++)
        {
            if (childList[i] != parent) Destroy(childList[i].gameObject);
        }

    }



}
