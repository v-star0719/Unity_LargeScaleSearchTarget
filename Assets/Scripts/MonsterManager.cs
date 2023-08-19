using System.Collections;
using System.Collections.Generic;
using Kernel.Core;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }
    public Monster monsterPrefab;
    public int count;

    public List<Monster> Monsters { get; private set; } = new List<Monster>();
    private Pool<Monster> pool;
    private List<Monster> dyingMonsters = new List<Monster>();

    void Awake()
    {
        Instance = this;
        monsterPrefab.gameObject.SetActive(false);
        pool = new Pool<Monster>(() => Object.Instantiate(monsterPrefab));
        GetComponentsInChildren(Monsters);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Monsters.Count < count)
        {
            var bornCountPerFrame = Mathf.Max(1, (count - Monsters.Count) / (2 * 30));
            for (int i = 0; i < bornCountPerFrame && Monsters.Count < count; i++)
            {
                SpawnMonster();
            }
        }

        if (Monsters.Count > count)
        {
            var killCountPerFrame = Mathf.Max(5, (Monsters.Count - count) / (2 * 30));
            for (int i = 0; i < killCountPerFrame && Monsters.Count > count; i++)
            {
                OnMonsterDead(Monsters[Monsters.Count - 1]);
            }
        }

        var deltaTime = Time.deltaTime;
        foreach (var monster in Monsters)
        {
            monster.Tick(deltaTime);
        }

        for (int i = dyingMonsters.Count - 1; i >= 0; i--)
        {
            var m = dyingMonsters[i];
            m.dyTime += Time.deltaTime;
            if (m.dyTime > 0.5f)
            {
                pool.Recycle(m);
                dyingMonsters.RemoveAt(i);
            }
        }
    }

    void SpawnMonster()
    {
        var monster = pool.Get();
        monster.transform.parent = transform;
        monster.transform.position = Stage.Instance.GetRandomPos();
        monster.StartRunning();
        monster.gameObject.SetActive(true);
        Monsters.Add(monster);
    }

    public void OnMonsterDead(Monster m)
    {
        m.gameObject.SetActive(false);
        Monsters.Remove(m);
        dyingMonsters.Add(m);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
