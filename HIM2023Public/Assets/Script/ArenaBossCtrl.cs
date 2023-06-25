using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ArenaBossCtrl : MonoBehaviour
{
    private Animator anim;

    private bool isArenaShout;

    private bool isPlayerHere;

    private bool isEndMotion;

    private bool isDeadFirst;

    public bool isDead;

    private int ArrowSkillCnt;

    private int RandomPoseInt;

    private float shoutTimer;
    public float RunTimer;

    private float PoseTimer;

    private float SkillTimer;

    private Rigidbody rb;

    private Vector3 PlayerTr;
    private Vector3 ArrowSkillTr;

    private float Damage = 20f;

    public float HP = 100;

    private bool isAttacking;

    [SerializeField]
    private GameObject ArrowSkill;

    [SerializeField]
    private CinemaCtrl cinema;

    [SerializeField]
    private AudioSource BossShout;

    [SerializeField]
    private ArenaBossZoneCheck ArenaZone;

    [SerializeField]
    private InGameManager inGm;

    public ParticleSystem hitEffect;

    private PlayerSoundManager psm;

    //public int walk = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        psm = GetComponent<PlayerSoundManager>();
        shoutTimer = 0f;
        RunTimer = 0f;
        SkillTimer = 0f;
        PoseTimer = 0f;
        isDead = false;
        isAttacking = false;
        isEndMotion = false;
        isDeadFirst = false;
        ArrowSkillCnt = 0;
        RandomPoseInt = 0;
    }



    // Update is called once per frame
    void Update()
    {
        BossSound();
        isArenaShout = cinema.isBossShout;
        isPlayerHere = ArenaZone.isPlayerCheck;
        PlayerTr = ArenaZone.PlayerTr;

        if (HP <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        if (isDead)
        {
            anim.SetInteger("RandomPoseInt", 0);
            BossDie();
        }
        else
            ArenaBossPatton();

        if (!isPlayerHere)
            PlayerOut();
        else
            isEndMotion = false;


    }



    private void FixedUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
            {
                rb.AddRelativeForce(Vector3.up * 10f * Time.deltaTime, ForceMode.Force);
                rb.AddRelativeForce(Vector3.forward * 500f * Time.deltaTime, ForceMode.VelocityChange);
            }
        }
    }

    Coroutine isMove;
    private int stopping, shouting, kicking, sliding, jumping, ready, attention, shoot = 0;

    IEnumerator Moving()
    {
        while (true)
        {
            psm.PlaySound3D("Walking");

            yield return new WaitForSecondsRealtime(0.18f);
        }
    }

    IEnumerator delaySoundPlay(float _delay, string _soundName)
    {
        yield return new WaitForSecondsRealtime(_delay);
        psm.PlaySound3D(_soundName);
    }
    void BossSound()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            if (isMove == null)
            {
                isMove = StartCoroutine(Moving());
            }
        }
        else
        {
            if (isMove != null)
            {
                StopCoroutine(isMove);
                isMove = null;
            }
        }


        if (anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose1") || anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose2") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose3") || anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose4") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose5") || anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose6") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose7") || anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose8") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose9"))
        {
            stopping++;
            if (stopping == 1)
            {
                psm.PlaySound3D("Breath");
            }
        }
        else
            stopping = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Shouting") || anim.GetCurrentAnimatorStateInfo(0).IsName("Shouting 0"))
        {
            shouting++;
            if (shouting == 1)
            {
                int num = Random.Range(1, 4);
                string soundName = "Shout 0" + num;
                psm.PlaySound3D(soundName);
            }
        }
        else
            shouting = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep") || anim.GetCurrentAnimatorStateInfo(0).IsName("Armada"))
        {
            kicking++;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep") && kicking == 1)
            {
                Debug.Log("Leg_Sweep");
                StartCoroutine(delaySoundPlay(0.36f, "Kick 01"));
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Armada") && kicking == 1)
            {
                Debug.Log("Armada");
                StartCoroutine(delaySoundPlay(0.51f, "Kick 02"));
            }
        }
        else
            kicking = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            sliding++;
            if (sliding == 1)
                StartCoroutine(delaySoundPlay(0.09f, "Slide"));
        }
        else
            sliding = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
        {
            jumping++;
            if (jumping == 1)
            {
                StartCoroutine(delaySoundPlay(0.15f, "Jumping"));
                StartCoroutine(delaySoundPlay(0.895f, "Falling"));
                StartCoroutine(delaySoundPlay(0.91f, "Dust"));
            }
        }
        else
            jumping = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("DrwaArrow"))
        {
            ready++;
            if (ready == 1)
            {
                StartCoroutine(delaySoundPlay(0.2f, "Ready"));
            }
        }
        else
            ready = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleArrow"))
        {
            attention++;
            if (attention == 1)
            {
                psm.PlaySound3D("Attention");
            }
        }
        else
            attention = 0;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ShotArrow"))
        {
            shoot++;
            if (shoot == 1)
            {
                psm.PlaySound3D("Shoot");
                StartCoroutine(delaySoundPlay(0.1f, "ArrowSkill"));
            }
        }
        else
            shoot = 0;
    }

    void ArenaBossPatton()
    {
        if (isPlayerHere)
        {
            if (isArenaShout)
            {
                anim.ResetTrigger("PlayerOut");
                anim.SetTrigger("Shouting");


                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Shouting"))
                    {
                        shoutTimer += Time.deltaTime;
                    }

                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep")
                        || anim.GetCurrentAnimatorStateInfo(0).IsName("Slide") || anim.GetCurrentAnimatorStateInfo(0).IsName("Armada"))
                    {
                        isAttacking = true;
                    }
                }
                else
                {
                    isAttacking = false;
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run") || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                {
                    this.gameObject.transform.LookAt(PlayerTr);
                    RunTimer += Time.deltaTime;
                    anim.SetFloat("RunTimer", RunTimer);
                    PoseTimer = 0;
                    anim.SetFloat("PoseTimer", 0);
                }


                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("ShotArrow"))
                    {
                        if (ArrowSkillCnt <= 0)
                        {
                            Instantiate(ArrowSkill, ArrowSkillTr, Quaternion.identity);
                            ArrowSkillCnt++;
                        }
                    }
                }

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep") || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack") ||
                        anim.GetCurrentAnimatorStateInfo(0).IsName("Shouting") || anim.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
                    {
                        RandomPoseInt = Random.Range(1, 10);
                        Debug.Log(RandomPoseInt);
                        anim.SetInteger("RandomPoseInt", RandomPoseInt);
                        RunTimer = 0;
                        anim.SetFloat("RunTimer", 0);
                    }
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("StopPose" + RandomPoseInt))
                {
                    PoseTimer += Time.deltaTime;
                    anim.SetFloat("PoseTimer", PoseTimer);
                }

                if (anim.GetFloat("PoseTimer") >= 3.89f)
                {
                    anim.SetInteger("RandomPoseInt", 0);
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Shouting"))
                    anim.SetInteger("Patton", 1);

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                    anim.SetInteger("Patton", 2);

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep"))
                    anim.SetInteger("Patton", 3);

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
                    anim.SetInteger("Patton", 4);


                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Leg_Sweep") || anim.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
                    {
                        Debug.Log("ÃÊ±âÈ­");

                        ArrowSkillCnt = 0;
                    }
                }

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleArrow"))
                        ArrowSkillTr = new Vector3(PlayerTr.x, PlayerTr.y - 0.5f, PlayerTr.z);
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isAttacking)
            {
                LivingEntity _living = other.gameObject.GetComponent<LivingEntity>();
                _living.TakeDamege(Damage);
            }
        }
    }

    public void TakeDamage(float Damage)
    {
        int num = Random.Range(1, 7);
        string soundName = "Hit 0" + num;
        psm.PlaySound3D(soundName);
        HP -= Damage;
    }

    private void PlayerOut()
    {
        if (!isEndMotion)
        {
            RandomPoseInt = Random.Range(1, 10);
            anim.SetInteger("RandomPoseInt", RandomPoseInt);
            anim.SetTrigger("PlayerOut");
        }
        isEndMotion = true;
    }

    private void BossDie()
    {
        inGm.isBossDead = true;
        if (!isDeadFirst)
        {
            anim.SetTrigger("Die");
        }
        isDeadFirst = true;
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