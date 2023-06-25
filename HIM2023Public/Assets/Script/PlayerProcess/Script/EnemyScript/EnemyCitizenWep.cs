using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCitizenWep : MonoBehaviour
{
    private GameObject PlayerObj;
    private PlayerManager PMgr;
    private LivingEntity PLiving;

    private EnemyKnight enemyKnight;

    private bool isAttacking;
    private bool stayTrigger;

    public float Damege = 10.0f;

    private PlayerSoundManager psm;
    void Start()
    {
        enemyKnight = GetComponentInParent<EnemyKnight>();
        psm = GetComponent<PlayerSoundManager>();
    }

    void Update()
    {
        isAttacking = enemyKnight.isEnemyAttacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerObj = other.gameObject;
                PMgr = PlayerObj.GetComponent<PlayerManager>();
                PLiving = PlayerObj.GetComponent<LivingEntity>();
                if (!stayTrigger && !PLiving.isDead)
                {
                    PMgr.SendMessage("TakeDamegeAnim");
                    PLiving.TakeDamege(Damege, other.bounds.ClosestPoint(transform.position), PMgr);
                    PMgr.ShowDamage(other.bounds.ClosestPoint(transform.position));
                    WeaponAttack();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            stayTrigger = false;
        }
    }

    public void WeaponAttack()
    {
        int num = Random.Range(1, 5);
        string soundName = "Attack 0" + num;
        GameObject openSound = new GameObject("Weapon Attack Sound");
        openSound.transform.position = gameObject.transform.position;
        TemporarySoundPlayer soundPlayer = openSound.AddComponent<TemporarySoundPlayer>();
        soundPlayer.SoundPlay(psm.GetClip(soundName));
        /*Debug.Log(soundName);
        psm.PlaySound3D(soundName);*/
    }
}