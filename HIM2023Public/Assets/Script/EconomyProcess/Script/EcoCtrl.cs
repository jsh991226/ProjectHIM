using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcoCtrl : MonoBehaviour
{
    [Header("Eco Objects")]
    [SerializeField]
    private DBManager dbm;
    [SerializeField]
    private ChatCtrl chatCtrl;
    [SerializeField]
    private EventController evc;


    private UserData userdata;
    

    private int money = 0;
    //public int Money { get => money; set => money = value; }




    private void Awake()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
        RefreshMoney();
    }
    private void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();
    }

    private void RefreshMoney()
    {
        string sql = "select money from user where userid='" + userdata.Userid + "'";
        List<string[]> result = dbm.GetData(sql);
        money = int.Parse(result[0][0]);
    }

    public void UpdateMoney(int _cost)
    {
        RefreshMoney();
        money += _cost;
        string _sql = "update user set money = '" + money + "' where userid = '" + userdata.Userid+ "'";
        dbm.QueryData(_sql);
    }

    private void UpdateMoney(string _nick, int _cost)
    {

        string sql = "select money from user where nickname='" + _nick + "'";
        List<string[]> result = dbm.GetData(sql);
        int _money = int.Parse(result[0][0]);
        _money += _cost;
        string _sql = "update user set money = '" + _money + "' where nickname = '" + _nick + "'";
        dbm.QueryData(_sql);
    }


    public void SyncEcoRecive(string _from, string _to, int _money)
    {
        if (userdata.Nickname == _to)
        {
            chatCtrl.ChatNotify(_from + "님 께서 " + _money + "원을 이체 하셨습니다.");
        }

    }


    public bool CheckAndSubMoney(int _cost)
    {
        RefreshMoney();
        if (money < _cost)return false;
        UpdateMoney(_cost * -1);
        return true;
    }



    public void Money(object[] data)
    {
        Debug.Log("data : " + data.Length);
        if (data.Length == 1 && (string)data[0] == "")
        {
            RefreshMoney();
            chatCtrl.ChatNotify("===========[ Money ]===========");
            chatCtrl.ChatNotify(" [+] 잔액 : " + money + "원");
            chatCtrl.ChatNotify(" [+] 송금하기 : /money send 닉네임 금액");
            chatCtrl.ChatNotify("===========[ Money ]===========");
        }

        if (((string)data[0]).ToUpper() == "send".ToUpper())
        {
            string sql = "select count(*) from user where nickname='" + (string)data[1] + "'";
            List<string[]> result = dbm.GetData(sql);
            if (int.Parse(result[0][0]) <= 0)
            {
                chatCtrl.ChatNotify((string)data[1] + " 유저를 찾을 수 없습니다.");
                return;
            }
            if(int.Parse((string)data[2]) <= 0)
            {
                chatCtrl.ChatNotify("전송할 수 없는 금액 입니다");
                return;
            }
            if (money < int.Parse((string)data[2]))
            {
                chatCtrl.ChatNotify("잔액이 부족 합니다.");
                return;
            }
            UpdateMoney(int.Parse((string)data[2]) * -1);
            UpdateMoney((string)data[1], int.Parse((string)data[2]));
            chatCtrl.ChatNotify((string)data[1]+ "님 께 "+ (string)data[2] + "원 이체가 완료 되었습니다");
            object[] _data = { userdata.Nickname, (string)data[1], int.Parse((string)data[2]) };
            evc.SendRaiseEvent(EventController.EVENTCODE.ECORECIVE, _data, EventController.SEND_OPTION.OTHER);
            return;
        }
        if (((string)data[0]).ToUpper() == "view".ToUpper())
        {
            RefreshMoney();
            chatCtrl.ChatNotify("잔액 : "+ money + "원");
        }




    }



}
