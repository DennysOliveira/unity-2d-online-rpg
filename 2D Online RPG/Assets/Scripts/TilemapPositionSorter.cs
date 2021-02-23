using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPositionSorter : MonoBehaviour
{
    TilemapCollider2D mapCollider;
    Tilemap tilemap;
    private Renderer myRenderer;

    void Awake()
    {
        mapCollider = GetComponent<TilemapCollider2D>();
        tilemap = GetComponent<Tilemap>();
        myRenderer = GetComponent<Renderer>();
    }
    
    void OnCollisionEnter2D(Collision2D player)
    {   
        Debug.Log(player.transform.position);
    }
}
