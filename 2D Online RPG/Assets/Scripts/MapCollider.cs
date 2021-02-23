using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCollider : MonoBehaviour
{   
    TilemapCollider2D mapCollider;
    Tilemap tilemap;
    ArrayList arr = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        mapCollider = GetComponent<TilemapCollider2D>();
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        arr.Clear();
        

    }

    private void OnTriggerStay2D(Collider2D other)
    {

        Vector3Int tilepos = tilemap.layoutGrid.WorldToCell(other.transform.position);
        
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
        tilemap.SetTileFlags(tilepos, TileFlags.None);
        tilemap.SetColor(tilepos, color);
        
        
        if(arr.IndexOf(tilepos) == -1)
        {
            arr.Add(tilepos);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnTriggerExit2D");

        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        foreach(Vector3Int tile in arr)  
        {
            tilemap.SetTileFlags(tile, TileFlags.None);
            tilemap.SetColor(tile, color);
        }
    }
}
