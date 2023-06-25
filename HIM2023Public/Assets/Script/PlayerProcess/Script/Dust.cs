using Photon.Pun;
using UnityEngine;

public class Dust : MonoBehaviour
{
    public ParticleSystem dust;
    public PhotonView pv;
    Vector3 dustScale;

    private void Start()
    {
        dustScale = new Vector3(dust.transform.localScale.x, dust.transform.localScale.y, dust.transform.localScale.z);
    }

    [PunRPC]
    public void ShowDust(bool _type)
    {
        if (dust.isPlaying == true) return;
        dust.Play();
        if (_type) pv.RPC("ShowDust", RpcTarget.Others, false);
        
    }
    [PunRPC]
    public void StopDust(bool _type)
    {
        dust.Stop();
        if (_type) pv.RPC("StopDust", RpcTarget.Others, false);

    }
    [PunRPC]
    public void SetScale(float _mul, bool _type)
    {
        dust.transform.localScale = new Vector3(dustScale.x + _mul, dustScale.y + _mul, dustScale.z + _mul);
        if (_type) pv.RPC("SetScale", RpcTarget.Others, _mul, false);

    }


}
