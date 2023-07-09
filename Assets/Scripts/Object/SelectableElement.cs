using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableElement : MonoBehaviour, ISelectable
{

    [SerializeField] SpriteRenderer spriteRenderer;
    public bool movable = false;
    [SerializeField] bool selected;

    void Start()
    {
        selected = false;
    }

    public void Select() {
        selected = true;
        spriteRenderer.material.SetColor("_BorderColor", Color.white);
    }

    public void Unselect() {
        selected = false;
        spriteRenderer.material.SetColor("_BorderColor", Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
