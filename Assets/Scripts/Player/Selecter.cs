using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selecter : MonoBehaviour
{

    protected GameObject selectedObject = null;
    [SerializeField] float selectSize = 1f;
    protected bool selectingElement = true;
    protected bool selectingTile = false;
    void Start()
    {
        
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject collisionObject = collider.gameObject;
        if (selectingElement && collisionObject.CompareTag("Selectable") || selectingTile && collisionObject.CompareTag("TileSelectable"))
        {
            if (selectedObject == null || Distance2D(gameObject, selectedObject) > Distance2D(gameObject, collisionObject))
            {
                if (selectedObject != null) selectedObject.GetComponent<ISelectable>().Unselect();
                collisionObject.GetComponent<ISelectable>().Select();
                selectedObject = collisionObject;
            }
        }
    }

    float Distance2D(GameObject first, GameObject second) {
        if (second == null) return 0;
        float deltaX = first.transform.position.x - second.transform.position.x;
        float deltaY = first.transform.position.y - second.transform.position.y;
        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    protected void DistanceUnselecter() {
        if (selectSize < Distance2D(gameObject, selectedObject))
        {
            selectedObject.GetComponent<ISelectable>().Unselect();
            selectedObject = null;
        }
    }


    void Update()
    {
        DistanceUnselecter();
    }
}
