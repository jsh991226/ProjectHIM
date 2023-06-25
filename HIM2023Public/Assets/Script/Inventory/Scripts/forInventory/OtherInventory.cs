using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;
using TMPro;

namespace FarrokhGames.Inventory.Examples
{
    [RequireComponent(typeof(InventoryRenderer))]
    public class OtherInventory : MonoBehaviour
    {
        static GameObject dropZone;
        [SerializeField] private InventoryRenderMode _renderMode = InventoryRenderMode.Grid;
        [SerializeField] private int _maximumAlowedItemCount = -1;
        [SerializeField] private ItemType _allowedItem = ItemType.Any;
        [SerializeField] private int _width = 4;
        [SerializeField] private int _height = 6;
        static InventoryManager inventory;
        public TMP_Text actionText;
        public List<GroundBag> Boxs;
        public EventController evc;


        void Start()
        {
            evc = GameObject.Find("EventController").GetComponent<EventController>();
            GameObject[] _temp = GameObject.FindGameObjectsWithTag("Bag");
            for (int i = 0; i < _temp.Length; i++) Boxs.Add(_temp[i].GetComponent<GroundBag>());

            var provider = new InventoryProvider(_renderMode, _maximumAlowedItemCount, _allowedItem);
            inventory = new InventoryManager(provider, _width, _height);

            GetComponent<InventoryRenderer>().SetInventory(inventory, provider.inventoryRenderMode);

            // Log items being dropped on the ground
            inventory.onItemDropped += (item) => {
                var clone = Instantiate((item as ItemDefinition).itemObject, new Vector3(dropZone.transform.position.x, dropZone.transform.position.y, dropZone.transform.position.z),
                    new Quaternion(dropZone.transform.rotation.x, dropZone.transform.rotation.y, dropZone.transform.rotation.z, dropZone.transform.rotation.w));

                clone.GetComponent<GroundItem>().durability = (item as ItemDefinition).durability;
                clone.GetComponent<GroundItem>().area = (item as ItemDefinition).area;
                clone.GetComponent<GroundItem>().describe = (item as ItemDefinition).describe;
                if ((item as ItemDefinition).code != "")
                    clone.GetComponent<GroundItem>().code = (item as ItemDefinition).code;

                clone.name = (item as ItemDefinition).itemObject.name + " " + clone.GetComponent<GroundItem>().durability + " " + clone.GetComponent<GroundItem>().area;

                int index = ActionController.lookedItem.GetComponent<GroundBag>().itemsCode.FindIndex(a => a == (item as ItemDefinition).code);
                ActionController.lookedItem.GetComponent<GroundBag>().itemList.RemoveAt(index);
                ActionController.lookedItem.GetComponent<GroundBag>().itemsCode.RemoveAt(index);
                ActionController.lookedItem.GetComponent<GroundBag>().itemsPosition.RemoveAt(index);

                Debug.Log((item as ItemDefinition).itemObject.name + ", 내구도: " + clone.GetComponent<GroundItem>().durability + ", 지역: " + clone.GetComponent<GroundItem>().area);
            };

            inventory.onItemDroppedFailed += (item) => {
                Debug.Log($"You're not allowed to drop {(item as ItemDefinition).Name} on the ground");
            };

            // Log when an item was unable to be placed on the ground (due to its canDrop    being set to false)
            inventory.onItemAddedFailed += (item) => {
                Debug.Log($"You can't put {(item as ItemDefinition).Name} there!");
            };

            inventory.onItemRemoved += (item) =>
            {
                GroundBag _viewBag = ActionController.lookedItem.GetComponent<GroundBag>();
                object[] data = { _viewBag.viewId, (item as ItemDefinition).code};
                evc.SendRaiseEvent(EventController.EVENTCODE.BOXITEMREMOVE, data, EventController.SEND_OPTION.OTHER);

            };

            inventory.onItemAdded += (item) => {
                if (ActionController.lookedItem.GetComponent<GroundBag>().itemList.Count == 0)
                {
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().durability = (item as ItemDefinition).durability;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().area = (item as ItemDefinition).area;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().code = (item as ItemDefinition).code;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().describe = (item as ItemDefinition).describe;
                    ActionController.lookedItem.GetComponent<GroundBag>().itemList.Add((item as ItemDefinition).itemObject);
                    ActionController.lookedItem.GetComponent<GroundBag>().itemsCode.Add((item as ItemDefinition).code);
                    ActionController.lookedItem.GetComponent<GroundBag>().itemsPosition.Add((item as ItemDefinition).position);
                }
                else
                {
                    for (int i = ActionController.lookedItem.GetComponent<GroundBag>().itemList.Count - 1; i >= 0; i--)
                    {
                        if (ActionController.lookedItem.GetComponent<GroundBag>().itemsCode[i] == (item as ItemDefinition).code)
                        {
                            ActionController.lookedItem.GetComponent<GroundBag>().itemsPosition[i] = (item as ItemDefinition).position;
                            Debug.Log(ActionController.lookedItem.GetComponent<GroundBag>().itemsCode[i] + " ||| " + (item as ItemDefinition).code + "겹침");
                            return;
                        }
                    }
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().durability = (item as ItemDefinition).durability;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().area = (item as ItemDefinition).area;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().code = (item as ItemDefinition).code;
                    (item as ItemDefinition).itemObject.GetComponent<GroundItem>().describe = (item as ItemDefinition).describe;
                    ActionController.lookedItem.GetComponent<GroundBag>().itemList.Add((item as ItemDefinition).itemObject);
                    ActionController.lookedItem.GetComponent<GroundBag>().itemsCode.Add((item as ItemDefinition).code);
                    ActionController.lookedItem.GetComponent<GroundBag>().itemsPosition.Add((item as ItemDefinition).position);
                }
                GroundBag _viewBag = ActionController.lookedItem.GetComponent<GroundBag>();
                object[] data = { _viewBag.viewId, (item as ItemDefinition).itemPrefabName, (item as ItemDefinition).code, (item as ItemDefinition).position.x, (item as ItemDefinition).position.y };
                evc.SendRaiseEvent(EventController.EVENTCODE.BOXITEMADD, data, EventController.SEND_OPTION.OTHER);

                Debug.Log((item as ItemDefinition).code + "코드 아이템 넣음");
            };
        }

        void Update()
        {

        }
        static public void ShowWatchingBag()
        {
            var WatchingItem = ActionController.lookedItem.GetComponent<GroundBag>();

            for (int i = 0; i < WatchingItem.itemList.Count; i++)
            {
                if (WatchingItem.watched == false)
                {
                    WatchingItem.itemList[i].GetComponent<GroundItem>().itemDef.code = WatchingItem.itemsCode[i];
                    WatchingItem.itemList[i].GetComponent<GroundItem>().itemDef.position = WatchingItem.itemsPosition[i];
                    if (inventory.TryAddAt(WatchingItem.itemList[i].GetComponent<GroundItem>().itemDef.CreateInstance(), WatchingItem.itemsPosition[i]) == false)
                    {
                        inventory.TryAdd(WatchingItem.itemList[i].GetComponent<GroundItem>().itemDef.CreateInstance());
                    }
                    inventory.TryAddAt(WatchingItem.itemList[i].GetComponent<GroundItem>().itemDef.CreateInstance(), WatchingItem.itemsPosition[i]);
                }
            }
            dropZone = ActionController.lookedItem.transform.GetChild(0).gameObject;
            ActionController.lookedItem.GetComponent<GroundBag>().watched = true;
        }

        static public void OtherInventoryClear()
        {
            inventory.Clear();
        }
        public bool SyncAddItem(int _viewid, string _itemDataName, string _code, int _x, int _y)
        {

            foreach(GroundBag _bag in Boxs)
            {
                if (_bag.viewId == _viewid)
                {
                    GameObject _spawnObj = (GameObject)Resources.Load("Prefab/" + _itemDataName);
                    _bag.itemList.Add(_spawnObj);
                    _bag.itemsCode.Add(_code);
                    _bag.itemsPosition.Add(new Vector2Int(_x, _y));
                }
            }
            return true;
        }
        public bool SyncRemoveItem(int _viewid, string _code)
        {
            GroundBag _searchBag = new GroundBag();
            foreach (GroundBag _bag in Boxs)
            {

                if (_bag.viewId == _viewid)
                {
                    _searchBag = _bag;
                    break;
                }
            }
            if (_searchBag.viewId == -999) return false;
            int i = 0;
            foreach(string itemCode in _searchBag.itemsCode)
            {
                if (itemCode == _code)
                {
                    _searchBag.itemsCode.RemoveAt(i);
                    _searchBag.itemList.RemoveAt(i);
                    _searchBag.itemsPosition.RemoveAt(i);
                    return true;
                }
                i++;

            }
            return false;
        }



        static public bool fastAddItem(IInventoryItem item)
        {
            if (inventory.CanAdd((item as ItemDefinition).CreateInstance()) == false) return false;
            IInventoryItem _addTemp = (item as ItemDefinition).CreateInstance();
            inventory.TryAdd(_addTemp);
            return true;
        }
        static public void fastRemoveItem(IInventoryItem item)
        {
            inventory.TryRemove(item);
        }
    }
}