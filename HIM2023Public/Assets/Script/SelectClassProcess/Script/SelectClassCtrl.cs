using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectClassCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    public string nowSel;
    public CardRollManager rollManager;
    public GameManager gameManager;
    public DBManager dbm;


    void Start()
    {
        nowSel = rollManager.cardEntity[rollManager.cardEntity.Count / 2].GetComponent<CardEntity>().myClass;
    }

    // Update is called once per frame
    public void ActiveSelect()
    {
        string userid = gameManager.userdata.Userid;
        dbm.QueryData("update user set userclass = '"+ nowSel + "' where userid = '" + userid+"'");
        gameManager.userdata.UserClass = nowSel;
        gameManager.SpawnPlayer();

    }


}
