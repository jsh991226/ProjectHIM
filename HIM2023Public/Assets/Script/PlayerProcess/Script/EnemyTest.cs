using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemyTest : MonoBehaviour
{
    public float HP;

    private Transform tr;
    private Transform PlayerTr;

    private CharacterController cc;

    private float speed = 0.3f;

    private bool isPlayerHere;
    private bool isHit;

    private GameObject OtherObj;

    private Animator animator;

    public bool isEnemyAttacking;

    public bool isDead;

    private bool bFirst;

    private int Targeting;

    LayerMask PlayerLayer;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PlayerLayer = LayerMask.NameToLayer("Player");

        isDead = false;
        bFirst = false;

        Targeting = 0;

        HP = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            EnemyStatus();
            EnemyAnim();

            if (Targeting < 2)
            {
                Area();
            }
        }
    }

    private void EnemyStatus()
    {
        if (HP <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    private void EnemyVision()
    {
        // if(Physics.Raycast)
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
                    isEnemyAttacking = true;
                }
            }
            else
            {
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
        Vector3 dir = tr.TransformDirection(Vector3.forward);
        tr.LookAt(PlayerTr.position);
        cc.SimpleMove(dir * speed);
    }

    public void TakeDamege(float Damege)
    {
        HP -= Damege;
        animator.SetTrigger("Damege");
        Debug.Log("Enemy: " + HP);
    }

    private void Area()
    {
        int layerMask = (1 << PlayerLayer);
        Collider[] colliders = Physics.OverlapSphere(tr.position, 10.0f, layerMask);
        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                Targeting++;
                //Debug.Log("�÷��̾� ����!");
                OtherObj = colliders[0].gameObject;
                PlayerTr = OtherObj.transform;
                //Debug.Log(colliders[0]);
                isPlayerHere = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(other.gameObject.name);
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
}
