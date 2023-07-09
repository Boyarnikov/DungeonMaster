using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TileManager : MonoBehaviour
{
    [SerializeField] public Dictionary<Tuple<int, int>, GameObject> tiles;
    [SerializeField] List<List<GameObject>> possibleTiles;

    [SerializeField] int coridorSize = 7;

    [SerializeField] List<List<int>> roomId;
    [SerializeField] public List<List<int>> tileType; // -1 - free, 0 - room, 1 - corridor

    [SerializeField] public int size = 10;
    [SerializeField] GameObject tileObject;
    [SerializeField] GameObject possibleTileObject, corridorObject;
    [SerializeField] Vector3 xOffset, yOffset;
    [SerializeField] bool reload = false;
    int roomAmount = 0;



    Vector2 clickedCell;


    void Start()
    {
        possibleTiles = new List<List<GameObject>>();
        roomId = new List<List<int>>();
        tileType = new List<List<int>>();
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        InicialazeBoard();
        RebuildExternalCollider(gameObject, tiles);
    }


    void InicialazeBoard()
    {
        DeleteBoard();
        tiles = new Dictionary<Tuple<int, int>, GameObject>();
        for (int i = 0; i < size; i++) {
            roomId.Add(new List<int>());
            tileType.Add(new List<int>());
            possibleTiles.Add(new List<GameObject>());
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate(possibleTileObject, transform.position + i * xOffset + j * yOffset, Quaternion.identity);
                tile.transform.parent = transform;
                possibleTiles[i].Add(tile);
                Cell ps = tile.GetComponent<Cell>();
                if (ps != null)
                {
                    ps.coordinates = new Vector2(i, j);
                    ps.tileManager = this;
                }

                roomId[i].Add(-1);
                tileType[i].Add(-1);
            }
        }
        roomAmount++;
    }


    GameObject PlaceTile(int x, int y, int id, GameObject t)
    {
        if (id < -1)
        {
            id = roomAmount;
        }
        if (t == null)
        {
            t = tileObject;
        }

        roomId[x][y] = id;
        if (tiles.ContainsKey(new Tuple<int, int>(x, y)))
        {
            Destroy(tiles[new Tuple<int, int>(x, y)]);
            tiles.Remove(new Tuple<int, int>(x, y));
        }

        GameObject tile = Instantiate(t, transform.position + x * xOffset + y * yOffset, Quaternion.identity);
        tile.transform.parent = transform;
        tiles.Add(new Tuple<int, int>(x, y), tile);
        Cell ps = tile.GetComponent<Cell>();
        ps.coordinates = new Vector2(x, y);
        ps.tileManager = this;
        return tile;
    }

    GameObject MoveTile(int x, int y, int id, GameObject t)
    {
        if (id < -1)
        {
            id = roomAmount;
        }

        roomId[x][y] = id;
        if (tiles.ContainsKey(new Tuple<int, int>(x, y)))
        {
            Destroy(tiles[new Tuple<int, int>(x, y)]);
            tiles.Remove(new Tuple<int, int>(x, y));
        }

        GameObject tile = t;
        tile.transform.parent = transform;
        tiles.Add(new Tuple<int, int>(x, y), tile);
        Cell ps = tile.GetComponent<Cell>();
        ps.coordinates = new Vector2(x, y);
        ps.tileManager = this;
        return tile;
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) { return; }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Handles.Label(transform.position + i * xOffset + j * yOffset, tileType[i][j].ToString());
            }
        }
    }


    void RebuildExternalCollider(GameObject collider, Dictionary<Tuple<int, int>, GameObject> tiles) {
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


    public void PlaceRoom(Dictionary<Tuple<int, int>, GameObject> tilesToPlace, int xPosition, int yPosition)
    {
        var k = tilesToPlace.Keys.ToList();
        foreach (var key in k) {
            if (tilesToPlace[key].GetComponent<DirCell>() != null)
            {
                Tuple<int, int> dir = tilesToPlace[key].GetComponent<DirCell>().rot;
                var t = MoveTile(key.Item1 + xPosition, key.Item2 + yPosition, roomAmount, tilesToPlace[key]);
                t.GetComponent<DirCell>().rot = dir;
                tileType[key.Item1 + xPosition][key.Item2 + yPosition] = 0;
                Tuple<int, int> p = new Tuple<int, int>(key.Item1 + xPosition + dir.Item1, key.Item2 + yPosition + dir.Item2);
                int counter = coridorSize;
                while (p.Item1 > 0 && p.Item1 < size && p.Item2 > 0 && p.Item2 < size && tileType[p.Item1][p.Item2] != 0 && counter > 0)
                {
                    counter--;
                    if (tileType[p.Item1][p.Item2] == -1) {
                        PlaceTile(p.Item1, p.Item2, roomAmount, tileObject);
                        tileType[p.Item1][p.Item2] = 1;
                    }
                    p = new Tuple<int, int>(p.Item1 + dir.Item1, p.Item2 + dir.Item2);
                }
            }
            else
            {
                MoveTile(key.Item1 + xPosition, key.Item2 + yPosition, roomAmount, tilesToPlace[key]);
                tileType[key.Item1 + xPosition][key.Item2 + yPosition] = 0;
            }

                
        }
        roomAmount++;
        UpdateCorridors();
        RebuildExternalCollider(gameObject, tiles);
    }


    /*public void PlaceRoom(Dictionary<Tuple<int, int>, GameObject> tilesToPlace, int xPosition, int yPosition)
    {
        var k = tilesToPlace.Keys.ToList();
        foreach (var key in k) {
            if (tilesToPlace[key].GetComponent<DirCell>() != null)
            {
                Tuple<int, int> dir = tilesToPlace[key].GetComponent<DirCell>().rot;
                var t = PlaceTile(key.Item1 + xPosition, key.Item2 + yPosition, roomAmount, corridorObject);
                t.GetComponent<DirCell>().rot = dir;
                tileType[key.Item1 + xPosition][key.Item2 + yPosition] = 0;
                Tuple<int, int> p = new Tuple<int, int>(key.Item1 + xPosition + dir.Item1, key.Item2 + yPosition + dir.Item2);
                int counter = coridorSize;
                while (p.Item1 > 0 && p.Item1 < size && p.Item2 > 0 && p.Item2 < size && tileType[p.Item1][p.Item2] != 0 && counter > 0)
                {
                    counter--;
                    if (tileType[p.Item1][p.Item2] == -1) {
                        PlaceTile(p.Item1, p.Item2, roomAmount, tileObject);
                        tileType[p.Item1][p.Item2] = 1;
                    }
                    p = new Tuple<int, int>(p.Item1 + dir.Item1, p.Item2 + dir.Item2);
                }
            }
            else
            {
                PlaceTile(key.Item1 + xPosition, key.Item2 + yPosition, roomAmount, tileObject);
                tileType[key.Item1 + xPosition][key.Item2 + yPosition] = 0;
            }

                
        }
        roomAmount++;
        UpdateCorridors();
        RebuildExternalCollider(gameObject, tiles);
    }*/


    void UpdateCorridors()
    {
        DeleteCorridors();

        var k = tiles.Keys.ToList();
        foreach (var key in k) {
            if (tiles[key].GetComponent<DirCell>() != null)
            {
                var dir = tiles[key].GetComponent<DirCell>().rot;
                tileType[key.Item1][key.Item2] = 0;
                Tuple<int, int> p = new Tuple<int, int>(key.Item1 + dir.Item1, key.Item2 + dir.Item2);
                int counter = coridorSize;
                while (p.Item1 >= 0 && p.Item1 < size && p.Item2 >= 0 && p.Item2 < size && tileType[p.Item1][p.Item2] != 0 && counter > 0)
                {
                    counter--;
                    if (tileType[p.Item1][p.Item2] == -1)
                    {
                        PlaceTile(p.Item1, p.Item2, roomAmount, tileObject);
                        tileType[p.Item1][p.Item2] = 1;
                    }
                    p = new Tuple<int, int>(p.Item1 + dir.Item1, p.Item2 + dir.Item2);
                }
            }
            
        }
    }

    void DeleteCorridors()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (tileType[i][j] == 1)
                {
                    Destroy(tiles[new Tuple<int, int>(i, j)]);
                    tiles.Remove(new Tuple<int, int>(i, j));
                    tileType[i][j] = -1;
                }
            }
        }

    }


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
                Cell pc = hit.collider.GetComponent<Cell>();
                Debug.Log(hit.collider.gameObject.name);
                clickedCell = pc.coordinates;
                Debug.Log(clickedCell);
            } else if (hit.collider != null && hit.collider.tag == "Tile")
            {
                Cell pc = hit.collider.GetComponent<Cell>();
                Debug.Log(hit.collider.gameObject.name);
                clickedCell = pc.coordinates;
                Debug.Log(clickedCell);
                Debug.Log(tileType[(int)(pc.coordinates.x)][(int)(pc.coordinates.y)]);
                if (tileType[(int)(pc.coordinates.x)][(int)(pc.coordinates.y)] == 0)
                {
                    int id = roomId[(int)(pc.coordinates.x)][(int)(pc.coordinates.y)];
                    Debug.Log("ROOM ID");
                    Debug.Log(id);
                    Dictionary<Tuple<int, int>, GameObject> d = new Dictionary<Tuple<int, int>, GameObject>();

                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j<size; j++)
                        {
                            if (tileType[i][j] == 0 && roomId[i][j] == id)
                            {
                                d.Add(new Tuple<int, int>(i, j), tiles[new Tuple<int, int>(i, j)]);
                                tileType[i][j] = -1;
                                roomId[i][j] = -1;
                            }
                        }
                    }
                    foreach (var k in d.Keys)
                    {
                        tiles.Remove(k);
                    }
                    GameObject room = Instantiate(new GameObject());
                    room.name = "CreateNewRoom";
                    room.AddComponent<RoomPlacer>();
                    room.GetComponent<RoomPlacer>().InicialazeBoardFromDict(d);
                    DeleteCorridors();
                }
                else
                {
                    Debug.Log(tileType);
                }
                //if (tiles[new Tuple<int, int>( (int)(pc.coordinates.x), (int)(pc.coordinates.y))])
            }
        }
    }
}
