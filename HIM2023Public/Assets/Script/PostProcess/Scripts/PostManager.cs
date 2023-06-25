using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PostManager : MonoBehaviour
{
    private UserData userdata;
    public NotifyCtrl notify;
    private float UpdateTime = 10;
    private DateTime nextCheckTime;
    public GameObject ItemPanelPrefab;
    public GameObject PostList;
    public DBManager dbm;
    public Text CountText;
    private int ListCount;

    private void Awake()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

    }
    private void Start()
    {
        nextCheckTime = DateTime.Now;
        nextCheckTime = nextCheckTime.AddSeconds(UpdateTime);
    }

    private void Update()
    {
        if (nextCheckTime < DateTime.Now)
        {
            nextCheckTime = DateTime.Now;
            nextCheckTime = nextCheckTime.AddSeconds(UpdateTime);
            StartCoroutine(PostCheck());
        }

    }

    public void PostRefresh()
    {
        StartCoroutine(PostRefreshIenum());
    }

    public void ClearTrans(Transform _trans)
    {
        foreach (Transform child in _trans)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void SubCount()
    {
        CountText.text = "수락하지 않은 우편물 : " + --ListCount + "개";
    }


    IEnumerator PostRefreshIenum()

    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("http://127.0.0.1/postitem/getPostItem.do?owner=" + userdata.Userid))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                CountText.text = "[알림] 서버가 일부 점검중입니다";
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                if (request.downloadHandler.text == "null")
                {
                    CountText.text = "수락하지 않은 우편물이 없습니다.";
                    ListCount = 0;
                    yield return null;

                }
                else
                {
                    ClearTrans(PostList.transform);
                    string[] ItemList = request.downloadHandler.text.Split("/");
                    CountText.text = "수락하지 않은 우편물 : "+ (ItemList.Length-1) +"개";
                    ListCount = ItemList.Length - 1;
                    for (int i = 1; i < ItemList.Length; i++)
                    {
                        string[] _splitTemp = ItemList[i].Split("-");
                        GameObject _panel = Instantiate(ItemPanelPrefab);
                        _panel.transform.SetParent(PostList.transform, false);
                        _panel.GetComponent<PostItemManager>().ItemDataName = _splitTemp[0];
                        _panel.GetComponent<PostItemManager>().ItemDataId = _splitTemp[1];
                        _panel.GetComponent<PostItemManager>().dbm = dbm;
                        _panel.GetComponent<PostItemManager>().mgr = gameObject.GetComponent<PostManager>();
                        _panel.GetComponent<PostItemManager>().SetGUI();
                    }



                }

            }
        }
    }



    IEnumerator PostCheck()

    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("http://127.0.0.1/postitem/getNewPost.do?owner=" + userdata.Userid))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string[] _temp = request.downloadHandler.text.Split("*/$@");
                string _checkStr = _temp[1];
                if (_checkStr =="1")
                {
                    notify.CastNotify("우편함에 새로운 상품이 도착 했습니다");
                }


            }
        }
    }



}
