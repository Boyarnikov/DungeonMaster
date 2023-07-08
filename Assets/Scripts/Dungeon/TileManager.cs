using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    List<List<GameObject>> tiles;
    int size = 10;
    [SerializeField] GameObject tile;
    [SerializeField] Vector3 x_offset, y_offset;
    [SerializeField] bool reload = false;
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
                GameObject this_tile = Instantiate(tile, i * x_offset + j * y_offset, Quaternion.identity);
                this_tile.transform.parent = transform;
                tiles[i].Add(this_tile);
            }
        }

    }

    void DeleteBoard()
    {
        if (tiles != null)
        {
            foreach (var tile_row in tiles)
            {
                foreach (var this_tile in tile_row) {
                    Destroy(this_tile);
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
    }
}
