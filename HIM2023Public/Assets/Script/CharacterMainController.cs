using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterMainController : MonoBehaviour
{
    /***********************************************************************
    *                               Definitions
    ***********************************************************************/
    #region .
    public enum CameraType { FpCamera, TpCamera };

    [Serializable]
    public class Components
    {
        public Camera tpCamera;
        public Camera fpCamera;

        [HideInInspector] public Transform tpRig;
        [HideInInspector] public Transform fpRoot;
        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject tpCamObject;
        [HideInInspector] public GameObject fpCamObject;

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
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float speed = 3f;
        [Range(1f, 3f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runningCoef = 1.5f;
        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpForce = 5.5f;
    }
    [Serializable]
    public class CameraOption
    {
        [Tooltip("���� ���� �� ī�޶�")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("ī�޶� �����¿� ȸ�� �ӵ�")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("�÷��ٺ��� ���� ����")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("�����ٺ��� ���� ����")]
        public float lookDownDegree = 75f;
        [Range(0f, 3.5f), Space, Tooltip("�� Ȯ�� �ִ� �Ÿ�")]
        public float zoomInDistance = 3f;
        [Range(0f, 5f), Tooltip("�� ��� �ִ� �Ÿ�")]
        public float zoomOutDistance = 3f;
        [Range(1f, 20f), Tooltip("�� �ӵ�")]
        public float zoomSpeed = 10f;
        [Range(0.01f, 0.5f), Tooltip("�� ����")]
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
    public TMP_Text actionText;

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

    /// <summary> TP ī�޶� ~ Rig �ʱ� �Ÿ� </summary>
    private float _tpCamZoomInitialDistance;

    /// <summary> TP ī�޶� �� �Է� �� </summary>
    private float _tpCameraWheelInput = 0;

    /// <summary> ���������� ���� �� �Է� �� </summary>
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

    private Vector3 _moveDir;
    private Vector3 _worldMove;
    private Vector2 _rotation;

    private float _deltaTime;

    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        InitComponents();
        InitSettings();
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
            // ȸ���� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Camera
        var allCams = FindObjectsOfType<Camera>();
        foreach (var cam in allCams)
        {
            cam.gameObject.SetActive(false);
        }
        // ������ ī�޶� �ϳ��� Ȱ��ȭ
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
            Debug.LogError($"{componentName} ������Ʈ�� �ν����Ϳ� �־��ּ���");
    }

    #endregion
    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .

    #endregion

    /// <summary> Ű���� �Է��� ���� �ʵ� �ʱ�ȭ </summary>
    private void SetValuesByKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) v += 1.0f;
        if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
        if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
        if (Input.GetKey(Key.moveRight)) h += 1.0f;

        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.speed); // ����, ����
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.sqrMagnitude > 0.01f;
        State.isRunning = Input.GetKey(Key.run);

        _tpCameraWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        _currentWheel = Mathf.Lerp(_currentWheel, _tpCameraWheelInput, CamOption.zoomAccel);
    }

    /// <summary> 1��Ī ȸ�� </summary>
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

    /// <summary> 1��Ī ȸ�� </summary>
    private void RotateFP()
    {
        float deltaCoef = _deltaTime * 50f;

        // ���� : FP Rig ȸ��
        float xRotPrev = Com.fpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // �¿� : FP Root ȸ��
        float yRotPrev = Com.fpRoot.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // ���� ȸ�� ���� ����
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        // FP Rig ���� ȸ�� ����
        Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // FP Root �¿� ȸ�� ����
        Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
    }

    /// <summary> 3��Ī ȸ�� </summary>
    private void RotateTP()
    {
        float deltaCoef = _deltaTime * 50f;

        // ���� : TP Rig ȸ��
        float xRotPrev = Com.tpRig.localEulerAngles.x;
        float xRotNext = xRotPrev + _rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // �¿� : TP Rig ȸ��
        float yRotPrev = Com.tpRig.localEulerAngles.y;
        float yRotNext =
            yRotPrev + _rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // ���� ȸ�� ���� ����
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        Vector3 nextRot = new Vector3
        (
            xRotatable ? xRotNext : xRotPrev,
            yRotNext,
            0f
        );

        // TP Rig ȸ�� ����
        Com.tpRig.localEulerAngles = nextRot;
    }

    /// <summary> 3��Ī�� ��� FP Root ȸ�� </summary>
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
        // �̵����� �ʴ� ���, �̲�� ����
        if (State.isMoving == false)
        {
            Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
            return;
        }

        // ���� �̵� ���� ���
        // 1��Ī
        if (State.isCurrentFp)
        {
            _worldMove = Com.fpRoot.TransformDirection(_moveDir);
        }
        // 3��Ī
        else
        {
            _worldMove = Com.tpRig.TransformDirection(_moveDir);
        }
        _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);

        // Y�� �ӵ��� �����ϸ鼭 XZ��� �̵�
        Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
    }

    private void Update()
    {
        _deltaTime = Time.deltaTime;
        
        SetValuesByKeyInput();
        Rotate();
        Move();
        TpCameraZoom();

        if (Input.GetKeyDown(Key.switchCamera) && cameraCoroutineRun == false)
        {
            StartCoroutine(CameraViewToggle());
        }

        if (GameObject.Find("Background") == true)
        {
            State.isCursorActive = true;
            ShowCursor(true);
        }
        if (GameObject.Find("Background") == false)
        {
            State.isCursorActive = false; 
            ShowCursor(false);
        }
        if (GameObject.Find("First Person Camera") == false)
        {
            actionText.gameObject.SetActive(false);
            /*ActionController.lookedItem.GetComponent<GroundBag>().watched = false;
            OtherInventory.OtherInventoryClear(); */
        }
        ShowCursorToggle();
        //KeyAction();
    }

    private void ShowCursorToggle()
    {
        if (Input.GetMouseButton(1))
        {
            State.isCursorActive = true;
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
        if (Mathf.Abs(_currentWheel) < 0.01f) return; // �� �Է� �־�� ����
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
                State.isMaxZoomOut = false ;
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
            {                tpCamTr.Translate(move, Space.Self);
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

    bool cameraCoroutineRun = false;
    public IEnumerator CameraViewToggle()
    {
        cameraCoroutineRun = true;

        // FP -> TP ZOOM OUT
        if ( State.isMaxZoomOut == false && State.isCurrentFp == true )
        {
            while (State.isMaxZoomOut == false)
            {
                yield return new WaitForSeconds(0.0005f);
                _currentWheel -= 0.08f;
                if (State.isMaxZoomOut == true)
                    cameraCoroutineRun = false;
                    yield return null;
            }
            cameraCoroutineRun = false;
            yield return null;
        } 

        // TP -> FP ZOOM IN
        else if (( State.isCurrentFp == false) || (State.isMaxZoomOut == false && State.isCurrentFp == false))
        {
            while (State.isCurrentFp == false)
            {
                yield return new WaitForSeconds(0.0005f);
                _currentWheel += 0.08f;
                if (State.isCurrentFp == true)
                    cameraCoroutineRun = false;
                    yield return null;
            }
            cameraCoroutineRun = false;
            yield return null;
        }
    }

    /*private void KeyAction()
    {
        if (actionText.IsActive())
        {
            if (actionText.text == "Take 'E'")
            {
                if (Input.GetKeyDown("e"))
                {
                    if (playerInv.AddWatchingItem() == true)
                        Destroy(ActionController.lookedItem);
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
    }*/
}

