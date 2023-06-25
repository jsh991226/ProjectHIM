using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhotonCtrl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PhotonView PV;



    public AreaCtrl areaCtrl;
    public NotifyCtrl notify;
    public AreaStatusCtrl areaStatus;

    public string areaCtrl_NewPlayer;
    public int areaCtrl_NewAreaId;











}
