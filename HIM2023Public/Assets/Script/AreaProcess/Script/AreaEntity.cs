using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaEntity : MonoBehaviour
{
    [Header("Area Options")]
    [SerializeField]
    private int id;
    [SerializeField]
    private string title;
    [SerializeField]
    [TextArea(3, 10)]
    private string desc;
    [SerializeField]
    private int level;
    [SerializeField]
    private int peopleNow;
    [SerializeField]
    private int peopleMin;
    [SerializeField]
    private int peopleMax;
    [SerializeField]
    private Sprite titleImage;
    [SerializeField]
    private string gameScene;
    [SerializeField]
    private string owner;

    [SerializeField]
    private List<string> playerList = new List<string>();
    [SerializeField]
    public List<string> readyPlayer = new List<string>();

    public void AddPlayer(string _nick)
    {
        PeopleNow++;
        playerList.Add(_nick);
    }
    public void RemovePlayer(string _nick)
    {
        PeopleNow--;
        playerList.Remove(_nick);
    }


    public int Id { get => id; set => id = value; }
    public string Title { get => title; set => title = value; }
    public string Desc { get => desc; set => desc = value; }
    public int Level { get => level; set => level = value; }
    public int PeopleMin { get => peopleMin; set => peopleMin = value; }
    public int PeopleNow { get => peopleNow; set => peopleNow = value; }
    public int PeopleMax { get => peopleMax; set => peopleMax = value; }
    public Sprite TitleImage { get => titleImage; set => titleImage = value; }
    public string GameScene { get => gameScene; set => gameScene = value; }
    public string Owner { get => owner; set => owner = value; }
    public List<string> PlayerList { get => playerList; set => playerList = value; }




}
