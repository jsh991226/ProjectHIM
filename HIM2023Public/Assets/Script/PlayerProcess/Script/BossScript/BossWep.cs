using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWep : MonoBehaviour
{
    private BossMoveCtrl bossMove;

    private bool isAttacking;

    public float Damege;


    private GameObject PlayerObj;
    private PlayerManager PMgr;
    private LivingEntity PLiving;

    private bool stayTrigger;

    // Start is called before the first frame update
    void Start()
    {
        bossMove = GetComponentInParent<BossMoveCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        isAttacking = bossMove.isAttacking;
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

                if (!PLiving.isDead)
                {

                    Debug.Log("¸Þ¼¼Áö º¸³¿");
                    PMgr.SendMessage("TakeDamegeAnim");
                    //PLiving.SendMessage("TakeDamege", Damege);
                }
            }
        }
    }
}
