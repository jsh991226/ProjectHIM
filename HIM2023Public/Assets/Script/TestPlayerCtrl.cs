using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 5.0f; //����Ƽ¯�� �̵��� �ӵ� ����
    public float rotSpeed = 120.0f; //����Ƽ¯�� ȸ���� ȸ�� ����

    private Transform tr; //����Ƽ¯�� transform

    private float h, v; //�����¿� ȭ��ǥ

    private float HP = 100;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>(); //����Ƽ¯�� Transform���� �Ҵ�
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal"); //�¿�,
        v = Input.GetAxis("Vertical");  //������ Ű���� ���� �޾ƿ�

        tr.Rotate(Vector3.up * h * rotSpeed * Time.deltaTime); //�޾ƿ� ����ŭ �̵� 
        tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime); //�޾ƿ� ����ŭ ȸ��

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
