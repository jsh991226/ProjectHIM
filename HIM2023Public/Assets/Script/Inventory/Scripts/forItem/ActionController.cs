using FarrokhGames.Inventory.Examples;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;

    private RaycastHit hitInfo;

    [SerializeField]
    public Text actionText;

    static public GameObject lookedItem;


    int layerMask;
    void Awake()
    {
        layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        CheckItem();
        CheckInventroy();
    }

    private void CheckInventroy()
    {
        if (actionText.text == "Open 'Q'" && !actionText.IsActive())
        {
            lookedItem.GetComponent<GroundBag>().watched = false;
            OtherInventory.OtherInventoryClear();
        }   
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                //  Debug.Log("Take E");
                actionText.text = "Take 'E'";
                lookedItem = hitInfo.transform.gameObject;
                ItemInfoAppear();
            }
            else if (hitInfo.transform.tag == "Bag")
            {
                actionText.text = "Open 'Q'";
                lookedItem = hitInfo.transform.gameObject;
                ItemInfoAppear();
            }
            else ItemInfoDisappear();
        }
        else ItemInfoDisappear();
    }

    private void ItemInfoAppear()
    {
        actionText.gameObject.SetActive(true);
    }

    private void ItemInfoDisappear()
    {
        actionText.gameObject.SetActive(false);
    }
}