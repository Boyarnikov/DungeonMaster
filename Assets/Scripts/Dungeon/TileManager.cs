using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{

    [SerializeField] Dictionary<Tuple<int, int>, GameObject> tiles;
    [SerializeField] List<List<GameObject>> possibleTiles;
    [SerializeField] int size = 10;
    [SerializeField] GameObject tileObject;
    [SerializeField] GameObject possibleTileObject;
    [SerializeField] Vector3 xOffset, yOffset;
    [SerializeField] bool reload = false;

    Vector2 clickedCell;

    // Start is called before the first frame update
    void Start()
    {
        possibleTiles = new List<List<GameObject>>();
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        InicialazeBoard();
        RebuildExternalCollider(gameObject, tiles);
    }

    void InicialazeBoard()
    {
        DeleteBoard();
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        for (int i = 0; i < size; i++) {
            possibleTiles.Add(new List<GameObject>());
            for (int j = 0; j < size; j++)
            {

                GameObject tile = Instantiate(possibleTileObject, transform.position + i * xOffset + j * yOffset, Quaternion.identity);
                tile.transform.parent = transform;
                possibleTiles[i].Add(tile);
                PossibleCell ps = tile.GetComponent<PossibleCell>();
                if (ps != null)
                {
                    ps.coordinates = new Vector2(i, j);
                    ps.tileManager = this;
                }

                if (UnityEngine.Random.Range(0, 3) != 0) { 
                    tile = Instantiate(tileObject, transform.position + i * xOffset + j * yOffset, Quaternion.identity);
                    tile.transform.parent = transform;
                    tiles.Add(new Tuple<int, int>(i, j), tile);
                }
            }
        }
    }

    void DeleteBoard()
    {
        foreach (var tile in tiles) {
            Destroy(tile.Value);
        }
        foreach (var tileRow in possibleTiles)
            foreach (var tile in tileRow)
            {
                Destroy(tile);
            }
    }

    void RebuildExternalCollider(GameObject collider, Dictionary<Tuple<int, int>, GameObject> tiles) {
        Debug.Log("RebuildExternalCollider");
        Destroy(collider.GetComponent<PolygonCollider2D>());
        PolygonCollider2D component = collider.AddComponent<PolygonCollider2D>();
        Vector2[] main = new Vector2[4];
        main[0] = new Vector2(-1000, -1000);
        main[1] = new Vector2(1000, -1000);
        main[2] = new Vector2(1000, 1000);
        main[3] = new Vector2(-1000, 1000);
        component.pathCount = tiles.Count + 1;
        component.SetPath(0, main);
        int iter = 1;
        foreach (var tile in tiles) {
            Vector2 place = new Vector2(tile.Value.transform.position.x - transform.position.x, tile.Value.transform.position.y - transform.position.y);
            Vector2[] path = tile.Value.GetComponent<PolygonCollider2D>().GetPath(0);
            for (int i = 0; i < path.Count(); i++) {
                path[i] += place;
            }
            component.SetPath(iter, path);
            iter++;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (reload)
        {
            InicialazeBoard();
            RebuildExternalCollider(gameObject, tiles);
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
