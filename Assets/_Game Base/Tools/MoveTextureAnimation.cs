using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTextureAnimation : MonoBehaviour
{
    public Vector2 Direct;

    public MeshRenderer MeshRenderer;
    private float _offsetX;
    private float _offsetY;
    
    private void Awake()
    {
        if (MeshRenderer == null)
            MeshRenderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        _offsetX += Time.deltaTime * Direct.x;
        if (_offsetX >= 1) _offsetX -= 1;
        if (_offsetX <= -1) _offsetX += 1;
        
        _offsetY += Time.deltaTime * Direct.y;
        if (_offsetY >= 1) _offsetY -= 1;
        if (_offsetY <= -1) _offsetY += 1;
        
        MeshRenderer.material.SetTextureOffset("_BaseMap", new Vector2(_offsetX, _offsetY));
    }

}
