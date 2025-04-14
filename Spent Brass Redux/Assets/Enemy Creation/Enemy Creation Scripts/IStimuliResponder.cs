using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStimuliResponder
{
    public void ReceiveStimulus(Stimulus stimulus);
}

public struct Stimulus
{
    public StimulusType type;

    //cpnstructor

    public Stimulus(StimulusType type)
    {
        this.type = type;
    }
}

public enum StimulusType
{
    TookDamage,
    SpottedPlayer,
    LostPlayer,
    DamagedPlaayer,
    SquadMateDamaged,
    SquaMateKilled,
    SquaLeaderKilled
}
