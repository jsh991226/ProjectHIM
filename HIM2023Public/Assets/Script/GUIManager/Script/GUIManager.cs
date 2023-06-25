using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    [SerializeField]
    public GameObject[] GUIList;
    [Header("ChatSystem Option")]
    [SerializeField]
    private ChatCtrl chatCtrl;



    private GameObject nowOpened;

    void Start()
    {
       GUIList = GameObject.FindGameObjectsWithTag("GUI");
       foreach (GameObject obj in GUIList)
        {
            obj.GetComponent<PanelManager>().chatCtrl = chatCtrl;
        } 

    }


    public GameObject NowOpened { get => nowOpened; set => nowOpened = value; }

}
