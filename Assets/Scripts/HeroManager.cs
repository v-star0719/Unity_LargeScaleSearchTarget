using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }
    public Hero heroPrefab;
    public int count;
    public float attackRange = 20;

    public List<Hero> Heros { get; private set; } = new List<Hero>();

    public void Awake()
    {
        Instance = this;
        heroPrefab.gameObject.SetActive(false);
    }

    public void Update()
    {
        for (int i = Heros.Count; i < count; i++)
        {
            CreateHero();
        }

        for (int i = Heros.Count; i > count; i--)
        {
            DestroyHero(i - 1);
        }

        float deltaTime = Time.deltaTime;
        foreach (var hero in Heros)
        {
            hero.Tick(deltaTime);
        }
    }

    private void CreateHero()
    {
        var hero = Instantiate(heroPrefab);
        hero.gameObject.SetActive(true);
        hero.transform.parent = transform;
        hero.transform.position = Stage.Instance.GetRandomPos();
        Heros.Add(hero);
    }

    private void DestroyHero(int i)
    {
        Object.Destroy(Heros[i].gameObject);
        Heros.RemoveAt(i);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
