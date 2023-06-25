using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloods : MonoBehaviour
{
    // Start is called before the first frame update

    public List<ParticleSystem> bloods;
    
    public void ShowBlood()
    {
        int _rdm = Random.Range(0, bloods.Count);
        bloods[_rdm].Play();
    }
    
}
