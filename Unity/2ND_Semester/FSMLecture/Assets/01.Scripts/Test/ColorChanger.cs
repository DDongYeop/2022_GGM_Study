using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public SpriteRenderer SpriteRenderer
    {
        get 
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    private void Start() 
    {
        GenerateColor();
    }

    public void GenerateColor()
    {
        SpriteRenderer.color = UnityEngine.Random.ColorHSV();
    }

    public void ResetColor()
    {
        SpriteRenderer.color = Color.white;
    }
}
