using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomProcess : MonoBehaviour
{
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject btnMgr;
    public bool isActive = false;

    public void EnablePanel()
    {
        roomPanel.SetActive(true);
        gameObject.GetComponent<CreateButtonManager>().GetRoomList();
        isActive = true;
    }

    public void DisablePanel()
    {
        roomPanel.SetActive(false);
    }
}
