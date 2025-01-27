using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitTextController : MonoBehaviour
{

    public float damage;
    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        int damageInt = (int)damage;
        text.text = damageInt.ToString();
    }

    

}
