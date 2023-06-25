 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRollManager : MonoBehaviour
{
    [Header("GameObject of CardEntitys")]
    public List<GameObject> cardEntity;


    private bool isRolling;






    void Start()
    {
        for ( int i = 0; i < cardEntity.Count; i++)
        {
            foreach (GameObject entity in cardEntity) entity.GetComponent<CardEntity>().cardPos.Add(cardEntity[i].GetComponent<CardEntity>().myPos);
            cardEntity[i].GetComponent<CardEntity>().layerNum = i;
            cardEntity[i].GetComponent<CardEntity>().maxLayer = cardEntity.Count-1;
        }

    }

    void Update()
    {
        if (isRolling)
        {
            foreach ( GameObject entity in cardEntity)if(entity.GetComponent<CardEntity>().isMove) return;
            isRolling = false;
        }

    }

    public void nextAction()
    {
        if (isRolling) return; moveCall(+1);
    }

    public void prevAction()
    {
        if (isRolling) return;moveCall(-1);
    }
    private void moveCall(int _moveArrow)
    {
        
        isRolling = true;
        for (int i = 0; i < cardEntity.Count; i++)
        {
            cardEntity[i].GetComponent<CardEntity>().ChangePos(_moveArrow);
        }

    }








}
