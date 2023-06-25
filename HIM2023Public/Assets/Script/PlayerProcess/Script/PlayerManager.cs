using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using FarrokhGames.Inventory.Examples;

using Unity.VisualScripting;
using static UnityEngine.ParticleSystem;
using static UnityEngine.UI.Image;
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    public PhotonView PV;

    [SerializeField]
    public GameObject FPCamera;

    [SerializeField]
    private Vector3 currPos;

    [SerializeField]
    private Quaternion currRot;

    [SerializeField]
    private Transform tr;

    [SerializeField]
    public float speed = 6f;

    [SerializeField]
    public float rotSpeed = 120f;

    [SerializeField]
    private float HP = 100f;

    private float JumpForce = 3.0f;

    private WeaponScript weapon;

    private Rigidbody rigd;

    public DBManager dbm;

    public GameObject gameManager;
    public string nickName;

    public PhotonAnimatorView PVAnim;

    public Animator animator;

    private string GitHubTestVar = "test";

    public GameObject fpRoot;
    private Transform RootTr;

    public bool isMoveRightAttack, isMoveLeftAttack, isIdleRightAttack, isIdleLeftAttack;

    public bool isAttacking;
    public bool isMove;

    public bool DoSitting;

    public bool isRunning;

    public bool isSittingMove;

    public bool isDamege;

    public bool isGameIn;

    [SerializeField]
    private bool isDeadFirst;

    private bool bFirst;

    public bool isDead;

    public bool isRolling;

    public bool isRollingLast;

    public bool isStanding;

    private Text NickName;

    public bool DoJumping;

    private bool isJumpingLast;

    private string thisCharName;

    private string[] thisCharNameSub;

    private string thisTypeStr;

    private AnimatorClipInfo[] MoveAttackClip;
    private float AnimaInfoInLength;
    private string AttackAnimName;

    public CameraController cameraCtrl;

    public Text actionText;

    public EventController evc;
    public bool completeExit;

    public Transform leftHandTr;
    public Transform rightHandTr;

    public NotifyCtrl notify;
    public ChatCtrl chatCtrl;
    public TextMesh PlayerNickNameText;
    public bool InArea;
    public PanelManager loadingPnl;

    public Bloods bloods;

    public GameObject eyeSight;

    public ParticleSystem dust;
    private Vector3 dustScale;

    private PlayerSoundManager psm;

    public PlayerInventory playerInv;

    private GameObject PotionObjLeft;
    private GameObject PotionObjRight;

    private UserData userdata;

    public Image hpProg;
    private float playerMaxHP;
    public ReEquip leftEquip;
    public ReEquip rightEquip;

    private NPCManager npcMgr;

    public float attackSpeed, walkSpeed, runSpeed;

    public bool isJump;

    public QuestManager qm;

    public ParticleSystem bloodEffect, hitEffect;
    public enum ClassType
    {
        Goblin,
        Golem,
        Ogre,
        Ork,
        Skeleton
    }

    // Start is called before the first frame update
    void Start()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();

        PlayerNickNameText.text = photonView.Owner.NickName;
        nickName = PhotonNetwork.NickName;

        playerMaxHP = GetComponent<LivingEntity>().Health;
        tr = gameObject.GetComponent<Transform>();
        animator = GetComponent<Animator>();
        PVAnim = GetComponent<PhotonAnimatorView>();
        psm = GetComponent<PlayerSoundManager>();

        isMoveRightAttack = false;
        isMoveLeftAttack = false;
        isIdleLeftAttack = false;
        isIdleRightAttack = false;

        isDeadFirst = false;
        bFirst = false;
        thisCharName = this.gameObject.name;
        thisCharNameSub = thisCharName.Split("(");
        thisTypeStr = userdata.UserClass;
        PotionObjLeft = leftHandTr.Find("Potion_Health_Hand_Left").gameObject;
        PotionObjRight = rightHandTr.Find("Potion_Health_Hand_Right").gameObject;
        //StartGround = false;
        RootTr = fpRoot.transform;
        rigd = GetComponent<Rigidbody>();
        PV.RPC("SetOtherLayer", RpcTarget.OthersBuffered);
        dustScale = new Vector3(dust.transform.localScale.x, dust.transform.localScale.y, dust.transform.localScale.z);
        if (GameObject.Find("NPCManager") != null) npcMgr = GameObject.Find("NPCManager").GetComponent<NPCManager>();

    }

    public void addKill()
    {
        StartCoroutine(addCountSql("killhuman"));
    }

    public void addDeath()
    {
        StartCoroutine(addCountSql("death"));
    }

    IEnumerator addCountSql( string _column)
    {
        Debug.Log("실행됨 : " + _column);
        string sql = "update user set "+ _column+" = "+ _column+ " + 1 where nickname = '" + nickName +"'";
        dbm.QueryData(sql);
        yield return null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PV.IsMine) return;
        if (other.tag == "NPC")
        {
            npcMgr.NPCEvent(other.GetComponent<NPCEntity>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine) return;
        if (other.tag == "NPC")
        {
            npcMgr.NPCEvent(other.GetComponent<NPCEntity>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!PV.IsMine) return;
        if (other.tag == "NPC")
        {
            npcMgr.NPCEvent(other.GetComponent<NPCEntity>());
            npcMgr.CloseShop();
            npcMgr.NPCEvent(null);
        }
    }


    [PunRPC]
    private void SetOtherLayer()
    {
        if (!PV.IsMine)
        {
            SetLayersRecursively(gameObject.transform, "OtherPlayer");
        }

    }

    public void SetLayersRecursively(Transform trans, string name)
    {
        if (trans.tag == "MinimapIcon")
        {
            trans.gameObject.SetActive(false);
            return;
        }

        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            SetLayersRecursively(child, name);
        }
    }



    [PunRPC]
    public void ShowDust(bool _type)
    {
        if (dust.isPlaying == true) return;
        dust.Play();
        if (_type) PV.RPC("ShowDust", RpcTarget.Others, false);

    }
    [PunRPC]
    public void StopDust(bool _type)
    {
        dust.Stop();
        if (_type) PV.RPC("StopDust", RpcTarget.Others, false);

    }
    [PunRPC]
    public void SetScale(float _mul, bool _type)
    {
        dust.transform.localScale = new Vector3(dustScale.x + _mul, dustScale.y + _mul, dustScale.z + _mul);
        if (_type) PV.RPC("SetScale", RpcTarget.Others, _mul, false);

    }

    Coroutine isWalk, isRun = null;

    void NoSoundDust()
    {
        if (isWalk != null)
            StopCoroutine(isWalk);
        if (isRun != null)
            StopCoroutine(isRun);
        isWalk = null;
        isRun = null;
        StopDust(true);
    }



    private void Update()
    {
        if (PV.IsMine && !isDead)
        {
            KeyAction();
            tr.rotation = new Quaternion(0f, 0f, 0f, 0f);

            isSittingMove = animator.GetBool("isSittingMove");
            isRunning = animator.GetBool("isRunning");
            isMove = animator.GetBool("isMove");

            isDead = GetComponent<LivingEntity>().isDead;

            UpdateState();

            MoveAnim();
            //Debug.Log("체력 : " + GetComponent<LivingEntity>().Health + " / " + playerMaxHP + " = " + (GetComponent<LivingEntity>().Health / playerMaxHP));
            hpProg.fillAmount = GetComponent<LivingEntity>().Health / playerMaxHP;

            if (isMove)
            {
                ShowDust(true);

                if (isJump == true)
                    NoSoundDust();

                if (isWalk == null)
                    isWalk = StartCoroutine(Moving());

                if (isRunning)
                {
                    if (isWalk != null)
                        StopCoroutine(isWalk);
                    if (isRun == null)
                        isRun = StartCoroutine(Moving());
                    SetScale(1.0f, true);
                }
                else
                    SetScale(0.4f, true);
            }
            else
                NoSoundDust();
        }
        else
        {
            rigd.isKinematic = true;
        }
        PV.RPC("CharAnim", RpcTarget.All);
    }

    IEnumerator Moving()
    {
        while (isMove)
        {
            if (isRunning == false)
                yield return new WaitForSecondsRealtime(0.235f / walkSpeed);
            else
                yield return new WaitForSecondsRealtime(0.215f / runSpeed);
            PV.RPC("walkSoundPlay", RpcTarget.All);
        }
    }

    [PunRPC]
    public void walkSoundPlay()
    {
        psm.PlaySound3D("Walking");
    }



    private void FixedUpdate()
    {
        if (PV.IsMine)
        {

        }
        else
        {
            if (tr.position != currPos)
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
                RootTr.rotation = Quaternion.Slerp(RootTr.rotation, currRot, Time.deltaTime * 10.0f);
                currPos = tr.position;
            }
        }
    }



    private void MoveAnim()
    {
        currPos.x = Input.GetAxisRaw("Vertical");
        currPos.z = Input.GetAxisRaw("Horizontal");

        currPos.Normalize();
    }

    private void UpdateState()
    {
        if (PV.IsMine)
        {
            if (Mathf.Approximately(currPos.z, 0) && Mathf.Approximately(currPos.x, 0) || isStanding)
            {
                isMove = false;
                animator.SetBool("isMove", false);
            }
            else
            {
                isMove = true;
                animator.SetBool("isMove", true);
            }
            animator.SetFloat("xDir", currPos.x);
            animator.SetFloat("yDir", currPos.z);
        }
    }


    private void UsePotion(float _num)
    {
        if (GetComponent<LivingEntity>().Health + _num >= playerMaxHP)
        {
            GetComponent<LivingEntity>().Health = playerMaxHP;
        }
        else
        {
            GetComponent<LivingEntity>().Health += _num;
        }

    }

    [PunRPC]
    private void CharAnim()
    {
        if (PV.IsMine)
        {
            animator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));

            if (Input.anyKeyDown)
            {
                //isKeyDown = true;
            }
            else
            {
                //isKeyDown = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("StandToSitting") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("SittingMoveBlend"))
                {
                    rigd.constraints = RigidbodyConstraints.FreezePositionY;
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("SittingIdle"))
                {
                    rigd.constraints = RigidbodyConstraints.FreezeAll;
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && animator.GetBool("isGround"))
            {
                DoSitting = true;
                animator.SetBool("isSitting", true);

                if (animator.GetBool("isSitting") && isMove)
                {
                    //currPos = new Vector3(currPos.x, this.transform.position.y - 0.01f, currPos.z);
                    //currPos = new Vector3(currPos.x, currPos.y - 0.02f, currPos.z);
                    animator.SetBool("isSittingMove", true);
                }
                else if (animator.GetBool("isSitting") && Input.GetMouseButtonDown(1))
                {
                    animator.SetTrigger("SittingAttack");
                }
                else
                {
                    animator.SetBool("isSittingMove", false);
                    return;
                }
            }
            else
            {
                DoSitting = false;
                rigd.constraints = RigidbodyConstraints.None;
                rigd.constraints = RigidbodyConstraints.FreezeRotation;
                animator.SetBool("isSitting", false);
                animator.SetBool("isSittingMove", false);
            }

            if (Input.GetButtonDown("Jump") && animator.GetBool("isGround") && !DoJumping && !isRolling && !isAttacking && !Input.GetButton("Rolling"))
            {
                if (animator.GetBool("isRunning"))
                {
                    animator.SetTrigger("RunningJump");
                    StartCoroutine(Jumping(0.25f));
                }

                if (isMove && !animator.GetBool("isRunning"))
                {
                    animator.SetTrigger("Jumping");
                    StartCoroutine(Jumping(0.25f));
                }
            }

            if (animator.GetBool("isGround"))
            {
                if (Input.GetButtonDown("Jump") && !DoJumping && !isMove && !isRolling && !isAttacking && !Input.GetButton("Rolling"))
                {
                    animator.SetTrigger("idleJumping");
                    StartCoroutine(Jumping(0.76f));
                }
            }

            IEnumerator Jumping(float _time)
            {
                isJump = true;
                yield return new WaitForSecondsRealtime(_time);
                isJump = false;
            }

            if (!animator.GetBool("isGround") && animator.GetBool("isJumping"))
            {
                animator.SetBool("isFalling", true);
            }
            animator.SetBool("isJumping", false);

            if (Input.GetMouseButtonDown(1) && !isMove && PotionObjRight.active == true)
            {
                //포션 처리할 부분
                if (PotionObjRight.active == true)
                {
                    UsePotion(40);
                    ChangeWeapon(gameObject.GetComponent<PlayerManager>(), "empty", true, 0, false);
                    rightEquip.UseEquip();
                    animator.SetTrigger("RightPotion");
                    return;
                }
                else
                {
                    animator.SetTrigger("Attack");
                }
                animator.ResetTrigger("RightPotion");
            }

            if (Input.GetMouseButtonDown(0) && !isMove && PotionObjLeft.active == true)
            {
                //포션 처리할 부분
                if (PotionObjLeft.active == true)
                {
                    UsePotion(40);
                    ChangeWeapon(gameObject.GetComponent<PlayerManager>(), "empty", false, 0, false);
                    leftEquip.UseEquip();
                    animator.SetTrigger("LeftPotion");
                    return;
                }
                else
                {
                    animator.SetTrigger("LeftAttack");
                }
                animator.ResetTrigger("LeftPotion");

            }

            if (Input.GetMouseButtonDown(1) && !isMove)
            {
                animator.SetTrigger("Attack");
            }

            if (Input.GetMouseButtonDown(0) && !isMove)
            {
                animator.SetTrigger("LeftAttack");
            }

            if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
            {
                animator.SetBool("isAttack", true);
            }
            else
            {
                animator.SetBool("isAttack", false);
            }

            if (Input.GetButtonDown("Rolling") && !isRolling && !DoJumping && !isJumpingLast && !Input.GetButton("Jump"))
            {
                animator.SetTrigger("Rolling");
            }

            if (isMove && !DoJumping)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    animator.SetTrigger("MoveRightAttack");
                }

                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetTrigger("MoveLeftAttack");
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("IdleJump") ||
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                {
                    DoJumping = true;
                }
            }
            else
            {
                DoJumping = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
                {
                    isRollingLast = true;
                    // StartCoroutine("RollingPower");
                }
            }
            else
            {
                isRollingLast = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackOne") || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackTwo") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("LeftOne") || animator.GetCurrentAnimatorStateInfo(0).IsName("LeftTwo"))
            {
                isAttacking = true;
            }
            else
                isAttacking = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackOne") || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackTwo"))
            {
                isIdleRightAttack = true;
            }
            else
                isIdleRightAttack = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftOne") || animator.GetCurrentAnimatorStateInfo(0).IsName("LeftTwo"))
            {
                isIdleLeftAttack = true;
            }
            else
                isIdleLeftAttack = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("OnDamege"))
            {
                isDamege = true;
            }
            else
                isDamege = false;

            if (animator.GetCurrentAnimatorStateInfo(1).nameHash == Animator.StringToHash("Upper Layer.MoveRightAttack") ||
                    animator.GetCurrentAnimatorStateInfo(1).nameHash == Animator.StringToHash("Upper Layer.MoveRightAttackCombo") ||
                    animator.GetCurrentAnimatorStateInfo(1).nameHash == Animator.StringToHash("Upper Layer.CrouchAttack"))
            {
                isMoveRightAttack = true;
            }
            else
            {
                isMoveRightAttack = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(1).nameHash == Animator.StringToHash("Upper Layer.MoveLeftAttack") ||
                    animator.GetCurrentAnimatorStateInfo(1).nameHash == Animator.StringToHash("Upper Layer.MoveLeftAttackCombo"))
            {
                isMoveLeftAttack = true;
            }
            else
            {
                isMoveLeftAttack = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(1).IsName("MoveRightAttack") || animator.GetCurrentAnimatorStateInfo(1).IsName("MoveRightAttackCombo") ||
                    animator.GetCurrentAnimatorStateInfo(1).IsName("MoveLeftAttack") || animator.GetCurrentAnimatorStateInfo(1).IsName("MoveLeftAttackCombo")
                    || animator.GetCurrentAnimatorStateInfo(1).IsName("CrouchAttack"))
            {
                //Debug.Log("달리면서 때린다");
                isAttacking = true;
            }
            else
            {
                //Debug.Log("달리면서 다 팼다");
                //isAttacking = false;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
                {
                    isDeadFirst = true;
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
                {
                    isRolling = true;
                    // StartCoroutine("RollingPower");
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("IdleJump") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                {
                    isJumpingLast = true;
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("SittingToStand"))
                {
                    //tr.position -= new Vector3(0, 0.006f, 0);
                    isStanding = true;
                }

            }
            else
            {
                //StopCoroutine("RollingPower");
                isDeadFirst = false;
                isRolling = false;
                isJumpingLast = false;
                isStanding = false;
            }

            switch (thisTypeStr)
            {
                case "Ork":
                    attackSpeed = 1.3f;
                    walkSpeed = 0.9f;
                    runSpeed = 1.3f;
                    break;

                case "Ogre":
                    attackSpeed = 1.8f;
                    walkSpeed = 0.6f;
                    runSpeed = 1.0f;
                    break;

                case "Goblin":
                    attackSpeed = 1.0f;
                    walkSpeed = 1.2f;
                    runSpeed = 1.6f;
                    break;

                case "Golem":
                    attackSpeed = 1.4f;
                    walkSpeed = 0.5f;
                    runSpeed = 0.9f;
                    break;

                case "Skeleton":
                    attackSpeed = 1.2f;
                    walkSpeed = 1.0f;
                    runSpeed = 1.4f;
                    break;
            }
            animator.SetFloat("AttackSpeed", attackSpeed);
            animator.SetFloat("WalkingSpeed", walkSpeed);
            animator.SetFloat("RunningSpeed", runSpeed);
        }
    }


    public void SyncDestroyItem(string _nick, int _viewId)
    {
        if (PhotonNetwork.NickName != _nick) return;
        Debug.LogError("요청 받음 : " + _nick + " : " + _viewId);
        PhotonNetwork.Destroy(PhotonView.Find(_viewId));
    }

    public void SyncRemoveMob(int _viewId)
    {
        Debug.Log("DEbug : " + _viewId);
        PhotonView.Find(_viewId).gameObject.GetComponent<EnemyKnight>().HP = 0;
    }

    public void RemoveMob(PhotonView _mobView)
    {
        object[] data = { _mobView.ViewID };
        evc.SendRaiseEvent(EventController.EVENTCODE.MOBREMOVE, data, EventController.SEND_OPTION.ALL);
    }


    public void DisableChild(Transform[] childList)
    {
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    childList[i].gameObject.SetActive(false);
            }
        }
    }


    public void ChangeWeapon(PlayerManager _pm, string _itemName, bool _handType, float _damage, bool _sync)
    {
        if (_itemName == "empty")
        {
            if (_handType) DisableChild(_pm.rightHandTr.GetComponentsInChildren<Transform>());
            else DisableChild(_pm.leftHandTr.GetComponentsInChildren<Transform>());
        }
        else
        {
            if (_handType)
            {
                DisableChild(_pm.rightHandTr.GetComponentsInChildren<Transform>());
                _pm.rightHandTr.Find(_itemName + "_Hand_Right").gameObject.SetActive(true);
                if (_itemName.Contains("Potion"))
                {

                }
                else
                {
                    _pm.rightHandTr.Find(_itemName + "_Hand_Right").gameObject.GetComponent<WeaponScript>().Damege = _damage;
                }


            }
            else
            {
                DisableChild(_pm.leftHandTr.GetComponentsInChildren<Transform>());
                _pm.leftHandTr.Find(_itemName + "_Hand_Left").gameObject.SetActive(true);
                if (_itemName.Contains("Potion"))
                {

                }
                else
                {
                    _pm.leftHandTr.Find(_itemName + "_Hand_Left").gameObject.GetComponent<WeaponScript>().Damege = _damage;
                }
            }
        }
        object[] data = { nickName, _itemName, _handType, _damage };
        if (_sync == false) evc.SendRaiseEvent(EventController.EVENTCODE.CHANGEWEAPON, data, EventController.SEND_OPTION.OTHER);

    }

    public void SyncChangeItem(string _nick, string _itemName, bool _handType, float _damage)
    {
        GameObject[] _playerList = GameObject.FindGameObjectsWithTag("Player");
        Debug.LogError("유저수 : " + _playerList.Length);
        for (int i = 0; i < _playerList.Length; i++)
        {
            string[] _nickList = (_playerList[i].GetComponent<PhotonView>().Owner + "").Split("'");
            if (_nick == _nickList[1])
            {
                Debug.LogError("찾은 유저 : " + _nick);

                ChangeWeapon(_playerList[i].GetComponent<PlayerManager>(), _itemName, _handType, _damage, true);
                break;

            }

        }

    }


    



    private void KeyAction()
    {
        if (PV.IsMine && !isDead)
        {
            if (actionText.IsActive())
            {
                if (actionText.text == "Take 'E'")
                {
                    if (Input.GetKeyDown("e"))
                    {
                        if (playerInv.AddWatchingItem() == true)
                        {
                            string[] _nick = (ActionController.lookedItem.GetComponent<PhotonView>().Owner + "").Split("'");
                            // Debug.Log(" 소유주 자른거: " + _nic[1]);x

                            object[] data = { _nick[1], ActionController.lookedItem.GetComponent<PhotonView>().ViewID };
                            evc.SendRaiseEvent(EventController.EVENTCODE.DESTROYITEM, data, EventController.SEND_OPTION.ALL);
                            //PhotonNetwork.Destroy(ActionController.lookedItem);

                        }
                        else
                        {
                            notify.CastNotify("인벤토리[Q] 가 가득 찼습니다 정리 후 이용해주세요.");
                            chatCtrl.ChatNotify("인벤토리[Q] 가 가득 찼습니다 정리 후 이용해주세요.");
                        }
                    }
                }
                if (actionText.text == "Open 'Q'")
                {
                    if (Input.GetKeyDown("q"))
                    {
                        OtherInventory.ShowWatchingBag();
                    }
                }
            }
        }
    }

    [PunRPC]
    private void TakeDamegeAnim()
    {
        //PV.RPC("TakeDamegeAnim", RpcTarget.Others);
        if (!isDead && !isDeadFirst)
        {
            animator.SetTrigger("TakeDamege");
        }
    }

    [PunRPC]
    public void PlayerDie()
    {

        if (!bFirst)
        {
            animator.SetTrigger("Die");
            PV.RPC("PlayerDie", RpcTarget.Others);
            if (PV.IsMine)
            {
                notify.CastNotify("사망 하였습니다! 퀵슬롯의 아이템을 잃고 로비로 돌아갑니다");
                StartCoroutine(returnLobby());
            }
            else
            {
                notify.CastNotify("습격조 인원이 사망 하였습니다");
            }
        }
        bFirst = true;

    }

    IEnumerator returnLobby()
    {
        yield return new WaitForSeconds(1f);
        loadingPnl.GUIToggle(true);
        yield return new WaitForSeconds(1f);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.ConnectUsingSettings();

    }


    private void OnCollisionStay(Collision collision)
    {
        if (PV.IsMine)
        {
            if (collision.gameObject.layer == 7)
            {
                animator.SetBool("isGround", true);
            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (PV.IsMine)
        {
            animator.SetBool("isGround", false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(RootTr.rotation);
            stream.SendNext(isDead);
            stream.SendNext(isAttacking);
            stream.SendNext(isGameIn);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
            isAttacking = (bool)stream.ReceiveNext();
            isGameIn = (bool)stream.ReceiveNext();
        }
    }

    Coroutine isBlindCor, isSlowCor = null;
    bool isSlow = false;

    public enum DebuffType
    {
        bleed,
        slow,
        blind
    }

    public void Debuff(DebuffType _type, float _time, object _etc)
    {
        if (_type == DebuffType.bleed)
        {
            StartCoroutine(bleeding(_time, (float)_etc));
        }

        IEnumerator bleeding(float _time, float _tickDmg)
        {
            LivingEntity target = gameObject.GetComponent<LivingEntity>();
            while (_time > 0)
            {
                _time -= 0.5f;
                yield return new WaitForSecondsRealtime(0.5f);
                target.TakeDamege(_tickDmg);
            }
        }
    }

    public void Debuff(DebuffType _type, float _time)
    {

        if (_type == DebuffType.bleed)
        {
            StartCoroutine(bleeding(_time));
        }

        if (_type == DebuffType.slow)
        {
            if (isSlowCor != null)
                StopCoroutine(isSlowCor);
            isSlowCor = StartCoroutine(slowing(_time));
        }

        if (_type == DebuffType.blind)
        {
            if (isBlindCor != null)
                StopCoroutine(isBlindCor);
            isBlindCor = StartCoroutine(blinding(_time));
        }

        IEnumerator bleeding(float _time)
        {
            LivingEntity target = gameObject.GetComponent<LivingEntity>();
            while (_time > 0)
            {
                _time -= 0.5f;
                yield return new WaitForSecondsRealtime(0.5f);
                target.TakeDamege(5f);
            }
        }

        IEnumerator slowing(float _time)
        {
            if (isSlow == false)
                cameraCtrl.MoveOption.speed *= 0.6f;
            isSlow = true;
            yield return new WaitForSeconds(_time);
            if (isSlow == true)
                cameraCtrl.MoveOption.speed /= 0.6f;
            isSlow = false;
            yield return null;
        }

        IEnumerator blinding(float _time)
        {
            eyeSight.SetActive(true);
            yield return new WaitForSeconds(_time);
            eyeSight.SetActive(false);
            yield return null;
        }
    }

    public void ShowBlood(Vector3 _position)
    {
        StartCoroutine(EffectStart(bloodEffect, _position));
    }

    public void ShowDamage(Vector3 _position)
    {
        StartCoroutine(EffectStart(hitEffect, _position));
    }

    private IEnumerator EffectStart(ParticleSystem _effect, Vector3 _position)
    {
        ParticleSystem newEffect = Instantiate(_effect);
        int num = Random.Range(0, 361);
        newEffect.GetComponent<Transform>().localEulerAngles = new Vector3(0, num, 0);
        newEffect.GetComponent<Transform>().position = _position;
        newEffect.Play();
        yield return new WaitForSeconds(1.0f);
        Destroy(newEffect.gameObject);
    }

    public void WeaponWield()
    {
        PV.RPC("WieldSoundPlay", RpcTarget.All);
    }

    [PunRPC]
    public void WieldSoundPlay()
    {
        int num = Random.Range(1, 4);
        string soundName = "Wield 0" + num;
        psm.PlaySound3D(soundName);
    }

    public void WeaponAttack()
    {
        PV.RPC("AttackSoundPlay", RpcTarget.All);
    }

    [PunRPC]
    public void AttackSoundPlay()
    {
        psm.PlaySound3D("Attack 01");
    }
}