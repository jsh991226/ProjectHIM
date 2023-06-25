using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;

public class SlotSwap : MonoBehaviour
{
    [SerializeField]
    private SmithCtrl ctrl;

    public IInventoryItem _myItem = null;



    public void MoveToMatSlot()
    {
        if (_myItem == null) return; 
        if (ctrl.MatSlot.Count < 3)
        {
            ctrl.InvSlot.Remove(_myItem);
            ctrl.MatSlot.Add(_myItem);
            ctrl.ReFreshGUI();
        }
    }
    public void MoveToInvSlot()
    {
        if (_myItem == null) return;
        ctrl.MatSlot.Remove(_myItem);
        ctrl.InvSlot.Add(_myItem);
        ctrl.ReFreshGUI();
    }





}
