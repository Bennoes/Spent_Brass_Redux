using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AtlusNode
{
    //represents a square on the map - contains info about accessability etc

    public bool permanentInaccessable;
    public Vector2 worldCoordintates;
    public Vector2 arrayCoordinates;

    public List<AtlusNode> neighbours = new List<AtlusNode>();

    public TileBase thisTile;
    public Tilemap tileMapUsed;

    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    public AtlusNode parentNode;

    public AtlusNode()
    {
        
    }
    
    
}


