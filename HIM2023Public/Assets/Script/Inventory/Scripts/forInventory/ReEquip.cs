using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FarrokhGames.Inventory.Examples;
using FarrokhGames.Inventory;


public class ReEquip : MonoBehaviour
{
    public GameObject reEquipText;
    public bool isEmpty;
    public IInventoryItem nowItem;
    public Sprite emptyImg;
    public NotifyCtrl notifyCtrl;
    public PlayerManager playerMgr;
    public bool handType;


    // Update is called once per frame

    private void Awake()
    {
        isEmpty = true;
    }
    public void SetEquip(Sprite _sprite,IInventoryItem _nowItem ,bool _isEmpty, bool _handType)
    {
        gameObject.GetComponent<Image>().sprite = _sprite;
        isEmpty = _isEmpty;
        nowItem = _nowItem;
        handType = _handType;
    }

    public void PointerStatus(bool _type)
    {
        if (isEmpty) return;
        reEquipText.SetActive(_type);
    }

    public void UseEquip()
    {
        isEmpty = true;
        gameObject.GetComponent<Image>().sprite = emptyImg;
        reEquipText.SetActive(false);
        nowItem = null;
    }

    public void ReEquipItem()
    {
        if (nowItem == null) return;
        if (PlayerInventory.fastAddItem(nowItem)) //������ ������ empty ���� Ȯ�� �ؾ���
        {
            isEmpty = true;
            gameObject.GetComponent<Image>().sprite = emptyImg;
            notifyCtrl.CastNotify((nowItem as ItemDefinition).itemName + "��(��) ���� �Ͽ����ϴ�.");
            reEquipText.SetActive(false);
            nowItem = null;
            playerMgr.ChangeWeapon(playerMgr, "empty", handType, 5, false);
        } else
        {
            notifyCtrl.CastNotify("�κ��丮 ������ �����Ͽ� ������ �� �����ϴ�.");
        }

    }



}
