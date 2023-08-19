using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5;
    public bool IsFinished { get; private set; }

    private Monster target;
    // Start is called before the first frame update
    public void StartFly(Monster target)
    {
        this.target = target;
        IsFinished = false;
    }

    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        var targetPos = target.transform.position;
        Vector3 dir = targetPos - transform.position;
        var delta = speed * deltaTime;
        if (delta >= dir.sqrMagnitude)
        {
            transform.position = targetPos;
            IsFinished = true;
            target.Hit();
        }
        else
        {
            transform.position += dir.normalized * delta;
        }

        if (target.dyTime > 0)
        {
            IsFinished = true;
        }
    }
}
