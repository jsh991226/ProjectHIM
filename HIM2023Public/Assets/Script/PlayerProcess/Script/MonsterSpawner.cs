using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    public List<GameObject> enemyType;
    public static MonsterSpawner instance;

    public int howManyEnemy;

    private Queue<GameObject> m_queue = new Queue<GameObject>();
    private List<Vector3> spawnPosition = new List<Vector3>();
    void Start()
    {
        instance = this;

        spawnPosition.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.GetChild(i).transform.position);
            spawnPosition.Add(transform.GetChild(i).transform.position);
        }

        for (int i = 0; i < howManyEnemy; i++)
        {
            int num = Random.Range(0, 4);
            GameObject t_object = Instantiate(enemyType[num]);
            m_queue.Enqueue(t_object);
            Spawn();
        }
    }

    public void InsertQueue(GameObject _deadEnemy)
    {
        m_queue.Enqueue(_deadEnemy);
        _deadEnemy.SetActive(false);
        Spawn();
    }

    public GameObject GetQueue()
    {
        GameObject deadEnemy = m_queue.Dequeue();
        return deadEnemy;
    }

    private void Spawn()
    {   
        GameObject deadEnemy = GetQueue();
        int num = Random.Range(0, 361);
        deadEnemy.transform.localEulerAngles = new Vector3(0, num, 0);
        num = Random.Range(0, spawnPosition.Count);
        deadEnemy.transform.position = new Vector3(spawnPosition[num].x - Random.Range(-0.4f, 0.4f), spawnPosition[num].y - 0.2f, spawnPosition[num].z - Random.Range(-0.4f, 0.4f));
        deadEnemy.SetActive(true);
    }
}
