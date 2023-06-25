using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HandsAttack : MonoBehaviourPunCallbacks
{
    //PhotonView PV;

    private PlayerManager PMgr;

    private float Damege;

    private bool isPossibleHandAttack;

    private bool isStayTrigger;

    // Start is called before the first frame update
    void Start()
    {
        //PV = GetComponent<PhotonView>();
        PMgr = GetComponentInParent<PlayerManager>();
        Damege = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandAttack();
    }

    public bool emptyCheck()
    {
        int numOfChild = this.transform.childCount;
        for (int i = 0; i < numOfChild; i++)
        {
            if (transform.GetChild(i).gameObject.active == true) return false;
        }
        return true;
    }

    private void HandAttack()
    {
        if (emptyCheck())
        {
            isPossibleHandAttack = true;
        }
        else
        {
            isPossibleHandAttack = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PMgr.isGameIn) return;
        if (PMgr.isAttacking && isPossibleHandAttack)
        {
            Debug.Log("맨손 어택");
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                GameObject _otherObj = other.gameObject;

                //_otherObj.SendMessage("TakeDamege", Damege);
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        isStayTrigger = true;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        isStayTrigger = false;
    //    }
    //}
}