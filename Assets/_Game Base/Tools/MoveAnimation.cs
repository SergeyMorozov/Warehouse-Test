using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    public Vector3 Direct;
    
    private void LateUpdate()
    {
        transform.Translate(Direct * Time.deltaTime);
    }

}
