using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory.Examples;
using UnityEngine.UI;
using FarrokhGames.Inventory;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private int cost;

    [SerializeField]
    private Text ItemNameText;
    [SerializeField]
    private Text ItemCostText;
    [SerializeField]
    private Image ItemIcon;

    private ShopCtrl _ctrl;
    private ItemDefinition _def;
    private string ItemDataName;



    public void SetGUI(ItemDefinition _def, ShopCtrl _ctrl, int _cost)
    {
        this._def = _def;
        this._ctrl = _ctrl;
        this.cost = _cost;
        this.ItemDataName = _def.itemPrefabName;
        Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + ItemDataName + "_Shop");
        ItemNameText.text = _def.itemName;
        ItemCostText.text = (string)cost.ToString("#,##0") + "¿ø";
        ItemIcon.sprite = sprites[0];
    }

    public void BuyItem()
    {
        _ctrl.BuyItem(_def.itemName, cost, ItemDataName);
    }

}
