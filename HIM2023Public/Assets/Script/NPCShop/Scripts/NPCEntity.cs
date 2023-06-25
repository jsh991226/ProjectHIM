using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NPCEntity : MonoBehaviour
{
    [SerializeField]
    public string npcName;
    [SerializeField]
    public string npcDesc;
    [SerializeField]
    private TextMesh nick;
    public string openBtnText;

    [SerializeField]
    List<string> comments = new List<string>();

    [Header("OpenShop Event")]
    public UnityEvent OpenTrigger;
    [Header("CloseShop Event")]
    public UnityEvent CloseTrigger;

    private string webUrl = "http://127.0.0.1/user/gen.do?quest=";



    private void Start()
    {
        nick.text = npcName;  
        if (npcDesc.Length > 0) 
        {
            Debug.Log("[NPC : " + npcName + " is Use ChatGPT");
            GenComment(); 
        }

    }

    public string NPCTalk()
    {
        int rdm = Random.Range(0, comments.Count);
        return comments[rdm];
    }

    private void GenComment()
    {
        if (comments.Count < 5) StartCoroutine(GetComment());

    }

    IEnumerator GetComment()

    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get(webUrl + npcDesc))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string _addResult = request.downloadHandler.text.Replace("\n", "");
                Debug.Log(_addResult);
                if (_addResult.Contains("서버에"))
                {
                    Debug.LogError("[NPCEntity] 서버와 정상 통신 할수 없어 대사 생성이 중단됩니다");
                }
                else
                {
                    comments.Add(_addResult);
                    GenComment();
                }


            }
        }
    }



}
