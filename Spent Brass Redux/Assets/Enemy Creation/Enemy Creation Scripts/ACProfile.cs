using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new profile", menuName = "AC Profile")]
public class ACProfile : ScriptableObject
{

    public EnemyType enemyType;

    [System.Serializable]
    public struct StimulusResponse
    {
        public StimulusType stimulus;
        public Vector2 acChange;
    }

    public List<StimulusResponse> responses;

    


}


