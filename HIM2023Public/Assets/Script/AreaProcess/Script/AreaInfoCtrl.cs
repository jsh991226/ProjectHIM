using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaInfoCtrl : MonoBehaviour
{

    private GameObject panelImg;
    private float currentPosX;
    private float currentPosY;
    private float currentPosZ;
    public bool ActiveInfo = false;
    private float resizeTime = 0.5f;
    private float sizeMultiple = 4;
    private bool isSizing = false;
    private float gapY;

    [SerializeField]
    private GameObject disableInfoGroup;
    [SerializeField]
    private GameObject enableInfoGroup;



    private void Awake()
    {
        panelImg = gameObject;
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        currentPosZ = transform.position.z;
        gapY = panelImg.transform.position.y - (Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isSizing == false)
        {
            ToggleInfo();
        }

    }
    



    public void ToggleInfo()
    {
        if (ActiveInfo) //info disable
        {
            ActiveInfo = !ActiveInfo;
            StartCoroutine(downSize(true));
            disableInfoGroup.SetActive(true);
            enableInfoGroup.SetActive(false);
            Cursor.visible = ActiveInfo;
            Cursor.lockState = ActiveInfo ? CursorLockMode.None : CursorLockMode.Locked;


        }
        else
        {
            ActiveInfo = !ActiveInfo;
            StartCoroutine(upSize(true));
            disableInfoGroup.SetActive(false);
            enableInfoGroup.SetActive(true);
            Cursor.visible = ActiveInfo;
            Cursor.lockState = ActiveInfo ? CursorLockMode.None : CursorLockMode.Locked;



        }

    }


    public void SetDownSize()
    {
        panelImg.transform.localScale = new Vector3(1, 1, 1);
        panelImg.transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
        isSizing = false;
        ActiveInfo = false;
        disableInfoGroup.SetActive(true);
        enableInfoGroup.SetActive(false);
    }
    


    public IEnumerator upSize(bool _type)
    {
        isSizing = true;
        float mul = 0;
        while (panelImg.transform.localScale.x <= sizeMultiple)
        {
            mul += Time.deltaTime / resizeTime;
            float resizeNum = 1 + mul * sizeMultiple;
            Vector3 resizeScale = new Vector3(resizeNum, resizeNum, 1);
            panelImg.transform.localScale = resizeScale;
            float tempY = gapY * Time.deltaTime*2;
            panelImg.transform.position = new Vector3(currentPosX, panelImg.transform.position.y-tempY, currentPosZ);
            yield return null;
        }
        panelImg.transform.localScale = new Vector3(sizeMultiple, sizeMultiple, 1);
        isSizing = false;
    }


    public IEnumerator downSize(bool _type)
    {
        isSizing = true;
        float mul = 0;
        while (panelImg.transform.localScale.x >= 1)
        {
            mul += Time.deltaTime / resizeTime;
            float resizeNum = 5-(1 + mul * sizeMultiple);
            Vector3 resizeScale = new Vector3(resizeNum, resizeNum, 1);
            panelImg.transform.localScale = resizeScale;
            float tempY = gapY * Time.deltaTime*2f;
            panelImg.transform.position = new Vector3(currentPosX, panelImg.transform.position.y + tempY, currentPosZ);
            yield return null;
        }
        panelImg.transform.localScale = new Vector3(1, 1, 1);
        panelImg.transform.position = new Vector3(currentPosX, currentPosY, currentPosZ);
        isSizing = false;

    }

}
