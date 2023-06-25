using FarrokhGames.Inventory.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;


public class GroundBag : MonoBehaviour
{

    public List<GameObject> itemList;
    public PhotonView pv;
    public int viewId = -999;

    public List<string> itemsCode;
    public List<Vector2Int> itemsPosition;

    [HideInInspector]
    public bool watched = false;


    private EventController evc;

    void Start()
    {
        evc = GameObject.Find("EventController").GetComponent<EventController>();
        viewId = pv.ViewID;

        for (int i = 0; i < itemList.Count; i++)
        {
            itemsCode.Add(viewId + "_groundbag_"+i);
            itemList[i].GetComponent<GroundItem>().code = itemsCode[i];
            itemsPosition.Add(new Vector2Int(0, 0));
        }
    }
}

/*[CustomEditor(typeof(GroundBag))]
public class GroundBagInspector : Editor
{
    GroundBag groundBag;

    public List<GameObject> itemList;
    public List<int> itemsDura;
    public List<string> itemsArea;

    private GameObject newItem;
    private int newItemDura;
    private string newItemArea;

    void OnEnable()
    {
        groundBag = target as GroundBag;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        newItem = EditorGUILayout.ObjectField("New Item", newItem, typeof(GameObject), true) as GameObject;

        if (newItem == null) return;

        if (newItem.GetComponent<GroundItem>().itemDef.Type == ItemType.Weapons)
        {
            newItemDura = EditorGUILayout.IntField("New Item Durability", newItemDura);
            newItemArea = EditorGUILayout.TextField("New Item Area", newItemArea);
        }
        else
        {
            newItemDura = 0;
            newItemArea = "";
        }

        if (GUILayout.Button("Add Item"))
        {
            groundBag.itemList.Add(newItem);
            groundBag.itemsDura.Add(newItemDura);
            groundBag.itemsArea.Add(newItemArea);
        }
    }
}*/