using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Transactions;
using Unity.VisualScripting;

public class WeaponScript : MonoBehaviourPunCallbacks
{
    public float Damege;

    public float HandDamege;

    public float AttackSpeed;

    private PlayerManager Pmgr;

    public PhotonView PV;

    private bool stayTrigger;

    private bool Attacking;

    private bool EnemyStay;

    private float PlayerPower;

    private PlayerSoundManager psm;

    private int attack = 0;
    public enum effect
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponentInParent<PhotonView>();
        Pmgr = GetComponentInParent<PlayerManager>();
        psm = GetComponent<PlayerSoundManager>();

        Attacking = false;
        stayTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        Attacking = Pmgr.isAttacking;
        if (Attacking)
        {
            attack++;
            if (attack == 1)
            {
                Pmgr.WeaponWield();
            }
        }
        else
            attack = 0;
    }



    

    private void OnTriggerEnter(Collider other)
    {
        if (!Pmgr.isGameIn) return;
        if (Attacking)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameObject OtherPlayer = other.gameObject;
                PlayerManager otherPM = OtherPlayer.GetComponent<PlayerManager>();
                LivingEntity otherLiving = OtherPlayer.GetComponent<LivingEntity>();

                if (!otherLiving.isDead)
                {
                    otherPM.SendMessage("TakeDamegeAnim");
                    otherLiving.TakeDamege(Damege, other.bounds.ClosestPoint(transform.position), Pmgr);
                    otherPM.ShowDamage(other.bounds.ClosestPoint(transform.position));
                    WeaponAttack();
                }
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                GameObject Enemy = other.gameObject;
                EnemyKnight enemyKnight = Enemy.GetComponent<EnemyKnight>();

                if (!enemyKnight.isDead)
                {
                    enemyKnight.TakeDamege(Damege, other.bounds.ClosestPoint(transform.position), Pmgr);
                    enemyKnight.ShowDamage(other.bounds.ClosestPoint(transform.position));
                    WeaponAttack();
                }
            }

            if (other.gameObject.CompareTag("Boss"))
            {
                GameObject Boss = other.gameObject;
                BossMoveCtrl bossMove = Boss.GetComponent<BossMoveCtrl>();

                if (bossMove == null)
                {
                    ArenaBossCtrl ArenaMove = Boss.GetComponent<ArenaBossCtrl>();
                    ArenaMove.ShowDamage(other.bounds.ClosestPoint(transform.position));
                    ArenaMove.TakeDamage(Damege);
                }
                else
                {
                    bossMove.TakeDamege(Damege);
                }

                WeaponAttack();
            }
        }
    }

    public void WeaponAttack()
    {
        string soundName = "Attack 01";
        GameObject openSound = new GameObject("Weapon Attack Sound");
        openSound.transform.position = gameObject.transform.position;
        TemporarySoundPlayer soundPlayer = openSound.AddComponent<TemporarySoundPlayer>();
        soundPlayer.SoundPlay(psm.GetClip(soundName));
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            stayTrigger = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            stayTrigger = false;
        }
    }
}