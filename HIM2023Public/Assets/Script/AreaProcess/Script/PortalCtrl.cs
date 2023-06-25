using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PortalCtrl : MonoBehaviourPunCallbacks
{
    public InGameManager inGm;
    public GetSpawnPos parCtrl;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            inGm.UsePortal(parCtrl.index);
            other.transform.gameObject.GetComponent<PlayerManager>().completeExit = true;
            other.transform.gameObject.GetComponent<PlayerManager>().loadingPnl.GUIToggle(true);
            PhotonNetwork.LeaveRoom();
        }
    }
}
