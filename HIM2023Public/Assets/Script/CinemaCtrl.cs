using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityStandardAssets.Cameras;
using UnityEngine.Animations;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class CinemaCtrl : MonoBehaviour
{
    public float timer;

    [SerializeField]
    private ArenaBossZoneCheck arenaZone;

    [SerializeField]
    private GameObject mainCamera;

    [SerializeField]
    private GameObject vCam1;
    [SerializeField]
    private GameObject vCam2;
    [SerializeField]
    private GameObject vCam3;
    [SerializeField]
    private GameObject vCam4;

    [SerializeField]
    private GameObject PlayerCam;

    [SerializeField]
    private GameObject MinimapCamera;

    public GameObject playerFPC;

    [SerializeField]
    private GameObject ActionArenaBoss;
    [SerializeField]
    private GameObject RealArenaBoss;

    [SerializeField]
    private GameObject FirstDust;
    [SerializeField]
    private GameObject DualDust;

    [SerializeField]
    private PostProcessVolume post;

    [SerializeField]
    private CinemachineDollyCart oneDollyCart;

    [SerializeField]
    private CinemachineDollyCart TwoDollyCart;

    [SerializeField]
    private CinemachineDollyCart ThreeDollyCart;

    [SerializeField]
    private CinemachineDollyCart FoDollyCart;

    [SerializeField]
    private CinemachineImpulseSource _source;

    public LivingEntity localPlayer;

    public bool isBossShout;

    public AudioSource BGM;
    public AudioClip bossBGM;

    private bool isPlayerHere;

    public bool isArenaBossCinemaEnd;

    private List<bool> StartChecker = new List<bool>();
    private bool isArenaCamSet;

    private CinemaBossCtrl cbc;

    private AudioSource audioSource;

    [SerializeField]
    private List<GameObject> MoveObject;

    private List<Vector3> CurrPos = new List<Vector3>();

    private int startCheck;
    // Start is called before the first frame update 
    void Start()
    {
        timer = 0f;
        isBossShout = false;
        isArenaBossCinemaEnd = false;
        isArenaCamSet = false;
        cbc = ActionArenaBoss.GetComponent<CinemaBossCtrl>();
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < 20; i++) StartChecker.Add(false);
    }

    void Update()
    {
        isPlayerHere = arenaZone.isPlayerCheck;

        if (isPlayerHere)
        {
            CinemaMove();
        }
    }

    void CinemaMove()
    {
        if (!isArenaCamSet)
        {
            var allCams = FindObjectsOfType<Camera>();
            foreach (var cam in allCams)
            {
                cam.gameObject.SetActive(false);
            }
            isArenaCamSet = true;
        }

        if (!StartChecker[3])
        {
            mainCamera.SetActive(true);
            vCam1.SetActive(true);
            BGM.Stop();
            BGM.clip = bossBGM;
            BGM.Play();
            BGM.volume = 0.1f;
            StartChecker[3] = true;

        }
        if (!StartChecker[10])
        {
            oneDollyCart.enabled = true;
            foreach (GameObject _obj in MoveObject)
            {
                CurrPos.Add(_obj.transform.position);
                _obj.transform.position = new Vector3(10000, 10000, 10000);
            }
            localPlayer.isCinema = true;
            StartChecker[10] = true;
        }

  

        startCheck++;
        if (startCheck == 1)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        timer += Time.deltaTime;


        if (timer > 4.7f)
        {
            TwoDollyCart.enabled = true;
            vCam2.SetActive(true);
        }

        if (timer > 5f)
        {
            vCam1.SetActive(false);

        }

        if (timer > 10f)
        {
            post.enabled = true;
            vCam2.SetActive(false);
            vCam3.SetActive(true);
        }

        if (timer > 12.4f)
            ActionArenaBoss.SetActive(true);

        if (timer > 12.7f && !StartChecker[0])
        {
            BGM.volume = 0.15f;

            cbc.CinematicFalling();
            StartChecker[0] = true;

        }

        if (timer >= 13.4f)
        {
            if (timer >= 13.4f && !StartChecker[1])
            {
                StartChecker[1] = true;
                Debug.Log("cbc.CinematicFall();");
                cbc.CinematicFall();
                BGM.volume = 0.17f;

            }

            //ActionArenaBoss.SetActive(false);
            //DoImpulse(true);
            ThreeDollyCart.enabled = true;
            FirstDust.SetActive(true);
        }

        if (timer >= 14f)
        {
            RealArenaBoss.SetActive(true);
            if ((int)timer == 14 && !StartChecker[2])
            {
                StartChecker[2] = true;
                BGM.volume = 0.2f;

                Debug.Log("cbc.CinematicDustWind();");
                cbc.CinematicDustWind();
            }

        }


        if (timer >= 15f && !StartChecker[4])
        {
            vCam3.SetActive(false);
            vCam4.SetActive(true);
            BGM.volume = 0.23f;
            FoDollyCart.enabled = true;
            StartChecker[4] = true;
        }

        if (timer >= 16f && !StartChecker[5]) { 
            BGM.volume = 0.27f;
            DualDust.SetActive(true);
            StartChecker[5] = true;
        }

        if (timer >= 20f &&!StartChecker[6]) { 
            FoDollyCart.m_Speed = 3f;
            BGM.volume = 0.3f;
            StartChecker[6] = true;
         }

        if (timer >= 23f && !StartChecker[7])
        {
            isBossShout = true;
            StartChecker[7] = true;
        }

        if (timer >= 23.6f && !StartChecker[8])
        {
            FoDollyCart.m_Speed = 40f;
            BGM.volume = 0.35f;
            StartChecker[8] = true; 
        }



        if (timer >= 27f&& !StartChecker[9])
        {
            vCam4.SetActive(false);
            isArenaBossCinemaEnd = true;
            post.enabled = false;
            MinimapCamera.SetActive(true);
            mainCamera.SetActive(false);
            playerFPC.SetActive(true);
            localPlayer.isCinema = false;
            BGM.volume = 0.45f;
            StartChecker[9] = true; 
            int i = 0;
            foreach (GameObject _obj in MoveObject) _obj.transform.position = CurrPos[i++];
        }
    }

    private void DoImpulse(bool _play)
    {
        if (_play)
        {
            _source.GenerateImpulse();
        }
    }

    IEnumerator Dolly3PlusSpeed(float _slowTime)
    {
        float currTime = 0f;

        while (currTime < _slowTime)
        {
            currTime += 0.1f;
            ThreeDollyCart.m_Speed += 1;
            yield return new WaitForSeconds(0.1f);
        }
    }
}