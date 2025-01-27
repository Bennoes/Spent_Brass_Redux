using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public void OnHit(float damage);

    public float HitPoints { get; set; }

}
