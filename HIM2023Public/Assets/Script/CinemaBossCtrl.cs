using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaBossCtrl : MonoBehaviour
{
    private Rigidbody rigd;

    private Animator animator;

    private float timer;

    private PlayerSoundManager psm;

    // Start is called before the first frame update
    void Start()
    {
        rigd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        timer = 0;
        psm = GetComponent<PlayerSoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            BossPatton();
        }
    }

    void BossPatton()
    {
        //Debug.Log(timer);
        timer += Time.deltaTime;
        if (timer <= 5f)
        {
            rigd.AddForce(Vector3.down * 2812.56f, ForceMode.Force);
        }
    }

    public void CinematicCameraMove()
    {
        Debug.Log("CinematicCameraMove");
        psm.PlaySound3D("Cinematic Camera Move");
    }
    public void CinematicDustWind()
    {
        Debug.Log("CinematicDustWind");
        psm.PlaySound3D("Cinematic Dust Wind");
    }
    public void CinematicFall()
    {
        Debug.Log("CinematicFall");
        psm.PlaySound3D("Cinematic Fall");
    }
    public void CinematicFalling()
    {
        Debug.Log("CinematicFalling");
        psm.PlaySound3D("CinematicFalling");
    }
    /*public void CinematicWind()
    {
        psm.PlaySound3D("Cinematic Wind");
    }*/

}