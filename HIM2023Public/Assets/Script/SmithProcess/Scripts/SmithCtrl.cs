using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using UnityEngine.UI;

public class SmithCtrl : MonoBehaviour
{
    [SerializeField]
    private PlayerInventory playerInv;
    [SerializeField]
    private List<GameObject> InvItemSlot;
    [SerializeField]
    private List<GameObject> MatItemSlot;
    [SerializeField]
    private GameObject ItemSlotGroup;
    [SerializeField]
    private Sprite _emptyImg;
    [SerializeField]
    private Text SucPerText;
    [SerializeField]
    private Text SmithCostText;
    [SerializeField]
    private List<ItemDefinition> resultItem;
    [SerializeField]
    private EcoCtrl ecoCtrl;
    [SerializeField]
    private ChatCtrl chatCtrl;
    [SerializeField]
    private NotifyCtrl notifyCtrl;
    [SerializeField]
    private DBManager dbm;
    private UserData userdata;
    private int _per = 0;
    private int _cost = 0;
    [SerializeField]
    private GameObject _anvil;
    [SerializeField]
    private AudioClip successSound;
    [SerializeField]
    private AudioClip failSound;
    [SerializeField]
    private AudioSource SmithSource;


    public List<IInventoryItem> MatSlot = new List<IInventoryItem>();
    public List<IInventoryItem> InvSlot = new List<IInventoryItem>();

    private void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

        for ( int i = 0; i < ItemSlotGroup.transform.childCount; i++)
        {
            InvItemSlot.Add(ItemSlotGroup.transform.GetChild(i).gameObject);
        }

    }

    public void ActionSmith()
    {
        if (!ecoCtrl.CheckAndSubMoney(_cost))
        {
            chatCtrl.ChatNotify("������ �����ϱ� ���� �ݾ��� �����մϴ�.");
            return;
        }
        if (MatSlot.Count <= 0)
        {
            chatCtrl.ChatNotify("������ ������ ��ᰡ �����ϴ�.");
            return;
        }
        int randomNum = Random.Range(0, 100);
        foreach(IInventoryItem _item in MatSlot)
        {
            playerInv.fastRemoveItemDynamic(_item);
        }
        if (randomNum < _per) //����
        {
            StartCoroutine(smithGUI(true));

        } else
        {
            StartCoroutine(smithGUI(false));

        }

    }

    IEnumerator smithGUI(bool _com)
    {
        _anvil.SetActive(true);
        yield return new WaitForSeconds(3f);
        _anvil.SetActive(false);
        gameObject.GetComponent<PanelManager>().GUIToggle(false);
        if (_com)
        {
            SmithSource.clip = successSound;
            SmithSource.Play();
            int resultNum = Random.Range(0, resultItem.Count);
            notifyCtrl.CastNotify("������ �����Ͽ� " + resultItem[resultNum].itemName + "��(��) ȹ���Ͽ����ϴ� ������[P]���� Ȯ�����ּ���");
            string _sql = "INSERT INTO postitem (ownerid, itemdataname, vaild) VALUES ('" + userdata.Userid + "', '" + resultItem[resultNum].itemPrefabName + "', '0')";
            dbm.QueryData(_sql);
            Debug.Log("resultItem[randomNum].itemPrefabName : " + resultItem[resultNum].itemPrefabName);
        } else
        {
            SmithSource.clip = failSound;
            SmithSource.Play();
            notifyCtrl.CastNotify("������ �����Ͽ����ϴ�, ��ᰡ ��� �Ҹ�˴ϴ�");
        }

    }

    public void ReFreshGUI()
    {

        //reset
        foreach (GameObject invSlotImg in InvItemSlot)
        {
            invSlotImg.GetComponent<Image>().sprite = _emptyImg;
            invSlotImg.GetComponent<SlotSwap>()._myItem = null;
        }
        foreach (GameObject invSlotImg in MatItemSlot)
        {
            invSlotImg.GetComponent<Image>().sprite = _emptyImg;
            invSlotImg.GetComponent<SlotSwap>()._myItem = null;
        }
        //set
        int i = 0;
        foreach (IInventoryItem _item in InvSlot)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + (_item as ItemDefinition).itemPrefabName + "_Shop");
            InvItemSlot[i].GetComponent<Image>().sprite = sprites[0];
            InvItemSlot[i].GetComponent<SlotSwap>()._myItem = _item;
            i++;
        }

        i = 0;
        _per = 0;
        foreach (IInventoryItem _item in MatSlot)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + (_item as ItemDefinition).itemPrefabName + "_Shop");
            if ((_item as ItemDefinition).Type.ToString() =="Rune")
            {
                _per += (int)(_item as ItemDefinition).damage;
            } else
            {
                _per += ((_item as ItemDefinition).width + 1) * ((_item as ItemDefinition).height + 1);
            }
            MatItemSlot[i].GetComponent<Image>().sprite = sprites[0];
            MatItemSlot[i].GetComponent<SlotSwap>()._myItem = _item;
            i++;
        }

        SucPerText.text = _per + "%";
        _cost = _per / 2 * 100;
        if (_cost < 0 ) _cost = 0;
        SmithCostText.text = "���� ��� : " + (string)_cost.ToString("#,##0") + "��";
    }

    public void OpenSetGUI()
    {
        _anvil.SetActive(false);
        IInventoryItem[] _items = playerInv.GetItemList();
        MatSlot = new List<IInventoryItem>();
        InvSlot = new List<IInventoryItem>();
        for (int i = 0; i < _items.Length; i++)
        {
            IInventoryItem _item = _items[i];
            InvSlot.Add(_item);
        }
        ReFreshGUI();

    }






}
