using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCollider : MonoBehaviour
{   
    TilemapCollider2D mapCollider;
    Tilemap tilemap;
    ArrayList arr = new ArrayList();
    List<Vector3Int> positions;
    List<Vector3Int> cachedPositions;

    
    [Header("Colors")]
    public Color normalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color fadeColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);


    // Specify layers to be collided with Raycast
    [Header("Fade Layers")]
    public string[] layers = { "Tree-top" };


    Dictionary<string, Vector2> grid = new Dictionary<string, Vector2>(){
        { "top-left", Vector2.left + Vector2.up },      { "top", Vector2.up },          { "top-right", Vector2.up + Vector2.right},
        { "left", Vector2.left },                       { "center", Vector2.zero },     { "right", Vector2.right},
        { "bottom-left", Vector2.left + Vector2.down }, { "bottom", Vector2.down},       { "bottom-right", Vector2.down + Vector2.right}
    };
    

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

        // Raycast around and store it in a List to be iterated
        positions = GetPositionsToFade(player, layers);
        
        cachedPositions = positions;

        // If there are any colliders, output them (TEMP) 
        // Those positions will be implemented to be faded
        if(positions.Count != 0)
        {
            foreach(Vector3 pos in positions)
            {
                Debug.Log("X: " + pos.x + ", Y: " + pos.y);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        positions = GetPositionsToFade(other, layers);
                
        if (positions != cachedPositions){
            // Clean-up old positions from the arr
            ClearMask();

            // Update mask with current values
            UpdateMask();
            cachedPositions = positions;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnTriggerExit2D");

        foreach(Vector3Int tile in arr)  
        {
            tilemap.SetTileFlags(tile, TileFlags.None);
            tilemap.SetColor(tile, normalColor);
        }
    }

    private List<Vector3Int> GetPositionsToFade(Collider2D player, string[] layers)
    {
        List<Vector3Int> list = new List<Vector3Int>();

        int layerMask = LayerMask.GetMask(layers);

        foreach(KeyValuePair<string, Vector2> entry in grid)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position,
                                                 entry.Value,
                                                 2f,
                                                 layerMask);

            if (hit.collider != null)
            {   
                Vector3Int tiledPosition = tilemap.layoutGrid.WorldToCell(player.transform.position + (Vector3)entry.Value);
                list.Add(tiledPosition);
            }
            
        }


        return list;   
    }

    void UpdateMask()
    {
        foreach(Vector3Int tilePosition in positions)
        {
            tilemap.SetTileFlags(tilePosition, TileFlags.None);
            tilemap.SetColor(tilePosition, fadeColor);

            // If current tile being iterated DOESNT exist on the array -> add it
            if(arr.IndexOf(tilePosition) == -1)
            {
                arr.Add(tilePosition);
            }
        }
    }    

    void ClearMask()
    {
        foreach(Vector3Int tile in arr)  
        {
            tilemap.SetTileFlags(tile, TileFlags.None);
            tilemap.SetColor(tile, normalColor);
        }

        arr.Clear();
    }
}
