using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using Photon.Pun;
using UnityEngine.EventSystems;
using System.Linq;

namespace FarrokhGames.Inventory.Examples
{
    [RequireComponent(typeof(InventoryRenderer))]
    public class PlayerInventory : MonoBehaviourPunCallbacks
    {
        public GameObject EquipUI;
        public GameObject dropZone;
        public DBManager dbm;
        public GameObject playerObj;
        [SerializeField] private InventoryRenderMode _renderMode = InventoryRenderMode.Grid;
        [SerializeField] private int _maximumAlowedItemCount = -1;
        [SerializeField] private ItemType _allowedItem = ItemType.Any;
        [SerializeField] private int _width = 4;
        [SerializeField] private int _height = 6;
        public static InventoryManager inventory;
        public Text actionText;
        private IInventoryItem hoverdItem;
        [SerializeField]
        private GameObject itemInfo;
        [SerializeField] private Text itemNameTxt, itemAreaTxt, itemDuraTxt, itemDescTxt;
        private UserData userdata;
        private Vector2 _localCursor;
        private IInventoryItem rightClickItem;
        [SerializeField]
        private Text ItemEquipNameText;
        [SerializeField]
        private ReEquip LeftHand;
        [SerializeField]
        private ReEquip RightHand;

        private IInventoryItem leftHandItem;
        private IInventoryItem rightHandItem;
        private string _nickName;

        public NotifyCtrl notify;
        public ChatCtrl chatCtrl;
        public Sprite emptyHandImg;
        public GameObject equipUI;
        public EcoCtrl ecoCtrl;

        

        public IInventoryItem[] GetItemList()
        {
            return inventory.allItems;
        }



        public void UpdateInvDB()
        {
            string sql = "delete from itemdata where ownerId = '" + userdata.Userid + "'";
            dbm.QueryData(sql);
            string sprite, ispartofshape, candrop, durability, area, code, itemprefabname;
            int positionx, positiony, width, height;
            List<string> sqlList = new List<string>();
            foreach (IInventoryItem _item in inventory.allItems)
            {
                ItemDefinition _insertItem = _item as ItemDefinition;
                sprite = _insertItem.spriteName;
                ispartofshape = "true";
                candrop = _insertItem.canDrop + "";
                durability = _insertItem.durability+"";
                code = _insertItem.code;
                itemprefabname = _insertItem.itemPrefabName;
                positionx = _insertItem.position.x;
                positiony = _insertItem.position.y;
                width = _insertItem.width;
                height = _insertItem.height;
                area = _insertItem.area + "";
                if (!(area.Length > 0)) area = "noarea";
                sql = string.Format("insert into itemdata(sprite, positionx, positiony, width, height, ispartofshape, candrop, durability, area, code, itemprefabname, ownerId) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", sprite, positionx, positiony, width, height, ispartofshape, candrop, durability, area, code, itemprefabname, userdata.Userid);
                sqlList.Add(sql);
                Debug.Log("sql : " + sql);
            }
            dbm.QueryData(sqlList);

        }

        public void LoadInvDB()
        {
            Debug.Log("염");

            string sprite, ispartofshape, candrop, durability, area, code, itemprefabname;
            int positionx, positiony, width, height;
            string sql = "select * from itemdata where ownerId = '" + userdata.Userid + "'";
            List<string[]> result;
            try
            {
                 result = dbm.GetData(sql);
            }catch
            {
                return;
            }
            dbm.ListToDebug(result);
            inventory.Clear();
            foreach (string[] _rows in result)
            {
                sprite = _rows[1];
                positionx= int.Parse(_rows[2]);
                positiony = int.Parse(_rows[3]);
                width = int.Parse(_rows[4]);
                height = int.Parse(_rows[5]);
                ispartofshape = _rows[6];
                candrop = _rows[7];
                durability = _rows[8];
                area = _rows[9];
                if (area == "noarea") area = "";
                code = _rows[10];
                itemprefabname = _rows[11];

                GameObject _spawnObj = (GameObject)Resources.Load("Prefab/" + itemprefabname);
                _spawnObj.GetComponent<GroundItem>().durability = int.Parse(durability);
                _spawnObj.GetComponent<GroundItem>().area = area;
                _spawnObj.GetComponent<GroundItem>().code = code;
                inventory.TryAddAt(_spawnObj.GetComponent<GroundItem>().itemDef.CreateInstance(), new Vector2Int(positionx, positiony));
                Debug.Log("[Debug] : " + code + " 소환~");
            }
        }



        void Start()
        {
            _nickName = PhotonNetwork.NickName;
            IInventoryItem o = (IInventoryItem)ScriptableObject.CreateInstance(typeof(ItemDefinition));
            userdata = GameObject.Find("UserData").GetComponent<UserData>();
            var provider = new InventoryProvider(_renderMode, _maximumAlowedItemCount, _allowedItem);
            inventory = new InventoryManager(provider, _width, _height);
            var allControllers = GameObject.FindObjectsOfType<InventoryController>();

            foreach (var controller in allControllers)
            {
                controller.onItemHovered += (item) => {
                    if (item == null)
                    {
                        hoverdItem = null;
                        itemInfo.SetActive(false);
                    }
                    else itemInfo.SetActive(true);
                    hoverdItem = item;
                    if (hoverdItem != null)
                    {
                        itemNameTxt.text = (hoverdItem as ItemDefinition).itemName;
                        itemAreaTxt.text = (hoverdItem as ItemDefinition).area;
                        itemDuraTxt.text = (hoverdItem as ItemDefinition).durability+"/100";
                        itemDescTxt.text = (hoverdItem as ItemDefinition).describe;
                    }
                };
            }

            GetComponent<InventoryRenderer>().SetInventory(inventory, provider.inventoryRenderMode);

            // Log items being dropped on the ground
            inventory.onItemDropped += (item) => //아이템 드랍 할 때
            {
                GameObject clone = PhotonNetwork.Instantiate("Prefab/" + (item as ItemDefinition).itemPrefabName, new Vector3(dropZone.transform.position.x, dropZone.transform.position.y, dropZone.transform.position.z), 
                    new Quaternion(dropZone.transform.rotation.x, dropZone.transform.rotation.y, dropZone.transform.rotation.z, dropZone.transform.rotation.w));

                clone.GetComponent<GroundItem>().itemName = (item as ItemDefinition).itemName;
                clone.GetComponent<GroundItem>().durability = (item as ItemDefinition).durability;
                clone.GetComponent<GroundItem>().area = (item as ItemDefinition).area;
                clone.GetComponent<GroundItem>().code = (item as ItemDefinition).code;
                clone.GetComponent<GroundItem>().describe = (item as ItemDefinition).describe;
                clone.GetComponent<GroundItem>().damage = (item as ItemDefinition).damage;

                //clone.GetComponent<GroundItem>().durability -= 10;

                clone.name = (item as ItemDefinition).itemObject.name + " " + clone.GetComponent<GroundItem>().durability + " " + clone.GetComponent<GroundItem>().area;

                Debug.Log((item as ItemDefinition).itemObject.name + ", 내구도: " + clone.GetComponent<GroundItem>().durability + ", 지역: " + clone.GetComponent<GroundItem>().area + ", 코드: " + clone.GetComponent<GroundItem>().code);
            };

            // Log when an item was unable to be placed on the ground (due to its canDrop being set to false)
            inventory.onItemDroppedFailed += (item) =>
            {
                Debug.Log($"You're not allowed to drop {(item as ItemDefinition).Name} on the ground");
            };

            // Log when an item was unable to be placed on the ground (due to its canDrop being set to false)
            inventory.onItemAddedFailed += (item) =>
            {
                Debug.Log($"You can't put {(item as ItemDefinition).Name} there!");
            };

            inventory.onItemAdded += (item) => //아이템 먹을때
            {
                if (actionText.text == "Open 'Q'" && actionText.IsActive())
                {


                    for (int i = ActionController.lookedItem.GetComponent<GroundBag>().itemList.Count - 1; i >= 0; i--)
                    {
                        if (ActionController.lookedItem.GetComponent<GroundBag>().itemsCode[i] == (item as ItemDefinition).code)
                        {
                            Debug.Log((item as ItemDefinition).code + "코드 아이템 가져옴");
                            ActionController.lookedItem.GetComponent<GroundBag>().itemList.RemoveAt(i);
                            ActionController.lookedItem.GetComponent<GroundBag>().itemsCode.RemoveAt(i);
                            ActionController.lookedItem.GetComponent<GroundBag>().itemsPosition.RemoveAt(i);
                        }
                    }
                }
                if (ActionController.lookedItem != null)
                {
                    //notify.CastNotify((item as ItemDefinition).itemName + "을(를) 획득 했습니다.");
                    chatCtrl.ChatNotify((item as ItemDefinition).itemName + "을(를) 획득 했습니다.");
                    StartCoroutine(UpdateInvIEnum());

                }



                if ((item as ItemDefinition).itemObject.GetComponent<GroundItem>().code == "" || (item as ItemDefinition).code == ""
                    || (item as ItemDefinition).itemObject.GetComponent<GroundItem>().code.Contains("_groundbag_") || (item as ItemDefinition).code.Contains("_groundbag_"))
                {
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().code = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    (item as ItemDefinition).code = (item as ItemDefinition).itemObject.GetComponent<GroundItem>().code;
                    Debug.Log((item as ItemDefinition).code + "로 코드 변경");
                }


            };
            inventory.onItemRemoved += (item) =>
            {

            };

            LoadInvDB();
        }

        public void UpdateInvCor()
        {
            StartCoroutine(UpdateInvIEnum());
        }

        IEnumerator UpdateInvIEnum()
        {
            UpdateInvDB();

            yield return null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
                eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
                return;

            Debug.Log("LocalCursor:" + localCursor);
        }


        public void RemoveChild(Transform[] childList)
        {
            if (childList != null)
            {
                for (int i = 1; i < childList.Length; i++)
                {
                    if (childList[i] != transform)
                        Destroy(childList[i].gameObject);
                }
            }
        }

        public void DisableChild(Transform[] childList)
        {
            if (childList != null)
            {
                for (int i = 1; i < childList.Length; i++)
                {
                    if (childList[i] != transform)
                        childList[i].gameObject.SetActive(false);
                }
            }
        }

        public void EquipItem(bool _handType) //false == lefthand, true == righthand
        {
            PlayerManager _pm = playerObj.GetComponent<PlayerManager>();
            equipUI.SetActive(false);
            if ((rightClickItem as ItemDefinition).Type == ItemType.Rune)
            {
                return;
            }
            if (_handType)
            {
                if (RightHand.isEmpty != true)
                {
                    chatCtrl.ChatNotify("슬롯의 장비를 해제 하고 실행해주세요.");
                    return;
                }

                rightHandItem = rightClickItem;
                Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + (rightClickItem as ItemDefinition).itemPrefabName + "_Shop");
                RightHand.SetEquip(sprites[0], rightHandItem, false, _handType);
                inventory.TryRemove(rightHandItem);
                RightHand.playerMgr = playerObj.GetComponent<PlayerManager>();
                playerObj.GetComponent<PlayerManager>().ChangeWeapon(_pm, (rightHandItem as ItemDefinition).itemPrefabName, _handType, (rightHandItem as ItemDefinition).damage, false);
                
            }
            else
            {
                if (LeftHand.isEmpty != true)
                {
                    chatCtrl.ChatNotify("슬롯의 장비를 해제 하고 실행해주세요.");
                    return;
                }

                leftHandItem = rightClickItem;
                Sprite[] sprites = Resources.LoadAll<Sprite>("ShopPrefab/" + (rightClickItem as ItemDefinition).itemPrefabName + "_Shop");

                LeftHand.SetEquip(sprites[0], leftHandItem, false, _handType);
                inventory.TryRemove(leftHandItem);
                LeftHand.playerMgr = playerObj.GetComponent<PlayerManager>();
                playerObj.GetComponent<PlayerManager>().ChangeWeapon(_pm, (leftHandItem as ItemDefinition).itemPrefabName, _handType, (leftHandItem as ItemDefinition).damage, false);
            }

        }


        /*        public void EquipItem(bool _handType) //false == lefthand, true == righthand
                {
                    if (_handType)
                    {
                        RightHand.sprite = rightClickItem.sprite;
                        rightHandItem = rightClickItem;
                        RemoveChild(playerObj.GetComponent<PlayerManager>().rightHandTr.GetComponentsInChildren<Transform>());
                        GameObject _temp = (Resources.Load("RightHandPrefab/" + (rightHandItem as ItemDefinition).itemPrefabName + "_Hand_Right") as GameObject);
                        GameObject _obj = PhotonNetwork.Instantiate("RightHandPrefab/" + (rightHandItem as ItemDefinition).itemPrefabName + "_Hand_Right", _temp.transform.position, _temp.transform.rotation);
                        //GameObject _obj = Instantiate((Resources.Load("RightHandPrefab/" + (rightHandItem as ItemDefinition).itemPrefabName + "_Hand_Right") as GameObject));
                        _obj.transform.SetParent(playerObj.GetComponent<PlayerManager>().rightHandTr, false);

                    }
                    else
                    {
                        LeftHand.sprite = rightClickItem.sprite;
                        leftHandItem = rightClickItem;
                        RemoveChild(playerObj.GetComponent<PlayerManager>().leftHandTr.GetComponentsInChildren<Transform>());
                        GameObject _temp = (Resources.Load("LeftHandPrefab/" + (leftHandItem as ItemDefinition).itemPrefabName + "_Hand_Left") as GameObject);
                        GameObject _obj = PhotonNetwork.Instantiate("LeftHandPrefab/" + (leftHandItem as ItemDefinition).itemPrefabName + "_Hand_Left", _temp.transform.position, _temp.transform.rotation);
                        _obj.transform.SetParent(playerObj.GetComponent<PlayerManager>().leftHandTr, false);
                    }

                }*/


        //RemoveChild(playerObj.GetComponent<PlayerManager>().leftHandTr.GetComponentsInChildren<Transform>());
        //GameObject _obj = Instantiate((Resources.Load("LeftHandPrefab/" + (leftHandItem as ItemDefinition).itemPrefabName + "_Hand_Left") as GameObject));
        //_obj.transform.SetParent(playerObj.GetComponent<PlayerManager>().leftHandTr, false);




        void Update()
        {
            Cursor.lockState = CursorLockMode.Confined;

            if (hoverdItem != null && Input.GetMouseButtonDown(1))
            {
                if (EquipUI.active == true)
                {
                    EquipUI.SetActive(false);
                    return;
                }
                rightClickItem = hoverdItem;
                float _posX = (hoverdItem.position.x) * 50;
                float _posY = (hoverdItem.position.y) * 50 - hoverdItem.height*50;
                EquipUI.SetActive(true);
                ItemEquipNameText.text = (rightClickItem as ItemDefinition).itemName;
                EquipUI.transform.localPosition = new Vector2(_posX, _posY);

            }

            if (hoverdItem != null && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
            {
                return;
                var WatchingItem = ActionController.lookedItem.GetComponent<GroundBag>();
                for (int i = ActionController.lookedItem.GetComponent<GroundBag>().itemList.Count - 1; i >= 0; i--)
                {
                    if (ActionController.lookedItem.GetComponent<GroundBag>().itemsCode[i] == (hoverdItem as ItemDefinition).code)
                    {
                        if (fastAddItem(hoverdItem) == true)
                        {
                            OtherInventory.OtherInventoryClear();
                            WatchingItem.watched = false;
                            OtherInventory.ShowWatchingBag();
                            return;
                        }
                    }
                }
                if (OtherInventory.fastAddItem(hoverdItem) == true) fastRemoveItem(hoverdItem);
            }

        }



         public bool AddWatchingItem()
        {
            var WatchingItem = ActionController.lookedItem.GetComponent<GroundItem>();

            if (WatchingItem.itemName.Contains("Money-"))
            {
                string[] _temp = WatchingItem.itemName.Split("-");
                int Money = int.Parse(_temp[1]);
                Debug.Log("머근 돈 : " + Money);
                ecoCtrl.UpdateMoney(Money);
                chatCtrl.ChatNotify("돈주머니 에서 " + (string)Money.ToString("#,##0") + "원" + " 을 획득했습니다");
                
                return true;
            }

            if (inventory.CanAdd(WatchingItem.itemDef.CreateInstance()) == false) return false;

            WatchingItem.itemDef.itemName = WatchingItem.itemName;
            WatchingItem.itemDef.durability = WatchingItem.durability;
            WatchingItem.itemDef.area = WatchingItem.area;
            if (WatchingItem.code == "")
                WatchingItem.code = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            WatchingItem.itemDef.code = WatchingItem.code;
            WatchingItem.itemDef.describe = WatchingItem.describe;
            WatchingItem.itemDef.damage = WatchingItem.damage;
            inventory.TryAdd(WatchingItem.itemDef.CreateInstance());

            Debug.Log(WatchingItem.itemDef.itemObject.name + ", 내구도: " + WatchingItem.durability + ", 지역: " + WatchingItem.area + ", 코드: " + WatchingItem.code);
            return true;
        }


        static public bool fastAddItem(IInventoryItem item)
        {
            if (inventory.CanAdd((item as ItemDefinition).CreateInstance()) == false) return false;
            inventory.TryAdd((item as ItemDefinition).CreateInstance());    
            return true;
        }
        static public void fastRemoveItem(IInventoryItem item)
        {
            inventory.TryRemove(item);
        }
        public void fastRemoveItemDynamic(IInventoryItem item)
        {
            inventory.TryRemove(item);
        }



    }
}   