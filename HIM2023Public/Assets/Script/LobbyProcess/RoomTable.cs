using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTable
{
    private int roomNum;
    private string roomTitle;
    private string roomDesc;
    private string people;
    private string owner;
    private int vaild;
    private List<string> playerList = new List<string>();
    private int maxPlayer; //현재 설정 안함
    private int nowPlayer;
    private DBManager dbm;

    public RoomTable(int roomNum, string roomTitle, string roomDesc, string people, string owner, int vaild)
    {
        this.RoomNum = roomNum;
        this.RoomTitle = roomTitle;
        this.RoomDesc = roomDesc;
        this.People = people;
        string[] _temp = people.Split("/".ToCharArray());
        for (int i = 0; i < _temp.Length; i++) this.PlayerList.Add(_temp[i]);
        this.NowPlayer = PlayerList.Count;
        this.Owner = owner;
        this.Vaild = vaild;
    }
    public RoomTable(string[] sqlResult)
    {
        this.RoomNum = int.Parse(sqlResult[0]);
        this.RoomTitle = sqlResult[1];
        this.RoomDesc = sqlResult[2];
        this.People = sqlResult[3];
        string[] _temp = People.Split("/".ToCharArray());
        for (int i = 0; i < _temp.Length; i++) PlayerList.Add(_temp[i]);
        this.NowPlayer = PlayerList.Count;
        this.Owner = sqlResult[4];
        this.Vaild = int.Parse(sqlResult[5]);
    }
    public void JoinPlayer(string _nick)
    {
        this.PlayerList.Add(_nick);
        this.NowPlayer++;
        dbm.QueryData("update roomdata set people = '"+ ListToString()+ "'where num = " + roomNum);
    }
    public int QuitPlayer(string _nick)
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerList[i].Equals(_nick))
            {
                PlayerList.RemoveAt(i);
                this.NowPlayer--;
                break;
            }
        }
        Debug.Log("Quit Room Num : " + RoomNum);

        return NowPlayer;
    }


    public string ListToString() {
        string _result = null;
        for (int i = 0; i < PlayerList.Count; i++)
        {
            _result = _result + PlayerList[i];
            if (i != PlayerList.Count - 1) _result = _result + "/";
        }
        return _result;

    }



    public int RoomNum { get => roomNum; set => roomNum = value; }
    public string RoomTitle { get => roomTitle; set => roomTitle = value; }
    public string RoomDesc { get => roomDesc; set => roomDesc = value; }
    public string People { get => people; set => people = value; }
    public string Owner { get => owner; set => owner = value; }
    public int Vaild { get => vaild; set => vaild = value; }
    public int MaxPlayer { get => maxPlayer; set => maxPlayer = value; }
    public int NowPlayer { get => nowPlayer; set => nowPlayer = value; }
    public List<string> PlayerList { get => playerList; set => playerList = value; }
    public DBManager Dbm { get => dbm; set => dbm = value; }
}
