using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Selecter
{
    bool moving = false;
    GameObject movingObject = null;
    [SerializeField] GameObject objectPool;
    [SerializeField] MovementSpriteManager movementSpriteManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Take() {
        if (selectedObject != null && selectedObject.GetComponent<SelectableElement>().movable)
        {
            selectingElement = false;
            selectingTile = true;
            moving = true;
            selectedObject.transform.parent.parent = transform;
            movingObject = selectedObject;
            selectedObject.GetComponent<SelectableElement>().Unselect();
            selectedObject.transform.parent.position = transform.position + new Vector3(0f, 0.75f, 0f);
            selectedObject.transform.parent.GetComponent<PolygonCollider2D>().enabled = false;
            movementSpriteManager.flag = true;
        }
    }

    void Drop() {
        if (selectedObject != null)
        {
            selectingTile = false;
            selectingElement = true;
            moving = false ;
            movingObject.transform.parent.parent = selectedObject.transform.parent;
            selectedObject.GetComponent<ISelectable>().Unselect();
            movingObject.transform.parent.position = selectedObject.transform.parent.position + new Vector3(0f, 0.5f, 0f);
            movingObject.transform.parent.GetComponent<PolygonCollider2D>().enabled = true;
            movementSpriteManager.flag = false;
            movingObject = null;
            selectedObject = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DistanceUnselecter();
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!moving) Take();
            else Drop();
        }
    }
}
