using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AtlasController : MonoBehaviour
{
    public GameObject testStart;
    public GameObject testEnd;

    public GameObject TileMapParent;
    public AtlusNode[,] gameAtlus;

    private List<Tilemap> tilemaps = new List<Tilemap>();

    private int atlusSizeX;
    private int atlusSizeY;
    private Vector2 originAdjustment;

    // Start is called before the first frame update
    void Start()
    {
        //populate tileMaps list
        GetTileMaps();

        //calculate world size
        CalculateWorldSize();

        CalculateOriginAdjustment();

        PopulateGameAtlus();

        //Debug.Log("atlus size " + gameAtlus.Length);
        //Debug.Log("array dims " + gameAtlus.GetLength(0) + "," + gameAtlus.GetLength(1));

        SetUpNeighbours();

    }

    public AtlusNode GetNodeAtPoint(Vector2 point)
    {
        Vector2 arrayPoint;
        arrayPoint.x = point.x - originAdjustment.x;
        arrayPoint.y = point.y - originAdjustment.y;

        Vector2Int arrayPointInt = new((int)arrayPoint.x, (int)arrayPoint.y);

        AtlusNode node = gameAtlus[arrayPointInt.x, arrayPointInt.y];

        return node;
    }

    //gets a random viable node in range
    public List<AtlusNode> GetNodesInRange(Vector2 centre, int siteRange, GameObject gameObject)
    {
        List<AtlusNode> nodesInRange = new List<AtlusNode>();

        AtlusNode currentNode = GetNodeAtPoint(gameObject.transform.position);

        float currentX = currentNode.arrayCoordinates.x;
        float currentY = currentNode.arrayCoordinates.y;

        int arrayXmax = gameAtlus.GetLength(0);
        int arrayYmax = gameAtlus.GetLength(1);

        for (int x = (int)currentX - (int)siteRange; x < currentX + siteRange; x++)
        {
            for (int y = (int)currentY - (int)siteRange; y < currentY + siteRange; y++)
            {
                //Debug.Log(x + " and " + y);
                if (x < 0 || y < 0) continue;

                if (x >= arrayXmax || y >= arrayYmax) continue;

                if (gameAtlus[x, y] == null) continue;

                if (gameAtlus[x, y].permanentInaccessable) continue;

                nodesInRange.Add(gameAtlus[x, y]);
            }
        }

        return nodesInRange;
    }


    public bool IsInBounds(Vector2Int coords)
    {
        int width = gameAtlus.GetLength(0);
        int height = gameAtlus.GetLength(1);

        return coords.x >= 0 && coords.y >= 0 && coords.x < width && coords.y < height;
    }



    //need to sort issue with multiple tile maps
    private void GetTileMaps()
    {
        int numberOfChildren = TileMapParent.transform.childCount;

        for (int i = 0; i < numberOfChildren; i++)
        {
            GameObject checkChild = TileMapParent.transform.GetChild(i).gameObject;

            Tilemap tempTileMap = checkChild.GetComponent<Tilemap>();

            if(tempTileMap != null)
            {
                tilemaps.Add(tempTileMap);

                tempTileMap.CompressBounds();
                //Debug.Log("addded " +  tempTileMap.name + " to list");

                
            }
            else
            {
                Debug.Log("No tile maps on checked game object");
            }

        }

    }


    private void CalculateWorldSize()
    {
        atlusSizeX = int.MinValue;
        atlusSizeY = int.MinValue;

        foreach (Tilemap tilemap in tilemaps)
        {
            if(tilemap.size.x > atlusSizeX)
            {
                atlusSizeX = tilemap.size.x;
            }

            if(tilemap.size.y > atlusSizeY)
            {
                atlusSizeY = tilemap.size.y;
            }
;
        }
        
    }

    private void CalculateOriginAdjustment()
    {


        Vector2 checkVector = Vector2.positiveInfinity;

        foreach (Tilemap tilemap in tilemaps)
        {
            //Debug.Log(tilemap.name);
            if((Vector2Int)tilemap.size == Vector2Int.zero)
            {
                //Debug.Log("tile map is empty");
                continue;
            }
            checkVector.x = Mathf.Min(checkVector.x,tilemap.origin.x);
            checkVector.y = Mathf.Min(checkVector.y,tilemap.origin.y); 
        }

        //Debug.Log("Lowest vector " + checkVector);

        originAdjustment = checkVector;
    }

    private void PopulateGameAtlus()
    {
        
        gameAtlus = new AtlusNode[atlusSizeX, atlusSizeY];

        for (int i = 0;i < gameAtlus.GetLength(0) ; i++)
        {
            for (int j = 0; j < gameAtlus.GetLength(1); j++)
            {
                gameAtlus[i, j] = new AtlusNode();

                SetUpTileAtPoint(i, j);               
                                   
            }     
        }
    }

    private void SetUpTileAtPoint(int x, int y)
    {
        //get coords from array and adjust for offset
        Vector2 tileCoord = new Vector2(x + originAdjustment.x, y + originAdjustment.y);

        Vector3Int tileCoordInt = new Vector3Int((int)tileCoord.x, (int)tileCoord.y,0);
        foreach (Tilemap tilemap in tilemaps)
        {
            
            TileBase thisTileBase = tilemap.GetTile(tileCoordInt);
            AtlusNode thisAtlusNode = gameAtlus[x, y];

            switch (tilemap.name)
            {
                case "Ground":

                    if (thisTileBase != null)
                    {
                        thisAtlusNode.thisTile = thisTileBase;
                        thisAtlusNode.worldCoordintates = tileCoord;
                        thisAtlusNode.tileMapUsed = tilemap;
                        thisAtlusNode.arrayCoordinates = new(x, y);

                        CheckTileAtPoint(tileCoord, thisAtlusNode);


                    }
                    else
                    {
                        //Debug.Log("no tile here " + x + "," + y);
                        thisAtlusNode.permanentInaccessable = true;

                    }



                    break;
                case "Collisions":

                    if(thisTileBase != null)
                    {
                        thisAtlusNode.permanentInaccessable = true;
                    }


                    break;

                case "Walk Behind":

                   // Debug.Log("walk behind tile detected");

                    break;

                default:
                    break;
            }

            
        
        }


    }

    private void CheckTileAtPoint(Vector2 point, AtlusNode node)
    {
        //fire ray down to get collider and check for details
        Vector2 middleOfTile = new(point.x +0.5f, point.y + 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(middleOfTile, Vector2.zero, Mathf.Infinity);
        
        if(hit.collider != null && hit.collider.tag == "Obstacle")
        {
            Debug.Log(hit.collider.gameObject.name);
            node.permanentInaccessable = true;
        }
    }

    private void UpdateNodeProperties()
    {
        //check and assign details
    }

    private void SetUpNeighbours()
    {
        foreach (var centreNode in gameAtlus)
        {          
            if (centreNode != null)
            {
                Vector2 nodeCords = centreNode.arrayCoordinates;
                //itterate through 3*3 square:
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if(i == 0 && j == 0) continue;
                                     

                        Vector2Int neighbourCoord = new((int)centreNode.arrayCoordinates.x + i, (int)centreNode.arrayCoordinates.y + j);


                        if (neighbourCoord.x < 0 || neighbourCoord.y < 0 || neighbourCoord.x >= gameAtlus.GetLength(0)
                            || neighbourCoord.y >= gameAtlus.GetLength(1))
                        {
                            
                            continue;
                        }
                        else
                        {                          
                            centreNode.neighbours.Add(gameAtlus[neighbourCoord.x, neighbourCoord.y]);
                        }

                    }
                }        

            }

        }        
    }

    //public methods

    
    
}
