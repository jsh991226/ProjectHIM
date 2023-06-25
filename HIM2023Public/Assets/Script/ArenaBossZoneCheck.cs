using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBossZoneCheck : MonoBehaviour
{
    public bool isPlayerCheck;

    public Vector3 PlayerTr;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerCheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("¿Ô´Ù,,");
            isPlayerCheck = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log(PlayerTr);
            isPlayerCheck = true;
            PlayerTr = other.gameObject.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerCheck = false;
        }
    }
}
