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
    List<GameObject> tilesFlat;
    int size = 10;
    [SerializeField] GameObject tileObject;
    [SerializeField] Vector3 x_offset, y_offset;
    [SerializeField] bool reload = false;

    Vector2 clickedCell;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        InicialazeBoard();
    }

    void InicialazeBoard()
    {
        DeleteBoard();
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++)
            {

                GameObject tile = Instantiate(tileObject, i * x_offset + j * y_offset, Quaternion.identity);
                tile.transform.parent = transform;
                tiles.Add(new Tuple<int, int>(i, j), tile);
                PossibleCell ps = tile.GetComponent<PossibleCell>();
                if (ps != null)
                {
                    ps.coordinates = new Vector2(i, j);
                    ps.tileManager = this;
                }
            }
        }

    }

    void DeleteBoard()
    {
        foreach (var tile in tiles) {
            Destroy(tile.Value);
        }
    }

    void RebuildExternalCollider(GameObject collider, Dictionary<Tuple<int, int>, GameObject> tiles) {
        Destroy(collider.GetComponent<PolygonCollider2D>());
        PolygonCollider2D component = collider.AddComponent<PolygonCollider2D>();
        Vector2[] main = new Vector2[4];
        main[0] = new Vector2(-10000, -10000);
        main[1] = new Vector2(10000, -10000);
        main[2] = new Vector2(10000, 10000);
        main[3] = new Vector2(-10000, 10000);
        component.pathCount = tiles.Count + 1;
        component.SetPath(0, main);
        int iter = 1;
        foreach (var tile in tiles) {
            Vector2 place = new Vector2(tile.Value.transform.position.x, tile.Value.transform.position.y);
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
