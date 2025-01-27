using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    public void OnHit(float damage, Vector2 textPos);

    public float HitPoints { get; set; }

}
