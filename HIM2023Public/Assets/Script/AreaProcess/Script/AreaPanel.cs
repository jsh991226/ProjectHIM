using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaPanel : MonoBehaviour
{
    public Text titleText;
    public Text statusText;
    public Text actionBtnText;


    private AreaEntity myArea;
    private GameObject areaView;
    private UserData userdata;
    private AreaStatusCtrl areaStatus;
    private GameObject areaCtrl;




    private void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

    }



    public void SetContent(AreaEntity _myArea,GameObject _areaCtrl, GameObject _areaView, AreaStatusCtrl _areaStatus) //����ο� : 3/5
    {
        myArea = _myArea;
        areaView = _areaView;
        areaStatus = _areaStatus;
        areaCtrl = _areaCtrl;
        titleText.text = _myArea.Title;
        statusText.text = "��� �ο� : 0 / " + myArea.PeopleMax;
        actionBtnText.text = "�۾��� ã��";
    }

    public void SetPeople(int _num, int _myJoinArea)
    {
        myArea.PeopleNow = _num;
        statusText.text = "��� �ο� : "+ _num+  " /  " + myArea.PeopleMax;
        if (_num > 0) actionBtnText.text = "�۾� �Բ��ϱ�";
        else actionBtnText.text = "�۾��� ã��";
        if (_num >= myArea.PeopleMax) actionBtnText.text = "���� �Ұ�";
        if (_myJoinArea == myArea.Id) actionBtnText.text = "������";


    }

    public void ClickInfo()
    {
        areaView.GetComponent<PanelManager>().GUIToggle(true);
        areaView.GetComponent<AreaView>().SetContent(myArea);
    }

    public void ClickJoin()
    {
        if (myArea.PeopleNow >= myArea.PeopleMax) return;
        if (actionBtnText.text == "������") return;
        areaCtrl.GetComponent<AreaCtrl>().JoinArea(myArea, userdata.Nickname);
        areaCtrl.GetComponent<PanelManager>().GUIToggle(false);
        areaStatus.GUISetting(myArea);
    }


}
