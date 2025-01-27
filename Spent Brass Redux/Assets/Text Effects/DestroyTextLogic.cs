using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTextLogic : MonoBehaviour
{
    public void DestroyTextObject()
    {
        GameObject textParent = this.transform.parent.gameObject;
        Destroy(textParent);
    }

}
