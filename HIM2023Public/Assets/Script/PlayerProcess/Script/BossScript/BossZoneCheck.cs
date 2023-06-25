using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossZoneCheck : MonoBehaviour
{
    public bool isBossZoneIn;
    public Vector3 PlayerPos;

    public Transform PlayerTr;
    private GameObject Player;

    public bool isStartMotion;

    private GameObject ForestBoss;
    private BossMoveCtrl bossMove;

    public int FirstBossZoneIn;

    // Start is called before the first frame update
    void Start()
    {
        ForestBoss = GameObject.Find("ForestBoss");
        bossMove = ForestBoss.GetComponent<BossMoveCtrl>();

        FirstBossZoneIn = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStartMotion = true;
            Debug.Log("잘왔어요");
            FirstBossZoneIn++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isStartMotion)
        {
            if (other.CompareTag("Player"))
            {
                isBossZoneIn = true;
                Player = other.gameObject;
                PlayerTr = Player.transform;
                //PlayerPos = Player.transform.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isStartMotion)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("떠남");
                isBossZoneIn = false;
                Player = null;
                isStartMotion = false;
                bossMove.outArea();
            }
        }
    }
}
