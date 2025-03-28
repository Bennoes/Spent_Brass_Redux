using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHittable
{
    public event Action OnDeath;
    public void OnHit(float damage, Vector2 textPos);

    public float CurrentArmour { get; set; }

}
