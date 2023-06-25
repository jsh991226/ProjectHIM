using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 2.0f;

    private Animator anim;

    private bool inArea;
    private bool isHit;

    public bool isDead;

    private Transform tr;

    private Transform otherTr;

    private GameObject OtherObj;

    private Rigidbody rigd;
    
    public float Health;

    public bool isEnemyAttacking;

    private bool bFirst;

    private bool DoIdle;

    private bool isBump;

    private bool isChaseEnd;
    private bool isFounding;

    // Start is called before the first frame update
    void Start()
    {
        tr = this.gameObject.transform;
        anim = GetComponent<Animator>();
        Health = 100f;
        isDead = false;

        rigd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAnim();
        EnemyStatus();
    }

    private void EnemyStatus()
    {
        if(Health <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }
    }

    private void EnemyAnim()
    {
        if (!isDead)
        {
            if (inArea && !isHit)
            {
                tr.LookAt(otherTr);
                //anim.SetBool("isChase", true);
            }
            else
            {
                anim.SetBool("isChase", false);
            }

            if (anim.GetBool("isChase") && !isHit)
            {
                Vector3 dir = tr.TransformDirection(Vector3.forward);

                tr.position += dir * speed * Time.deltaTime;
            }

            if (isHit)
            {
                anim.SetTrigger("Attack");
                
            }

            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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
            DieEnemy();
        }
    }

    private void DieEnemy()
    {
        GetComponent<BoxCollider>().enabled = false;
        if (!bFirst)
        {
            anim.SetTrigger("Die");
        }
        bFirst = true;
        rigd.isKinematic = true;
    }
    
    private void TakeDamege(int Damege)
    {
        Debug.Log("enemy: " + Health);
        Health -= Damege;
        anim.SetTrigger("Damege");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("ºÎµúÈû!");
            isBump = true;
            isHit = true;
            rigd.isKinematic = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("ÀÌÁ¦ ÆÒ´Ù");
            isHit = true;
            //rigd.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("¸Â´Ù ¸»°í ¾îµð,,");
            isHit = false;
            rigd.isKinematic = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("HitZone");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Debug.Log("Àû °¨Áö!!!");
            OtherObj = other.gameObject;
            otherTr = OtherObj.transform;
            inArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("¾îµð°¬Áö??");
            inArea = false;
            isChaseEnd = true;
        }
    }
}
