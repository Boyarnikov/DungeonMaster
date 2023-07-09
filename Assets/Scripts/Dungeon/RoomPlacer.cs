using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class RoomPlacer : MonoBehaviour
{
    [SerializeField] public Dictionary<Tuple<int, int>, GameObject> tiles;
    [SerializeField] int size = 10;
    [SerializeField] GameObject tileObject;
    [SerializeField] GameObject corridorObject;
    [SerializeField] Vector3 xOffset, yOffset;
    [SerializeField] Vector3 globalOffset;

    [SerializeField] bool construct = false;
    [SerializeField] int sizeX;
    [SerializeField] int sizeY;
    //[SerializeField] public List<>;




    public TileManager tileManager;


    void Start()
    {
        tileManager = GameObject.FindObjectOfType<TileManager>();
        if (construct)
        {
            InicialazeBoard();
        }
        
        globalOffset = size * (xOffset + yOffset) / 4;
    }

    public void InicialazeBoardFromDict(Dictionary<Tuple<int, int>, GameObject> t)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - globalOffset;
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        transform.position = mousePos + Vector3.forward * 2f;

        xOffset = new Vector3(0.5f, 0.25f, 0.01f);
        yOffset = new Vector3(-0.5f, 0.25f, 0.01f);
        int xmin = 100, xmax = 0, ymin = 100, ymax = 0;
        foreach (var key in t.Keys)
        {
            xmin = Math.Min(xmin, key.Item1);
            xmax = Math.Max(xmax, key.Item1);
            ymin = Math.Min(ymin, key.Item2);
            ymax = Math.Max(ymax, key.Item2);
        }

        size = Math.Max(Math.Abs(xmax - xmin), Math.Abs(ymax - ymin));
        Debug.Log(xmin + " " + xmax + " " + ymin + " " + ymax + " " + size.ToString() + " SIZE");
        globalOffset = size * (xOffset + yOffset) / 4;

        tiles = new Dictionary<Tuple<int, int>, GameObject>();

        foreach (var key in t.Keys) {
            int i = key.Item1;
            int j = key.Item2;
            GameObject tile = t[new Tuple<int, int>(i, j)];
            tile.transform.parent = transform;
            tile.transform.position = transform.position + (i - xmin - 1) * xOffset + (j - ymin - 1) * yOffset + globalOffset;

            tiles.Add(new Tuple<int, int>(i - xmin, j - ymin), tile);

            Cell ps = tile.GetComponent<Cell>();
            if (ps != null)
            {
                ps.coordinates = new Vector2(i - xmin, j - ymin);
            }
        }
    }


    void InicialazeBoard()
    {
        if (tileObject == null)
        {
            return;
        }
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if ((i >= 2) && !(((i == 2) && (j == 1)) || ((i == 2) && (j == 2))))
                {
                    GameObject tile = Instantiate(tileObject, transform.position + i * xOffset + j * yOffset + globalOffset, Quaternion.identity);
                    tile.transform.parent = transform;

                    Cell ps = tile.GetComponent<Cell>();
                    if (ps != null)
                    {
                        ps.coordinates = new Vector2(i, j);
                    }
                    tiles.Add(new Tuple<int, int>(i, j), tile);
                }
                else if (((i == 2) && (j == 1)) || ((i == 2) && (j == 2)))
                {
                    GameObject tile = Instantiate(corridorObject, transform.position + i * xOffset + j * yOffset + globalOffset, Quaternion.identity);
                    tile.transform.parent = transform;

                    DirCell ps = tile.GetComponent<DirCell>();
                    if (ps != null)
                    {
                        ps.coordinates = new Vector2(i, j);
                        ps.rot = new Tuple<int, int>(-1, 0);
                    }
                    tiles.Add(new Tuple<int, int>(i, j), tile);
                    { 
                    }
                }
            }
        }
    }


    void RotateBoard()
    {
        Dictionary<Tuple<int, int>, GameObject> tiles_ = new Dictionary<Tuple<int, int>, GameObject>();
        foreach (Tuple<int, int> key in tiles.Keys)
        {
            if (tiles[key].GetComponent<Cell>() != null)
                tiles[key].GetComponent<Cell>().Rotate();
            if (tiles[key].GetComponent<DirCell>() != null)
                tiles[key].GetComponent<DirCell>().Rotate();
            tiles[key].transform.position = transform.position + key.Item2 * xOffset + (size - 1 - key.Item1) * yOffset;
            tiles_[new Tuple<int, int>(key.Item2, size - 1 - key.Item1)] = tiles[key];
        }
        tiles = tiles_;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RotateBoard();
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - globalOffset;
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        string[] s = { "PossibleTile" };
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, LayerMask.GetMask(s));
        if (hit.collider != null && hit.collider.CompareTag("PossibleTile"))
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            Vector2 c = cell.coordinates;
            bool freeSpace = true;
            foreach (var key in tiles.Keys)
            {
                if (key.Item1 + (int)c.x >= tileManager.size || key.Item2 + (int)c.y >= tileManager.size)
                {
                    freeSpace = false; break;
                }
                if (tileManager.tiles.ContainsKey(new Tuple<int, int>(key.Item1 + (int)c.x, key.Item2 + (int)c.y)) &&
                    tileManager.tileType[key.Item1 + (int)c.x][key.Item2 + (int)c.y] == 0)
                {
                    freeSpace = false; break;
                }
            }
            if (freeSpace) {
                transform.position = hit.collider.transform.position - Vector3.forward * 0.009f;
                if (Input.GetMouseButtonDown(0))
                {
                    tileManager.PlaceRoom(tiles, (int)c.x, (int)c.y);
                    Destroy(gameObject);
                }
            }
            else
            {
                transform.position = mousePos + Vector3.forward * 2f;
            }
        }
        else 
        {
            transform.position = mousePos + Vector3.forward * 2f;
        }
    }
}
