using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    List<List<GameObject>> tiles;
    [SerializeField] 
    int size = 10;
    [SerializeField] GameObject tile;
    [SerializeField] Vector3 xOffset, yOffset;
    [SerializeField] bool reload = false;

    Vector2 clickedCell;

    // Start is called before the first frame update
    void Start()
    {
        InicialazeBoard();
    }

    void InicialazeBoard()
    {
        DeleteBoard();
        tiles = new List<List<GameObject>>();
        for (int i = 0; i < size; i++) {
            tiles.Add(new List<GameObject>());
            for (int j = 0; j < size; j++)
            {
                GameObject thisTile = Instantiate(tile, transform.position + i * xOffset + j * yOffset, Quaternion.identity);
                thisTile.transform.parent = transform;
                PossibleCell ps = thisTile.GetComponent<PossibleCell>();
                if (ps != null)
                {
                    ps.coordinates = new Vector2(i, j);
                    ps.tileManager = this;
                }
                tiles[i].Add(thisTile);
            }
        }

    }

    void DeleteBoard()
    {
        if (tiles != null)
        {
            foreach (var tileRow in tiles)
            {
                foreach (var thisTile in tileRow) {
                    Destroy(thisTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reload)
        {
            InicialazeBoard();
            reload = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.tag == "PossibleTile")
            {
                PossibleCell pc = hit.collider.GetComponent<PossibleCell>();
                Debug.Log(hit.collider.gameObject.name);
                clickedCell = pc.coordinates;
                Debug.Log(clickedCell);
            }
        }
    }
}
