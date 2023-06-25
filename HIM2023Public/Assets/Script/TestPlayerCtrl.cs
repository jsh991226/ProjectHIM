using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 5.0f; //유니티짱이 이동할 속도 변수
    public float rotSpeed = 120.0f; //유니티짱이 회전할 회전 변수

    private Transform tr; //유니티짱의 transform

    private float h, v; //상하좌우 화살표

    private float HP = 100;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>(); //유니티짱의 Transform변수 할당
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal"); //좌우,
        v = Input.GetAxis("Vertical");  //상하의 키보드 값을 받아옴

        tr.Rotate(Vector3.up * h * rotSpeed * Time.deltaTime); //받아온 값만큼 이동 
        tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime); //받아온 값만큼 회전

        if (Input.GetMouseButtonDown(0))
        {

        }
    }

    public void TakeDamege(float Damage)
    {
        HP -= Damage;
        Debug.Log(HP);
    }
}
