using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToNull : MonoBehaviour
{
    public void SetParentNull()
    {
        transform.parent = null;
    }
}
