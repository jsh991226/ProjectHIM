using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyWepScript : MonoBehaviour
{
    public float Damege;

    private bool stayTrigger;
    private bool PlayerStay;

    private bool isAttacking;

    private EnemyScript enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = this.gameObject.GetComponentInParent<EnemyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        isAttacking = enemyScript.isEnemyAttacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking)
        {
            if (other.tag == "Player")
            {
                GameObject OtherPlayer = other.gameObject;
                PlayerManager otherPM = OtherPlayer.GetComponent<PlayerManager>();
                LivingEntity otherLiving = OtherPlayer.GetComponent<LivingEntity>();

                if (!PlayerStay && !stayTrigger && !otherLiving.isDead)
                {
                    otherPM.SendMessage("TakeDamegeAnim");
                }
                if (!PlayerStay && !stayTrigger && !otherLiving.isDead)
                {
                    otherLiving.SendMessage("TakeDamege", Damege);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerStay = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStay = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            stayTrigger = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            stayTrigger = true;
        }
    }


}
