using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;

public class QuestManager : MonoBehaviour
{
    public Text questNameText;
    public Text questDescText;
    public Text questProgText;
    public Image rewardIconImage;
    public Text questRewardText;

    [SerializeField]
    private List<GameObject> QuestReward;
    [SerializeField]
    private Sprite rewardIcon_Money;
    [SerializeField]
    private Sprite rewardIcon_Potion;
    [SerializeField]
    private Sprite rewardIcon_Rune;
    public DBManager dbm;
    public EcoCtrl ecoCtrl;



    public NotifyCtrl notifyCtrl;


    public PanelManager QuestPanel;

    private GameObject PlayerReward;
    private int rewardMoney = 0;
    private UserData userdata;



    public enum QuestType
    {
        KILLMOB,
        KILLPLY,
        KILLBOSS,
        GETITEMBOX,
    }
    public List<string> QuestNPCName;
    public List<string> QuestStr = new List<string>();
    public List<int[]> QuestMaxNum = new List<int[]>();
    public PlayerManager _pm;


    public QuestType nowQuest;
    private int QuestNow = 0;
    private int QuestMax = 0;
    private int QuestRewardIdx;
    bool isFin = false;

    



    


    private void Awake()
    {
        userdata = GameObject.Find("UserData").GetComponent<UserData>();


        //����Ʈ DESC���� �κ�
        QuestStr.Add("�ΰ� �����ϱ�");
        QuestStr.Add("���� �����ϱ�");
        QuestStr.Add("���� óġ�ϱ�");
        QuestStr.Add("���� �����ϱ�");
        //����Ʈ ��ġ ���� �κ�
        QuestMaxNum.Add(SetNumAry(5, 15));
        QuestMaxNum.Add(SetNumAry(1, 2));
        QuestMaxNum.Add(SetNumAry(1, 2));
        QuestMaxNum.Add(SetNumAry(1, 4));
    }

    private int GetRdmQuest()
    {
        Array values = Enum.GetValues(typeof(QuestType));
        QuestType responses = (QuestType)values.GetValue(new System.Random().Next(0, values.Length));
        return (int)responses;
    }
    private int GetRdmNum()
    {
        return UnityEngine.Random.Range(QuestMaxNum[(int)nowQuest][0], QuestMaxNum[(int)nowQuest][1]);
    }
    private string GetRdmNpc()
    {
        return QuestNPCName[UnityEngine.Random.Range(0, QuestNPCName.Count)];
    }

    
    

    private int[] SetNumAry(int _num1, int _num2)
    {
        int[] _return = {_num1, _num2 };
        return _return;
    }

    public void addCount( int _num)
    {
        QuestNow += _num;
        CheckCount();
        GUIRefresh();
    }
    public void ClearQuest()
    {
        QuestNow = QuestMax;
        CheckCount();
        GUIRefresh();
    }

    private void GUIRefresh()
    {
        questDescText.text = QuestStr[(int)nowQuest];
        if (QuestNow < QuestMax) questProgText.text = String.Format("[ {0} / {1} ]", QuestNow, QuestMax);
        else questProgText.text = "[ �Ϸ� ]";

    }
    private void CheckCount()
    {
        if (isFin) return;
        if (QuestNow >= QuestMax)
        {
            isFin = true;
            //���� �ִ� �κ�;
            IInventoryItem requestItem = PlayerReward.GetComponent<GroundItem>().itemDef.CreateInstance();
            if((requestItem as ItemDefinition).Type == ItemType.Money)
            {
                notifyCtrl.CastNotify("������ �Ƿڸ� �Ϸ��Ͽ����ϴ�!, ������ ���޵Ǿ����ϴ�.");
                ecoCtrl.UpdateMoney(rewardMoney);

            }
            else
            {
                notifyCtrl.CastNotify("������ �Ƿڸ� �Ϸ��Ͽ����ϴ�!, ������ ���������� ���޵˴ϴ�.");
                string _sql = "INSERT INTO postitem (ownerid, itemdataname, vaild) VALUES ('" + userdata.Userid + "', '" + (requestItem as ItemDefinition).itemPrefabName + "', '0')";
                dbm.QueryData(_sql);
            }

        }

    }

    private void SetRewardGUI()
    {
        IInventoryItem requestItem = PlayerReward.GetComponent<GroundItem>().itemDef.CreateInstance();
        string _rewardStr;
        if (PlayerReward.GetComponent<GroundItem>().itemDef.Type.ToString() == "Money")
        {
            string[] _temp = (requestItem as ItemDefinition).itemName.Split("-");
            int Money = int.Parse(_temp[1]);
            _rewardStr =Money.ToString("#,##0") + "��";
            rewardMoney = Money;
            rewardIconImage.sprite = rewardIcon_Money;
        }
        else if ((requestItem as ItemDefinition).Type.ToString() == "Rune")
        {
            _rewardStr = (requestItem as ItemDefinition).itemName;
            rewardIconImage.sprite = rewardIcon_Rune;
        }
        else if ((requestItem as ItemDefinition).Type.ToString() == "Potion")
        {
            _rewardStr = (requestItem as ItemDefinition).itemName;
            rewardIconImage.sprite = rewardIcon_Potion;
        }else
        {
            _rewardStr = (requestItem as ItemDefinition).itemName;
        } 
        questRewardText.text = _rewardStr;

    }


    public void StartQuest()
    {
        //nowQuest = (QuestType)GetRdmQuest();
        nowQuest = QuestType.GETITEMBOX; // �̰� �����ϸ� �װŸ� ����, 
        QuestNow = 0;
        QuestMax = GetRdmNum();
        QuestRewardIdx = UnityEngine.Random.Range(0, QuestReward.Count);
        PlayerReward = QuestReward[QuestRewardIdx];
        SetRewardGUI();
        Debug.LogError("���ο� ����Ʈ : " + nowQuest.ToString());
        
        questDescText.text = QuestStr[(int)nowQuest];
        questProgText.text = String.Format("[ {0} / {1} ]", QuestNow, QuestMax);
        questNameText.text = GetRdmNpc() +"�� ������ �Ƿ�";
        QuestPanel.GUIToggle(true);


    }







}
