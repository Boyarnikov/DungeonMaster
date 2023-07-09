using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTile : MonoBehaviour, ISelectable
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool movable = false;
    [SerializeField] bool selected;

    void Start()
    {
        selected = false;
    }

    public void Select()
    {
        selected = true;
        spriteRenderer.material.SetFloat("_Value", 0.1f);
    }

    public void Unselect()
    {
        selected = false;
        spriteRenderer.material.SetFloat("_Value", 0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
