using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyKnight : MonoBehaviourPunCallbacks
{
    public float HP;

    private Transform tr;
    private Transform PlayerTr;

    private float speed = 2.5f;

    private bool isPlayerHere;
    private bool isHit;

    private PhotonView pv;

    private GameObject OtherObj;

    private Animator animator;

    public bool isEnemyAttacking;

    public bool isDead;

    private bool bFirst;

    private int Targeting;

    public Bloods bloods;


    private Rigidbody rigd;

    private float currHP;

    LayerMask PlayerLayer;
    LayerMask OtherLayer;

    private PlayerSoundManager psm;

    private int attack = 0;
    public ParticleSystem bloodEffect, hitEffect;


    public void SetLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            SetLayersRecursively(child, name);
        }
    }




    void Start()
    {
        if (GetComponent<PhotonView>() != null )
        {
            pv = GetComponent<PhotonView>();
        }

        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody>();
        psm = GetComponent<PlayerSoundManager>();
        SetLayersRecursively(transform, "LiveMob");
        PlayerLayer = LayerMask.NameToLayer("Player");
        OtherLayer = LayerMask.NameToLayer("OtherPlayer");

        isDead = false;
        bFirst = false;

        Targeting = 0;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyStatus();
        EnemyAnim();

        if (!isDead)
            if (Targeting < 2)
                Area();
    }

    private void EnemyStatus()
    {
        if (HP <= 0)
        {
            isDead = true;

            StartCoroutine(dying());
        }
        if (HP > 0)
            isDead = false;

        IEnumerator dying()
        {
            yield return new WaitForSeconds(2f);
            SetLayersRecursively(transform, "DeadMob");
            //MonsterSpawner.instance.InsertQueue(gameObject);
        }

    }

    private void OnEnable()
    {
        //isDead = false;
        isHit = false;
        isPlayerHere = false;
        HP = 100;
        Targeting = 0;
        bFirst = false;
    }

    private void EnemyAnim()
    {
        if (!isDead)
        {
            if (isHit)
            {
                animator.SetTrigger("Attack");
            }

            // isPlayerHere = enemyZone.isPlayerHere;
            if (isPlayerHere && !isHit)
            {
                animator.SetBool("isChase", true);
                EnemyMove();
            }
            else
            {
                animator.SetBool("isChase", false);
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    attack++;
                    isEnemyAttacking = true;
                    if (attack == 1)
                    {
                        psm.PlaySound3D("Wield 01");
                    }
                }
            }
            else
            {
                attack = 0;
                isEnemyAttacking = false;
            }
        }
        else
        {
            if (!bFirst)
            {
                animator.SetTrigger("Die");
            }
            bFirst = true;
        }
    }

    private void EnemyMove()
    {
        try
        {
            Vector3 Move = transform.forward * speed;
            tr.LookAt(PlayerTr.position);
            rigd.velocity = Move;
        }
        catch
        {

        }

    }

    [PunRPC]
    public void RPCTakeDamage(float Damege)
    {
        HP -= Damege;
    }

    public void TakeDamege(float Damege, Vector3 _location, PlayerManager pmgr)
    {
        int num = Random.Range(1, 5);
        psm.PlaySound3D("Hit 0" + num);


        if (Damege <= (HP - Damege))
        {
            //psm.PlaySound3D("Male Hit 02");
        }
        else
        {
            // psm.PlaySound3D("Male Hit 04");
            pmgr.RemoveMob(pv);
            if (pmgr.PV.IsMine)
            {
                pmgr.addKill();
                if ((int)pmgr.qm.nowQuest == 0 )pmgr.qm.addCount(1);
            }

        }
        ShowBlood(_location);
        HP -= Damege;
        animator.SetTrigger("Damege");


    }

    public void TakeDamege(float Damege)
    {
        HP -= Damege;
        animator.SetTrigger("Damege");
        bloods.ShowBlood();
        Debug.Log("Enemy: " + HP);
    }

    private void Area()
    {
        int layerMask = (1 << PlayerLayer) + (1 << OtherLayer);

        Collider[] colliders = Physics.OverlapSphere(tr.position, 7.0f, layerMask);
        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                Targeting++;
                OtherObj = colliders[0].gameObject;
                PlayerTr = OtherObj.transform;
                isPlayerHere = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHit = false;
        }
    }

    public void ShowBlood(Vector3 _position)
    {
        StartCoroutine(EffectStart(bloodEffect, _position));
    }

    public void ShowDamage(Vector3 _position)
    {
        StartCoroutine(EffectStart(hitEffect, _position));
    }

    private IEnumerator EffectStart(ParticleSystem _effect, Vector3 _position)
    {
        ParticleSystem newEffect = Instantiate(_effect);
        int num = Random.Range(0, 361);
        newEffect.GetComponent<Transform>().localEulerAngles = new Vector3(0, num, 0);
        newEffect.GetComponent<Transform>().position = _position;
        newEffect.Play();
        yield return new WaitForSeconds(1.0f);
        Destroy(newEffect.gameObject);
    }
}