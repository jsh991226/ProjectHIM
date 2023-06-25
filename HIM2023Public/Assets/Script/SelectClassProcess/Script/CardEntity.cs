using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEntity : MonoBehaviour
{


    public Vector2 myPos;
    public int layerNum;
    public List<Vector2> cardPos;
    public bool isMove;
    public int maxLayer;
    public string myClass;
    public SelectClassCtrl selectCtrl;



    private Rigidbody2D rb;
    private float speed = 800f;
    private float voidXPos = 400f;
    private bool isVoiding;
    private Vector2 CenterScale = new Vector2(1.2f, 1.2f);
    private int cardCenter;
    private int moveArrow;
    private int UpScalingCount = 5;




    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        myPos = rb.position;
    }

    public void ChangePos(int _moveArrow)
    {
        moveArrow = _moveArrow;
        isMove = true;
    }



    private void CheckCenter()
    {
        cardCenter = cardPos.Count / 2;
        if (layerNum == cardCenter)
        {
            selectCtrl.nowSel = myClass;
            StartCoroutine(UpScaling());
        }
        else
        {
            transform.localScale = new Vector2(1.0f, 1.0f);
        }

    }

    IEnumerator UpScaling()
    {
        
        float gapX = (CenterScale.x - transform.localScale.x) / UpScalingCount;
        float gapY = (CenterScale.y - transform.localScale.y) / UpScalingCount;
        for ( int i = 0; i < UpScalingCount; i ++)
        {
            Vector2 currentScale = transform.localScale;
            currentScale.x += gapX;
            currentScale.y += gapY;
            transform.localScale = currentScale;
            yield return new WaitForSeconds(0.02f);
        }


    }



    private void FixedUpdate()
    {
        if (isMove)
        {
            int targetIndex;
            if ( moveArrow > 0) targetIndex = layerNum != maxLayer ? layerNum + 1 : 0;
            else targetIndex = layerNum > 0 ? layerNum - 1 : maxLayer;
            if ((layerNum == maxLayer && moveArrow > 0) || (layerNum == 0 && moveArrow < 0))
            {
                if (!isVoiding)
                {
                    Vector2 voidTarget = cardPos[layerNum]; voidTarget.x += (voidXPos * moveArrow);
                    Vector2 newPos = Vector2.MoveTowards(rb.position, voidTarget, speed*Time.fixedDeltaTime*3);
                    rb.MovePosition(newPos);
                    if (newPos == rb.position)
                    {
                        newPos = cardPos[targetIndex]; newPos.x -= (voidXPos * moveArrow);
                        transform.position = newPos;
                        isVoiding = true;
                    }
                } else
                {
                    Vector2 newPos = Vector2.MoveTowards(rb.position, cardPos[targetIndex], speed * Time.fixedDeltaTime * 3);
                    rb.MovePosition(newPos);
                    if (newPos == rb.position)
                    {
                        myPos = newPos;
                        isVoiding = false;
                        layerNum = targetIndex;
                        CheckCenter();
                        isMove = false;
                    }
                }

            }
            else
            {
                Vector2 newPos = Vector2.MoveTowards(rb.position, cardPos[targetIndex], speed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
                if (newPos == rb.position)
                {
                    myPos = newPos;
                    layerNum = targetIndex;
                    CheckCenter();
                    isMove = false;
                }
            }






        }



    }


}
