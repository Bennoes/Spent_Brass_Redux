using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusControl : MonoBehaviour
{

    public GameObject player;
    public GameObject mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = (player.transform.position + mousePosition.transform.position) / 2;
    }
}
