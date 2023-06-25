using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using FarrokhGames.Inventory.Examples;

using System;

public class EventController : MonoBehaviourPunCallbacks
{
    public enum EVENTCODE
    {
        JOINAREA,
        EXITAREA,
        READYPLAYER,
        MOVEAREA,
        RECIVECHAT,
        ECORECIVE,
        RECIVESPAWNPOINT,
        DESTROYITEM,
        CHANGEWEAPON,
        BOSSDEAD,
        USEPORTAL,
        CHANGEPORTAL,
        ALLPORTALCLOSE,
        BOXITEMADD,
        BOXITEMREMOVE,
        MOBREMOVE
    }

    public enum SEND_OPTION
    {
        OTHER,
        ALL,
        MASTER
    }


    [Header("Event Object")]
    [SerializeField]
    private AreaCtrl areaCtrl;
    [SerializeField]
    private ChatCtrl chatCtrl;
    [SerializeField]
    private EcoCtrl eco;
    [SerializeField]
    private InGameManager inGm;
    [SerializeField]
    private OtherInventory otherInv;

    public PlayerManager pm;

    public void OnEvent(EventData photonEvent)
    {
        // Do something
        EventReceived(photonEvent);
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void SendRaiseEvent(EVENTCODE eventcode, object[] datas, SEND_OPTION sendoption = SEND_OPTION.OTHER)
    {
        string DebugStr = string.Empty;
        DebugStr = "[SEND__" + eventcode.ToString() + "]";
        for (int i = 0; i < datas.Length; ++i)
        {
            DebugStr += "_" + datas[i];
        }
        //Debug.LogError(DebugStr);
        RaiseEventOptions raiseEventOption = new RaiseEventOptions
        {
            Receivers = (ReceiverGroup)sendoption,
        };
        PhotonNetwork.RaiseEvent((byte)eventcode, datas, raiseEventOption, SendOptions.SendReliable);
    }





    private void EventReceived(EventData photonEvent)
    {
        int code = photonEvent.Code;
        if (code == (int)EVENTCODE.JOINAREA)
        {
            object[] datas = (object[])photonEvent.CustomData;
            Debug.LogError("참가 이벤트 수신" + datas[0] + "_" + datas[1]);
            areaCtrl.SyncJoinArea((int)datas[0], (string)datas[1]);
        }
        else if (code == (int)EVENTCODE.EXITAREA)
        {
            object[] datas = (object[])photonEvent.CustomData;
            areaCtrl.SyncExitArea((int)datas[0], (string)datas[1], (string)datas[2]);
        }
        else if (code == (int)EVENTCODE.READYPLAYER)
        {
            object[] datas = (object[])photonEvent.CustomData;
            areaCtrl.SyncReadyPlayer((int)datas[0], (string)datas[1], (bool)datas[2]);
        }
        else if (code == (int)EVENTCODE.MOVEAREA)
        {
            object[] datas = (object[])photonEvent.CustomData;
            areaCtrl.SyncMoveArea((int)datas[0], (string)datas[1]);
        }
        else if (code == (int)EVENTCODE.RECIVECHAT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            chatCtrl.SyncSendChat((string)datas[0], (string)datas[1]);
        }
        else if (code == (int)EVENTCODE.ECORECIVE)
        {
            object[] datas = (object[])photonEvent.CustomData;
            eco.SyncEcoRecive((string)datas[0], (string)datas[1], (int)datas[2]);
        }
        else if (code == (int)EVENTCODE.RECIVESPAWNPOINT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            inGm.SyncRecivedSpawnPoint((string)datas[0], (int)datas[1]);
        }
        else if (code == (int)EVENTCODE.DESTROYITEM)
        {
            object[] datas = (object[])photonEvent.CustomData;
            pm.SyncDestroyItem((string)datas[0], (int)datas[1]);
        }
        else if (code == (int)EVENTCODE.CHANGEWEAPON)
        {
            object[] datas = (object[])photonEvent.CustomData;
            pm.SyncChangeItem((string)datas[0], (string)datas[1], (bool)datas[2], (float)datas[3]);
        }
        else if (code == (int)EVENTCODE.BOSSDEAD)
        {
            object[] datas = (object[])photonEvent.CustomData;
            inGm.SyncBossDead((int[])datas[0]);
        }
        else if (code == (int)EVENTCODE.USEPORTAL)
        {
            object[] datas = (object[])photonEvent.CustomData;
            inGm.SyncUsePortal((string)datas[0], (int)datas[1], (int)datas[2]);
        }
        else if (code == (int)EVENTCODE.CHANGEPORTAL)
        {
            object[] datas = (object[])photonEvent.CustomData;
            inGm.SyncChangePortal((int)datas[0]);
        }
        else if (code == (int)EVENTCODE.ALLPORTALCLOSE)
        {
            object[] datas = (object[])photonEvent.CustomData;
            inGm.SyncAllPortalClose();
        }
        else if (code == (int)EVENTCODE.BOXITEMADD)
        {
            object[] datas = (object[])photonEvent.CustomData;
            otherInv.SyncAddItem((int)datas[0], (string)datas[1], (string)datas[2], (int)datas[3], (int)datas[4]);
        }
        else if (code == (int)EVENTCODE.BOXITEMREMOVE)
        {
            object[] datas = (object[])photonEvent.CustomData;
            otherInv.SyncRemoveItem((int)datas[0], (string)datas[1]);
        }
        else if (code == (int)EVENTCODE.MOBREMOVE)
        {
            object[] datas = (object[])photonEvent.CustomData;
            pm.SyncRemoveMob((int)datas[0]);
        }



    }

}
