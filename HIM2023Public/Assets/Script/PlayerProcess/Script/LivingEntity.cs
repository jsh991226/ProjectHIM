using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float Health;

    public bool isDead;

    private bool isFirst;

    private PlayerManager PMgr;

    private CameraController PCam;

    public GameObject fpCamera;

    private CameraAnimCtrl cac;

    private PlayerSoundManager psm;

    public bool isCinema;

    public bool isGod;

    public PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        Health = 100;
        isDead = false;
        isFirst = false;
        PMgr = this.gameObject.GetComponent<PlayerManager>();
        PCam = this.gameObject.GetComponent<CameraController>();
        cac = fpCamera.GetComponent<CameraAnimCtrl>();
        psm = GetComponent<PlayerSoundManager>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        if (isDead && !isFirst)
        {
            PMgr.SendMessage("PlayerDie");
            isFirst = true;
        }
    }

    public void TakeDamege(float Damege, Vector3 _location, PlayerManager pmgr)
    {
        if (isCinema) return;
        if (isGod) return;

        if (!isDead)
        {
            if (Damege <= (Health - Damege))
            {
                PV.RPC("hitSoundPlay", RpcTarget.All);
            }
            else
            {
                if (pmgr.PV.IsMine)
                {
                    pmgr.addKill();
                    if ((int)pmgr.qm.nowQuest == 1 )pmgr.qm.addCount(1);
                }
                if (PV.IsMine)
                {
                    PMgr.addDeath();
                }

                PV.RPC("deathSoundPlay", RpcTarget.All);
            }
            Debug.Log("대미지 받음");
            PMgr.ShowBlood(_location);
            Health -= Damege;
            if (PV.IsMine) cac.AttackedCameraMove(Damege);

        }
    }

    public void TakeDamege(float Damege)
    {
        if (!isDead)
        {
            PMgr.bloods.ShowBlood();
            Health -= Damege;
        }
    }

    [PunRPC]
    public void hitSoundPlay()
    {
        int num = Random.Range(1, 5);
        string soundName = "Hit 0" + num;
        Debug.Log(soundName);
        psm.PlaySound3D(soundName);
    }

    [PunRPC]
    public void deathSoundPlay()
    {
        Debug.Log("사망 사운드");
        psm.PlaySound3D("Death 01");
    }
}
