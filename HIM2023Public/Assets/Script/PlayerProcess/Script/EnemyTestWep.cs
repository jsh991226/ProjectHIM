using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestWep : MonoBehaviour
{
    private GameObject PlayerObj;
    private PlayerManager PMgr;
    private LivingEntity PLiving;

    private EnemyTest enemyT;

    private bool isAttacking;
    private bool stayTrigger;

    public float Damege = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        enemyT = GetComponentInParent<EnemyTest>();
    }

    // Update is called once per frame
    void Update()
    {
        isAttacking = enemyT.isEnemyAttacking;
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
                    PLiving.SendMessage("TakeDamege", Damege);
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
}
