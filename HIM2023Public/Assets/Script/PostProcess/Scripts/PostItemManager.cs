using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory.Examples;
using UnityEngine.UI;
using FarrokhGames.Inventory;


public class PostItemManager : MonoBehaviour
{
    public ItemDefinition _def;
    public string ItemDataName;
    public Text ItemNameText;
    public Image ItemIcon;
    private IInventoryItem requestItem;
    public string ItemDataId;
    public DBManager dbm;
    public PostManager mgr;


    public void SetGUI()
    {
        Debug.Log("ItemDataName : " + ItemDataName);
        Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + ItemDataName+"_Shop");
        GameObject _requestItem = (GameObject)Resources.Load("Prefab/" + ItemDataName);
        requestItem = _requestItem.GetComponent<GroundItem>().itemDef.CreateInstance();
        ItemNameText.text = (requestItem as ItemDefinition).itemName;
        ItemIcon.sprite = sprites[0];
    }
    private void Start()
    {
        SetGUI();
    }

    private void GetItemToDeleteRow(string _id)
    {
        string _sql = "delete from postitem where id = "+_id;
        dbm.QueryData(_sql);

    }

    public void GetItem()
    {
        if (PlayerInventory.fastAddItem(requestItem) == true)
        {
            GetItemToDeleteRow(ItemDataId);
            mgr.SubCount();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("인벤 다참");
        }

    }


}
