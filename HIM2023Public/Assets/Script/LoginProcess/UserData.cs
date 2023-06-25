using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    [SerializeField]
    private string userid;
    [SerializeField]
    private string userClass;
    [SerializeField]
    private string nickname;


    private bool isGaming = false;

    private int nowPlayerCnt = 0;



    public string Userid { get => userid; set => userid = value; }
    public string UserClass { get => userClass; set => userClass = value; }
    public string Nickname { get => nickname; set => nickname = value; }
    public bool IsGaming { get => isGaming; set => isGaming = value; }
    public int NowPlayerCnt { get => nowPlayerCnt; set => nowPlayerCnt = value; }

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        var obj = FindObjectsOfType<UserData>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
