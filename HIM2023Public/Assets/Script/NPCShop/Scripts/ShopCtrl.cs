using FarrokhGames.Inventory.Examples;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopCtrl : MonoBehaviour
{

    [SerializeField]
    private List<GroundItem> shopItems;
    [SerializeField]
    private List<int> shopCosts;
    [SerializeField]
    private GameObject ShopItemPanel;
    [SerializeField]
    private GameObject ShopItemListView;
    [SerializeField]
    private EcoCtrl _eco;
    [SerializeField]
    private ChatCtrl _chatCtrl;
    [SerializeField]
    private DBManager dbm;
    [SerializeField]
    private PanelManager postPnl;
    [SerializeField]
    private PostManager postMgr;

    private UserData userdata;


    void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

        int i = 0;
        foreach(GroundItem item in shopItems)
        {
            GameObject _panel = Instantiate(ShopItemPanel);
            _panel.transform.SetParent(ShopItemListView.transform, false);
            _panel.GetComponent<ShopItem>().SetGUI(item.itemDef, GetComponent<ShopCtrl>(), shopCosts[i++]);
        }
        
    }
    

    public void BuyItem(string _itemName, int _itemCost, string _ItemDataName)
    {
        if (_eco.CheckAndSubMoney(_itemCost))
        {
            string _sql = "INSERT INTO postitem (ownerid, itemdataname, vaild) VALUES ('" + userdata.Userid + "', '" + _ItemDataName + "', '0')";
            dbm.QueryData(_sql);
            _chatCtrl.ChatNotify(_itemName+"을(를) 구매했습니다, 우편함[P]을 확인해주세요!");
            if (postPnl.GUIStatus)
            {
                postMgr.PostRefresh();
            }
        }
        else
        {
            _chatCtrl.ChatNotify("잔고가 부족해 " + _itemName + "을(를) 구매할 수 없습니다.");
            return;
        }

    }







}
