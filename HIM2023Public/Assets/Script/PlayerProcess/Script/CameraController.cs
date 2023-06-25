using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool CanMove = true;


    /***********************************************************************
    *                               Definitions
    ***********************************************************************/
    #region .
    [SerializeField]
    private PhotonView PV;

    public GameObject DollyCam;
    public GameObject Area02BossCam;
    public enum CameraType { FpCamera, TpCamera };

    PlayerManager PMgr;

    LivingEntity PLive;

    [Serializable]
    public class Components
    {
        public Camera tpCamera;
        public Camera fpCamera;

        [SerializeField]
        [HideInInspector] public Transform tpRig;

        [SerializeField]
        [HideInInspector] public Transform fpRoot;

        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject tpCamObject;
        [HideInInspector] public GameObject fpCamObject;

        [SerializeField]
        [HideInInspector] public Rigidbody rBody;

        [HideInInspector] public Animator anim;
    }
    [Serializable]
    public class KeyOption
    {
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode run = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;
        public KeyCode switchCamera = KeyCode.Tab;
        public KeyCode showCursor = KeyCode.LeftAlt;
    }
    [Serializable]
    public class MovementOption
    {
        [SerializeField]
        [Range(1f, 10f), Tooltip("이동속도")]
        public float speed = 3f;

        [Range(1f, 3f), Tooltip("달리기 이동속도 증가 계수")]
        public float runningCoef = 1.5f;
        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 5.5f;
    }
    [Serializable]
    public class CameraOption
    {
        [Tooltip("게임 시작 시 카메라")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("카메라 상하좌우 회전 속도")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("올려다보기 제한 각도")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("내려다보기 제한 각도")]
        public float lookDownDegree = 75f;
        [Range(0f, 3.5f), Space, Tooltip("줌 확대 최대 거리")]
        public float zoomInDistance = 3f;
        [Range(0f, 5f), Tooltip("줌 축소 최대 거리")]
        public float zoomOutDistance = 3f;
        [Range(1f, 20f), Tooltip("줌 속도")]
        public float zoomSpeed = 10f;
        [Range(0.01f, 0.5f), Tooltip("줌 가속")]
        public float zoomAccel = 0.1f;
    }
    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveY = "Move Y";
        public string paramMoveZ = "Move Z";
    }
    [Serializable]
    public class CharacterState
    {
        public bool isMaxZoomOut;
        public bool isCurrentFp;
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isCursorActive;
    }

    #endregion
    /***********************************************************************
    *                               Fields, Properties
    ***********************************************************************/
    #region .
    public Components Com => _components;
    public KeyOption Key => _keyOption;
    public MovementOption MoveOption => _movementOption;
    public CameraOption CamOption => _cameraOption;
    public AnimatorOption AnimOption => _animatorOption;
    public CharacterState State => _state;

    /// <summary> TP 카메라 ~ Rig 초기 거리 </summary>
    private float _tpCamZoomInitialDistance;

    /// <summary> TP 카메라 휠 입력 값 </summary>
    private float _tpCameraWheelInput = 0;

    /// <summary> 선형보간된 현재 휠 입력 값 </summary>
    private float _currentWheel;

    [SerializeField] private Components _components = new Components();
    [Space]
    [SerializeField] private KeyOption _keyOption = new KeyOption();
    [Space]
    [SerializeField] private MovementOption _movementOption = new MovementOption();
    [Space]
    [SerializeField] private CameraOption _cameraOption = new CameraOption();
    [Space]
    [SerializeField] private AnimatorOption _animatorOption = new AnimatorOption();
    [Space]
    [SerializeField] private CharacterState _state = new CharacterState();

    [SerializeField]
    private Vector3 _moveDir;

    [SerializeField]
    private Vector3 _worldMove;

    [SerializeField]
    private Vector2 _rotation;

    private float _deltaTime;

    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        //DollyCam = GameObject.Find("BossDollyCam");
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            if (SceneManager.GetActiveScene().name == "Area01")
            {
                DollyCam = GameObject.Find("BossDollyCam");
            }
            if (SceneManager.GetActiveScene().name == "Area02")
            {
                Area02BossCam = GameObject.Find("CinemaManager");
            }
            PMgr = GetComponent<PlayerManager>();
            PLive = this.gameObject.GetComponent<LivingEntity>();
            InitComponents();
            InitSettings();
        }
    }

    #endregion
    /***********************************************************************
    *                               Init Methods
    ***********************************************************************/
    #region .
    private void InitComponents()
    {
        LogNotInitializedComponentError(Com.tpCamera, "TP Camera");
        LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
        TryGetComponent(out Com.rBody);
        Com.anim = GetComponentInChildren<Animator>();

        Com.tpCamObject = Com.tpCamera.gameObject;
        Com.tpRig = Com.tpCamera.transform.parent;
        Com.fpCamObject = Com.fpCamera.gameObject;
        Com.fpRig = Com.fpCamera.transform.parent;
        Com.fpRoot = Com.fpRig.parent;
    }

    private void InitSettings()
    {
        // Rigidbody
        if (Com.rBody)
        {
            // 회전은 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Camera
        var allCams = FindObjectsOfType<Camera>();
        foreach (var cam in allCams)
        {
            if (cam.gameObject.tag != "MiniMapCamera")
                cam.gameObject.SetActive(false);
        }
        // 설정한 카메라 하나만 활성화
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FpCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);
        Com.tpCamObject.SetActive(!State.isCurrentFp);

        _tpCamZoomInitialDistance = Vector3.Distance(Com.tpRig.position, Com.tpCamera.transform.position);
    }

    #endregion
    /***********************************************************************
    *                               Checker Methods
    ***********************************************************************/
    #region .
    private void LogNotInitializedComponentError<T>(T component, string componentName) where T : Component
    {
        if (component == null)
            Debug.LogError($"{componentName} 컴포넌트를 인스펙터에 넣어주세요");
    }

    #endregion
    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .

    #endregion

    /// <summary> 키보드 입력을 통해 필드 초기화 </summary>
    private void SetValuesByKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) v += 1.0f;
        if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
        if (Input.GetKey(Key.moveRight)) h += 1.0f;

        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.speed); // 가속, 감속
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;
        State.isRunning = Input.GetKey(Key.run);

        _tpCameraWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        _currentWheel = Mathf.Lerp(_currentWheel, _tpCameraWheelInput, CamOption.zoomAccel);
    }

    [PunRPC]
    /// <summary> 1인칭 회전 </summary>
    private void Rotate()
    {
        if (State.isCurrentFp)
        {
            if (!State.isCursorActive)
                RotateFP();
        }
        else
        {
            if (!State.isCursorActive)
                RotateTP();
            RotateFPRoot();
        }
    }

    [PunRPC]
    /// <summary> 1인칭 회전 </summary>
    private void RotateFP()
    {
       // if (!CanMove) return; 1인칭 마우스 ui에서 lock
        float deltaCoef = _deltaTime * 50f;

        // 상하 : FP Rig 회전
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // 좌우 : FP Root 회전
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // 상하 회전 가능 여부
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        // FP Rig 상하 회전 적용
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // FP Root 좌우 회전 적용
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    [PunRPC]
    /// <summary> 3인칭 회전 </summary>
    private void RotateTP()
    {
        float deltaCoef = _deltaTime * 50f;

        // 상하 : TP Rig 회전
        float xRotPrev = Com.tpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // 좌우 : TP Rig 회전
        float yRotPrev = Com.tpRig.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // 상하 회전 가능 여부
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        Vector3 nextRot = new Vector3
        (
            xRotatable ? xRotNext : xRotPrev,
            yRotNext,
            0f
        );

        // TP Rig 회전 적용
        Com.tpRig.localEulerAngles = nextRot;
    }

    [PunRPC]
    /// <summary> 3인칭일 경우 FP Root 회전 </summary>
    private void RotateFPRoot()
    {
        if (State.isMoving == false) return;

        Vector3 dir = Com.tpRig.TransformDirection(_moveDir);
        float currentY = Com.fpRoot.localEulerAngles.y;
        float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        if (nextY - currentY > 180f) nextY -= 360f;
        else if (currentY - nextY > 180f) nextY += 360f;

        Com.fpRoot.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);
    }

    private void Move()
    {
        // 이동하지 않는 경우, 미끄럼 방지
        if (State.isMoving == false)
        {
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        }

        // 실제 이동 벡터 계산
        // 1인칭
        if (State.isCurrentFp)
        {
            _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        }
        // 3인칭
        else
        {
            _worldMove = Com.tpRig.TransformDirection(_moveDir);
        }
        //_worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1.5f);
        if (!PMgr.isRunning && !PMgr.isSittingMove)
        {
            _worldMove *= (MoveOption.speed) * 1.5f;
        }
        else if (PMgr.isRunning)
        {
            _worldMove *= (MoveOption.speed) * 2.3f;
        }
        else if (PMgr.isSittingMove)
        {
            _worldMove *= (MoveOption.speed) * 0.5f;
        }

        // Y축 속도는 유지하면서 XZ평면 이동
        Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
    }
    private void FixedUpdate()
    {
        _deltaTime = Time.deltaTime;

        //ShowCursorToggle();
        if (PV.IsMine && !PLive.isDead)
        {
            //if (CanMove == false) return;
            SetValuesByKeyInput();

            TpCameraZoom();

            if (!PMgr.isStanding)
            {
                Move();
            }

            if (!PMgr.isRollingLast)
            {
                Rotate();
            }
        }
    }

    public void ToggleCamera()
    {
        StartCoroutine(CameraViewToggle()); 
    }
    private void Update()
    {
        if (Input.GetKeyDown(Key.switchCamera))
        {
            if (PV.IsMine && !PLive.isDead && !PMgr.isGameIn)
            {
                Debug.Log("시점 변화 키 입력");
                StartCoroutine(CameraViewToggle());
            }
        }

        if (PV.IsMine)
        {
            if (SceneManager.GetActiveScene().name == "Area01")
            {
                if (!DollyCam.GetComponent<CameraChange>().cart.enabled)
                {
                    Com.fpCamObject.SetActive(true);
                }
            }


        }

    }

    private void ShowCursorToggle()
    {
        if (Input.GetKeyDown(Key.showCursor))
        {
            Debug.Log("커서 토글 키 입력");
            State.isCursorActive = !State.isCursorActive;
        }
        ShowCursor(State.isCursorActive);
    }

    private void ShowCursor(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void TpCameraZoom()
    {
        if (Mathf.Abs(_currentWheel) < 0.01f) return; // 휠 입력 있어야 가능
        Transform tpCamTr = Com.tpCamera.transform;
        Transform tpCamRig = Com.tpRig;

        float zoom = _deltaTime * CamOption.zoomSpeed;
        float currentCamToRigDist = Vector3.Distance(tpCamTr.position, tpCamRig.position);
        Vector3 move = Vector3.forward * zoom * _currentWheel * 10f;

        // Zoom In
        if (_currentWheel > 0.01f)
        {
            if (_tpCamZoomInitialDistance - currentCamToRigDist < CamOption.zoomInDistance)
            {
                State.isMaxZoomOut = false;
                tpCamTr.Translate(move, Space.Self);
            }

            if (_tpCamZoomInitialDistance - currentCamToRigDist > CamOption.zoomInDistance && State.isCurrentFp == false)
            {
                State.isMaxZoomOut = false;
                State.isCurrentFp = true;
                Com.fpCamObject.SetActive(State.isCurrentFp);
                Com.tpCamObject.SetActive(!State.isCurrentFp);
                Vector3 tpEulerAngle = Com.tpRig.localEulerAngles;
                Com.fpRig.localEulerAngles = Vector3.right * tpEulerAngle.x;
                Com.fpRoot.localEulerAngles = Vector3.up * tpEulerAngle.y;
            }
        }
        // Zoom Out
        else if (_currentWheel < -0.01f)
        {
            if (currentCamToRigDist - _tpCamZoomInitialDistance < CamOption.zoomOutDistance)
            {

                tpCamTr.Translate(move, Space.Self);
            }

            if (_tpCamZoomInitialDistance - currentCamToRigDist > CamOption.zoomInDistance && State.isCurrentFp == true)
            {
                State.isCurrentFp = false;
                Com.fpCamObject.SetActive(State.isCurrentFp);
                Com.tpCamObject.SetActive(!State.isCurrentFp);
                Vector3 newRot = default;
                newRot.x = Com.fpRig.localEulerAngles.x;
                newRot.y = Com.fpRoot.localEulerAngles.y;
                Com.tpRig.localEulerAngles = newRot;
            }

            if (_tpCamZoomInitialDistance - currentCamToRigDist <= CamOption.zoomOutDistance)
            {
                State.isCurrentFp = false;
                State.isMaxZoomOut = true;
            }
        }
    }

    public IEnumerator CameraViewToggle()
    {
        // FP -> TP ZOOM OUT
        if (State.isMaxZoomOut == false && State.isCurrentFp == true)
        {
            while (State.isMaxZoomOut == false)
            {
                yield return new WaitForSeconds(0.003f);
                _currentWheel -= 0.02f;
                if (State.isMaxZoomOut == true)
                    yield return null;
            }
            yield return null;
        }

        // TP -> FP ZOOM IN
        else if ((State.isCurrentFp == false) || (State.isMaxZoomOut == false && State.isCurrentFp == false))
        {
            while (State.isCurrentFp == false)
            {
                yield return new WaitForSeconds(0.003f);
                _currentWheel += 0.02f;
                if (State.isCurrentFp == true)
                    yield return null;
            }
            yield return null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
