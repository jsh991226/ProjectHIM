using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{

    private GameObject Sun;

    // Start is called before the first frame update
    void Start()
    {
        Sun = this.gameObject;
        StartCoroutine(CinemaSun());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CinemaSun()
    {
        float timer = 0.01f;
        float SunendTimer = 9f;

        while (timer < SunendTimer)
        {
            timer += 0.04f;
            Sun.transform.Rotate(new Vector3(0, 1f, 0));
            yield return new WaitForSeconds(0.04f);
        }
    }
}
