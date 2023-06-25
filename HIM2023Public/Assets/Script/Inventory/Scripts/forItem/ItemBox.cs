using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public List<GameObject> items;
    List<string> itemName = new List<string>();
    public ItemBoxMove _par;
    Collider rangeCollider;


    private void Start()
    {
        rangeCollider = GetComponent<BoxCollider>();
        foreach(GameObject item in items)
        {
            itemName.Add(item.name);
        }
    }   

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag != "Player") return;
        foreach(string item in itemName)
        {
            PhotonNetwork.Instantiate("Prefab/" + item, Return_RandomPosition(), Quaternion.identity, 0);
        }
        QuestManager _qm = other.gameObject.GetComponent<PlayerManager>().qm; ;
        if(other.gameObject.GetComponent<PhotonView>()) if ((int)_qm.nowQuest == 3) _qm.addCount(1);
        _par.DestroyItemBox();
    }



    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = gameObject.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, 2f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


}
