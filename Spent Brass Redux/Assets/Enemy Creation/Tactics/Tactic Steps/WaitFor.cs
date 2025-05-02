using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitFor : ITacticalStep
{
    public EnemyBaseControl EnemyBase { get; set; }

    //context holds info that step might need
    public TacticContext Context { get; set; }



    float time = 5;

    public void InitialiseStep(TacticContext context)
    {

    }

    public bool Execute()
    {
        time -= Time.deltaTime;
        //Debug.Log("wait for " + time);
        if (time <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
