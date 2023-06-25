using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class ItemBoxMove : MonoBehaviourPunCallbacks
{

    public GameObject boxModel;
    public bool upFlag = false;
    public PhotonView pv;
    public AudioSource audioSource;

    public void DestroyItemBox()
    {
        
        pv.RPC("DestroyItemBoxRPC", RpcTarget.All);
        
    }



    [PunRPC]
    public void DestroyItemBoxRPC()
    {
        openSound();
        Destroy(gameObject);
    }

    private void openSound()
    {
        GameObject openSound = new GameObject("Box Open Sound");
        openSound.transform.position = gameObject.transform.position;
        TemporarySoundPlayer soundPlayer = openSound.AddComponent<TemporarySoundPlayer>();
        soundPlayer.SoundPlay(audioSource.clip, audioSource.minDistance, audioSource.maxDistance);
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 70.0f * Time.deltaTime, 0));
        if (upFlag == false)
        {
            if (boxModel.GetComponent<CapsuleCollider>().height <= 1.8f) boxModel.GetComponent<CapsuleCollider>().height -= 0.01f;
            else boxModel.GetComponent<CapsuleCollider>().height -= 0.03f;
            if (boxModel.GetComponent<CapsuleCollider>().height <= 1.5) upFlag = true;
        }
        else if (upFlag == true)
        {
            if (boxModel.GetComponent<CapsuleCollider>().height >= 2.1) boxModel.GetComponent<CapsuleCollider>().height += 0.01f;
            else boxModel.GetComponent<CapsuleCollider>().height += 0.03f;
            if (boxModel.GetComponent<CapsuleCollider>().height > 2.4) upFlag = false;
        }

    }
}
