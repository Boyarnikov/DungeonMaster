using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomPlacer : MonoBehaviour
{
    List<List<GameObject>> tiles;
    [SerializeField] int size = 10;
    [SerializeField] GameObject tile;
    [SerializeField] Vector3 xOffset, yOffset;

    // Start is called before the first frame update
    void Start()
    {
        InicialazeBoard();
    }

    void InicialazeBoard()
    {
        tiles = new List<List<GameObject>>();
        for (int i = 0; i < size; i++)
        {
            tiles.Add(new List<GameObject>());
            for (int j = 0; j < size; j++)
            {
                GameObject thisTile = Instantiate(tile, transform.position + i * xOffset + j * yOffset, Quaternion.identity);
                thisTile.transform.parent = transform;
                PossibleCell ps = thisTile.GetComponent<PossibleCell>();
                if (ps != null)
                {
                    ps.coordinates = new Vector2(i, j);
                }
                tiles[i].Add(thisTile);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        string[] s = { "PossibleTile" };
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, LayerMask.GetMask(s));
        if (hit.collider != null && hit.collider.tag == "PossibleTile")
        {
            transform.position = hit.collider.transform.position;
        }
        else 
        {
            //transform.position = mousePos;
        }
        transform.position = transform.position.x * Vector3.right +
            transform.position.y * Vector3.up +
            -2 * Vector3.forward;

    }
}
