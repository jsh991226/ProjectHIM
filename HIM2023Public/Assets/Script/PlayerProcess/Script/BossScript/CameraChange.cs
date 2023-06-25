using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class CameraChange : MonoBehaviour
{
    public BossZoneCheck bossZone;
    public BossMoveCtrl bossMove;
    public GameObject MainCamera;
    public GameObject Sun;

    private SunScript sunMove;

    CinemachineVirtualCamera vCam;
    public CinemachineDollyCart cart;

    private int FirstUser;

    public bool DollyCamSet;

    public bool EndBossScene;

    public bool DollyCamEnd;

    private float timer = 0.0f;

    public bool JumpSlow;

    [SerializeField]
    private List<GameObject> MoveObject;
    private List<Vector3> CurrPos = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        sunMove = Sun.GetComponent<SunScript>();
        JumpSlow = false;
        CinemachineTrackedDolly dolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        dolly.m_AutoDolly.m_Enabled = true;
        cart.enabled = false;
        EndBossScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        FirstUser = bossZone.FirstBossZoneIn;
        //DollyCamEnd = bossMove.LoopPatton;

        if(timer > 8.8f)
        {
            DollyCamEnd = true;
        } else
        {
            DollyCamEnd = false;
        }


        if (DollyCamEnd) DollyCamSet = false;

        if (!DollyCamSet && FirstUser > 0)
        {
            var allCams = FindObjectsOfType<Camera>();
            foreach (var cam in allCams)
            {
                cam.gameObject.SetActive(false);
            }
            DollyCamSet = true;


        }

        if (!DollyCamEnd && DollyCamSet)
        {
            MainCamera.SetActive(true);
            cart.enabled = true;

            timer += Time.deltaTime;
            Debug.LogError("실행 된거 맞음 debug : " + MoveObject.Count);
            foreach (GameObject _obj in MoveObject)
            {
                CurrPos.Add(_obj.transform.position);
                _obj.transform.position = new Vector3(10000, 10000, 10000);
            }
            Debug.LogError("실행 된거 맞음 debug : " + CurrPos.Count);
            sunMove.enabled = true;

            //Debug.Log(timer);
            if (timer >= 4.8f && timer < 5.3f)
            {
                cart.m_Speed = 50f;
            }

            if(timer >= 5.3f && timer < 7.0f)
            {
                Debug.Log("점프 느리게 실행");
                JumpSlow = true;
                cart.m_Speed = 3f;
               
            }

            if(timer >= 7.0f && timer <= 7.1f)
            {
                JumpSlow = false;
                Debug.Log("점프 느리게 실행 끝");
                cart.m_Speed = 40f;
            }          
        }
        else
        {
            if (timer > 8f)
            {
                //Debug.Log("연출끗!");
                MainCamera.SetActive(false);
                cart.enabled = false;
                EndBossScene = true;
                int i = 0;
                foreach (GameObject _obj in MoveObject) _obj.transform.position = CurrPos[i++];
            }

        }
    }
}
