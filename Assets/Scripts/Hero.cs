using System.Collections.Generic;
using Kernel.Core;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float cd = 1;
    public Bullet bulletPrefab;
    public GameObject rangeGo;

    private Pool<Bullet> pool;
    private List<Bullet> bullets = new List<Bullet>();
    private float time;
    public float AttackRange { get; private set; }
    public bool isDestroyed;

    private Monster target;
    private bool hasRequestSearch;

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab.gameObject.SetActive(false);
        pool = new Pool<Bullet>(()=>Instantiate(bulletPrefab));
    }

    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        time += deltaTime;
        if (time >= cd)
        {
            if (!hasRequestSearch)
            {
                SearchCenter.Instance.Request(this);
                hasRequestSearch = true;
            }

            if (target != null)
            {
                time = 0;
                Fire();
            }
        }

        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            bullets[i].Tick(deltaTime);
            if (bullets[i].IsFinished)
            {
                pool.Recycle(bullets[i]);
                bullets[i].gameObject.SetActive(false);
                bullets.RemoveAt(i);
            }
        }

        if (AttackRange != HeroManager.Instance.attackRange)
        {
            AttackRange = HeroManager.Instance.attackRange;
            var f = HeroManager.Instance.attackRange * 2;
            rangeGo.transform.localScale = new Vector3(f, f, f);
        }
    }

    void Fire()
    {
        var bullet = pool.Get();
        bullet.gameObject.SetActive(true);
        bullet.transform.parent = transform;
        bullet.transform.position = transform.position;
        bullet.StartFly(target);
        bullets.Add(bullet);
        target = null;
    }

    public void SetTarget(Monster t)
    {
        target = t;
        hasRequestSearch = false;
    }

    void OnDestroy()
    {
        isDestroyed = true;
    }
}
