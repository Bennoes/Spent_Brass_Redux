using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Key List", menuName = "Tactics/Key List")]
public class TacticsKeyList : ScriptableObject
{
    public List<TacticsKey> Keys = new List<TacticsKey>();
}
