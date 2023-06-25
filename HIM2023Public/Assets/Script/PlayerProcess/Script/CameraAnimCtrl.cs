using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimCtrl : MonoBehaviour
{
    private Animator cameraAni;
    private PlayerManager PMgr;
    private LivingEntity livingEntity;
    private float walkSpeed, runSpeed;
    int leftAttacking, rightAttacking, rolling, jumping = 0;
    bool sitting = false;
    // Start is called before the first frame update
    void Start()
    {
        cameraAni = GetComponent<Animator>();
        PMgr = GetComponentInParent<PlayerManager>();
        livingEntity = GetComponentInParent<LivingEntity>();
        //chaAni = GetComponentInParent<Animator>();
        walkSpeed = PMgr.walkSpeed;
        runSpeed = walkSpeed + 0.4f;

        cameraAni.SetFloat("AttackSpeed", PMgr.attackSpeed);
    }
    void Update()
    {
        AttackCamera();
        CameraWalkMove();
    }

    private void CameraWalkMove()
    {

        if (PMgr.DoJumping)
        {
            jumping++;
            cameraAni.SetInteger("isJumping", jumping);
        }
        else jumping = 0;

        if (PMgr.DoSitting)
            cameraAni.SetBool("isSitting", true);
        else
            cameraAni.SetBool("isSitting", false);

        if (PMgr.isMove && !PMgr.isSittingMove)
        {
            cameraAni.SetFloat("MoveSpeed", walkSpeed);

            if (PMgr.isRunning)
            {
                cameraAni.SetFloat("MoveSpeed", runSpeed);
            }
            if (!PMgr.isAttacking)
            {
                if (livingEntity.Health >= 50)
                    cameraAni.SetBool("isWalk", PMgr.isMove);
                else
                    cameraAni.SetBool("isHurtWalk", PMgr.isMove);
            }
        }

        if (PMgr.isSittingMove || !PMgr.isMove)
        {
            cameraAni.SetBool("isWalk", false);
            cameraAni.SetBool("isHurtWalk", false);
        }

        if (PMgr.isRolling)
        {
            rolling++;
            cameraAni.SetInteger("isRolling", rolling);
        }
        else rolling = 0;
    }

    void AttackCamera()
    {
        if (PMgr.isMoveRightAttack || PMgr.isIdleRightAttack)
        {
            rightAttacking++;
            cameraAni.SetInteger("isRightOne", rightAttacking);
        }
        else rightAttacking = 0;

        if (PMgr.isMoveLeftAttack || PMgr.isIdleLeftAttack)
        {
            leftAttacking++;
            cameraAni.SetInteger("isLeftOne", leftAttacking);
        }
        else leftAttacking = 0;
    }
    public void AttackedCameraMove(float _damage)
    {
        if (100 >= _damage && _damage >= 40)
        {
            cameraAni.SetTrigger("isStrongAtt");
        }

        if (39 >= _damage && _damage >= 20)
        {
            cameraAni.SetTrigger("isMiddleAtt");
        }

        if (19 >= _damage && _damage >= 1)
        {
            cameraAni.SetTrigger("isWeakAtt");
        }
    }
}