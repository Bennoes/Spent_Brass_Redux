using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 MousePos2d = cam.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = MousePos2d;
    }
}
