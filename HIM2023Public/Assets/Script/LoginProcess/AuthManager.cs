using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("Ref Object Setting")]
    public GameObject DBManager;
    public GameObject panelMgr;
    [Header("Login InputField Setting")]
    public InputField idField;
    public InputField pwField;
    [Header("Register InputField Setting")]
    public InputField RegIdField;
    public InputField RegPwField;
    public InputField RegPwCheckField;
    public InputField RegNickField;
    public InputField RegEmailField;

    [Header("Debug Msg Text Setting")]
    public Text DebugMsgReg;
    public Text DebugMsgLogin;


    private DBManager dbm;
    private UserData userdata;
    private TitleManager titleManager;

    public GameObject testscript;
    public PanelManager loadingPnl;

    public AudioClip failSound;
    public AudioSource guiSource;


    void Start()
    {
        dbm = DBManager.GetComponent<DBManager>();
        userdata = GameObject.Find("UserData").GetComponent<UserData>();
        titleManager = GameObject.Find("TitleManager").GetComponent<TitleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //testscript.GetComponent<testscript>().DisableMethod();
    }
    
    public void ActionLogin()
    {
        string id = idField.GetComponent<InputField>().text;
        string pw = pwField.GetComponent<InputField>().text;
        List<string[]> result = dbm.GetData("select userid, nickname, userclass from user where userid= '" + id + "' and userpw = '" + pw + "'");
        CompleteLogin(result);
    }
    private void CompleteLogin(List<string[]> _result)
    {
        if(Convert.ToBoolean(_result.Count))
        {
            userdata.Userid = _result[0][0];
            userdata.Nickname = _result[0][1];
            userdata.UserClass = _result[0][2];
            loadingPnl.GUIToggle(true);
            titleManager.GoToLobby();
        } else
        {
            guiSource.clip = failSound;
            guiSource.Play();
            DebugMsgLogin.text = "ȸ�� ������ ��ġ���� �ʽ��ϴ�.";
        }

    }

    public bool CheckID(string _value, string _column)
    {
        List<string[]> result = dbm.GetData("select count(*) from user where "+ _column+"= '" + _value + "'");
        return Convert.ToBoolean(int.Parse(result[0][0]));
    }

    public bool IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        return valid;
    }


    public void ActionRegister()
    {
        string id = RegIdField.GetComponent<InputField>().text;
        string pw = RegPwField.GetComponent<InputField>().text;
        string pwck = RegPwCheckField.GetComponent<InputField>().text;
        string nick = RegNickField.GetComponent<InputField>().text;
        string email = RegEmailField.GetComponent<InputField>().text;
        string regdate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        Debug.Log("id : " + id + " pw : " + pw + " pwck : " + pwck + " nick : " + nick + " email : " + email);
        if (id.Length == 0 || pw.Length == 0 || pwck.Length == 0 || nick.Length == 0 || email.Length == 0 || regdate.Length == 0)
        {
            DebugMsgReg.text = "������ ������ �ֽ��ϴ�, �ٽ� �Է����ּ���.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }
        if (nick.Length > 8)
        {
            DebugMsgReg.text = "�г����� �ִ� 8 ���� �Դϴ�.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }

        if (CheckID(id, "userid"))
        {
            DebugMsgReg.text = "�̹� �����ϴ� ���̵� �Դϴ�.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }
        if (CheckID(nick, "nickname"))
        {
            DebugMsgReg.text = "�̹� �����ϴ� �г��� �Դϴ�.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }
        if (CheckID(email, "email"))
        {
            DebugMsgReg.text = "�̹� �����ϴ� �̸��� �Դϴ�.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }
        if (!IsValidEmail(email))
        {
            DebugMsgReg.text = "�̸��� ����� �ùٸ��� �ʽ��ϴ�.";
            guiSource.clip = failSound;
            guiSource.Play();
            return;
        }

        if (pw != pwck)
        {
            DebugMsgReg.text = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
            guiSource.Play();

            return;
        }
        string _sql = "INSERT INTO user (userid, userpw, nickname, email, regdate) VALUES ('"+id+ "', '" + pw + "', '" + nick + "', '" + email + "', '" + regdate + "')";
        int _result = dbm.QueryData(_sql);
        if (_result == 1)
        {
            guiSource.Play();
            Debug.Log("register complete");
            panelMgr.GetComponent<PanelManager>().GUIToggle(false);
        }
    }




}
