using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaStatusCtrl : MonoBehaviour
{
    [Header("Area Status Object")]
    public Text titleText;
    public List<GameObject> Icons;
    public List<GameObject> playerCards;
    public GameObject ownerBtn;
    public GameObject noneOwnerBtn;
    public Text minPeopleText;


    [SerializeField]
    private DBManager dbm;


    private AreaEntity myArea;
    private UserData userdata;

    private void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
    }


    public void SyncGUISetting()
    {
        Debug.Log("gui sync title : " + myArea.Title);
        GUISetting(myArea);
    }


    public void GUISetting(AreaEntity _myArea)
    {
        myArea = _myArea;
        titleText.text = myArea.Title;
        minPeopleText.text = "작전 최소 인원 "+ myArea.PeopleMin + "명 달성시 시작 가능";
        if (myArea.Owner == userdata.Nickname)
        {
            noneOwnerBtn.SetActive(false);
            ownerBtn.SetActive(true);
        }
        else
        {
            noneOwnerBtn.SetActive(true);
            ownerBtn.SetActive(false);
        }
        for ( int i = 0; i < Icons.Count; i++)
        {
            if (myArea.PeopleMax > i) Icons[i].SetActive(true);
            else Icons[i].SetActive(false);
            if (myArea.PeopleNow > i)Icons[i].GetComponent<OrcIconCtrl>().SetAble(true);
            else Icons[i].GetComponent<OrcIconCtrl>().SetAble(false);

        }
        int cnt = 0;
        foreach (GameObject obj in playerCards)
        {
            obj.SetActive(false);
        }

        foreach (string _nick in myArea.PlayerList)
        {
            List<string[]> results = dbm.GetData("select userclass from user where nickname='" + _nick + "'");
            string _userClass = results[0][0];
            playerCards[cnt].SetActive(true);
            Debug.LogError("[test] nick : " + _nick + " indexOf : " + myArea.readyPlayer.IndexOf(_nick));
            playerCards[cnt++].GetComponent<PlayerCardEntity>().GUISetting(_userClass, _nick, myArea.Owner, myArea.readyPlayer.IndexOf(_nick));
        }

        gameObject.GetComponent<PanelManager>().GUIToggle(true);
    }
    


}
