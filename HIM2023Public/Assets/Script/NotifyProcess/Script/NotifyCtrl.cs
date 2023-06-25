using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyCtrl : MonoBehaviour
{
    public Image notifyImage;
    public Text notifyText;

    private float fadeTime = 0.3f;
    private float notifyTime = 2f;
    private PanelManager pnM;


    public void CastNotify(string _text)
    {
        notifyText.text = _text;
        notifyImage.color = new Color(notifyImage.color.r, notifyImage.color.g, notifyImage.color.b, 0);
        notifyText.color = new Color(notifyText.color.r, notifyText.color.g, notifyText.color.b, 0);
        pnM.GUIToggle(true);
        StartCoroutine(FadeIn(fadeTime));
    }




    private void Start()
    {
        pnM = gameObject.GetComponent<PanelManager>();
    }





    public IEnumerator FadeIn(float time)
    {
        Color color = notifyImage.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime / time;
            notifyImage.color = color;
            notifyText.color = color;
            yield return null;
        }
        StartCoroutine(ShowContent(notifyTime-fadeTime*2));


    }
    public IEnumerator ShowContent(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeOut(fadeTime));
    }



    public IEnumerator FadeOut(float time)
    {
        Color color = notifyImage.color;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / time;
            notifyImage.color = color;
            notifyText.color = color;
            yield return null;

        }
        pnM.GUIToggle(false);


    }



}
