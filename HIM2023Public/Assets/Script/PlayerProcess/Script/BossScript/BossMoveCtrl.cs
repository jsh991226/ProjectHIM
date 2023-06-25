using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class BossMoveCtrl : MonoBehaviourPunCallbacks
{
    public float HP;

    private Animator animator;
    private Rigidbody rigd;

    private bool isFoundPlayer;

    private bool inPlayer;

    private Transform tr;
    private Transform PlayerTr;
    public Transform BackTr;

    public BossZoneCheck bossZone;
    public CameraChange cameraChange;

    private Vector3 PlayerPos;

    private bool StartTriggerSet;

    private bool bFirst;

    public bool isAttacking;

    public bool isDead;
    private bool isDeadFirst;

    LayerMask PlayerLayer;

    private GameObject OtherObj;
    public GameObject SkillZone;

    public GameObject MagicArea;
    public GameObject SwordEnergy;
    public ParticleSystem GroundHit;


    private int LoopCnt;
    public bool LoopPatton;
    private bool BossJumpSlow;
    public InGameManager inGm;


    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        animator = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        rigd = GetComponent<Rigidbody>();

        StartTriggerSet = false;
        isDeadFirst = false;
        LoopPatton = false;

        animator.SetFloat("JumpSlow", 1.4f);

        LoopCnt = 0;

        MagicArea.SetActive(false);
        GroundHit.Stop();

        PlayerLayer = LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void Update()
    {
        inPlayer = bossZone.isBossZoneIn;
        PlayerTr = bossZone.PlayerTr;
        StartTriggerSet = bossZone.isStartMotion;
        bFirst = false;
        BossJumpSlow = cameraChange.JumpSlow;

        if (HP <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        if (!isDead)
        {
            inArea();
        }
        else
        {
            BossDie();
        }
    }

    private void inArea()
    {
        if (!isDead)
        {
            if (inPlayer)
            {
                animator.SetTrigger("WakeUp");
                if (LoopPatton) tr.LookAt(PlayerTr);

                if (BossJumpSlow)
                {
                    animator.SetFloat("JumpSlow", 0.14f);
                } else
                {
                    animator.SetFloat("JumpSlow", 1.4f);
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("FirstAttack") || animator.GetCurrentAnimatorStateInfo(0).IsName("TwoAttack") ||
                        animator.GetCurrentAnimatorStateInfo(0).IsName("JumpSlash"))
                    {
                        isAttacking = true;
                    }

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                    {
                        this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    }
                }
                else
                {
                    isAttacking = false;
                    this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                }


                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f &&
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.41f/* &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f*/)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                    {
                        GroundHit.Play();
                        if (LoopPatton)
                        {
                            BossSkillRange();
                        }
                    }
                }
                else
                {
                    GroundHit.Stop();
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f
                && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                    {
                        if(LoopCnt > 0)
                        {
                            LoopPatton = true;
                        }
                        LoopCnt++;
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f &&
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("PrayEnergy"))
                    {
                        MagicArea.SetActive(true);
                    }

                }
                else
                {
                    MagicArea.SetActive(false);
                }
            }
        }
    }

    private void BossSkillRange()
    {
        int layerMask = (1 << PlayerLayer);
        Collider[] colliders = Physics.OverlapSphere(tr.position, 8.0f, layerMask);
        //Physics.OverlapSphere(tr.position, 5.0f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("Player"))
            {
                GameObject Players = colliders[i].gameObject;
                LivingEntity Pliv = Players.GetComponent<LivingEntity>();
                Pliv.TakeDamege(5.0f);
            }
        }
    }

    private void BossDie()
    {
        inGm.isBossDead = true;
        if (!isDeadFirst)
        {
            animator.SetTrigger("Die");
        }
        isDeadFirst = true;
    }

    public void TakeDamege(float Damege)
    {
        HP -= Damege;
        inGm.bossHP = HP;
        Debug.Log("Boss: " + HP);
    }

    public void outArea()
    {
        animator.SetTrigger("PlayerOut");
        tr.LookAt(BackTr);
        bFirst = true;
    }
}
