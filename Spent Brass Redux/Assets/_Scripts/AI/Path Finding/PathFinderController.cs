using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderController : MonoBehaviour
{
  
    public AtlasController atlasController;

    public List<AtlusNode> openNodes;
    public HashSet<AtlusNode> closedNodes;

    private int diagonalScore = 14;
    private int straightScore = 10;

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public List<Vector2> GetPathOfVectors(Vector2 Start, Vector2 end)
    {
        List<AtlusNode> nodePath = PathFinder(Start,end);
        List<Vector2> result = new List<Vector2>();
        nodePath.Reverse();

        foreach (AtlusNode node in nodePath)
        {
            //add 0.5f to vector components so they walk through the middle of tiles
            result.Add(new Vector2( node.worldCoordintates.x + 0.5f, node.worldCoordintates.y + 0.5f));

        }
        
        return result;

    }

    public List<AtlusNode> PathFinder(Vector2 start, Vector2 end)
    {
        //get a reference to node at start and end point
        AtlusNode startNode = atlasController.GetNodeAtPoint(start);

        AtlusNode endNode = atlasController.GetNodeAtPoint(end);

        //check if either point is ok
        if(startNode == null || endNode == null) { Debug.Log("Start or end node was null"); return null; }
        
        openNodes = new List<AtlusNode>();
        closedNodes = new HashSet<AtlusNode>();
        //add start node to OPEN list
        openNodes.Add(startNode);

        while(openNodes.Count > 0)
        {
            //Debug.Log("nodes left in open list " +  openNodes.Count);  
            //set top of list as current node
            AtlusNode currentNode = openNodes[0];

            
            //check that no other node in the list has lower f cost
            for(int i = 1; i < openNodes.Count; i++)
            {
                float nodeFcost = openNodes[i].fCost;
                if (nodeFcost < currentNode.fCost || nodeFcost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost)
                {
                    //found a lower cost node
                    currentNode = openNodes[i];
                    //Debug.Log("cords of node " + currentNode.arrayCoordinates);
                }
            }

            //take current node from open list and add to closed
            closedNodes.Add(currentNode);
            openNodes.Remove(currentNode);
            //check if current node = target node e.g youve reached your destination
            if(currentNode == endNode)
            {
                Debug.Log("reached the end node");
                
                return RetracePath(startNode, endNode);
                //
                //
                
            }

            //check through neighbours. Skip if in closed or inaccessable
            foreach(AtlusNode neighbour in currentNode.neighbours)
            {
                if(neighbour.permanentInaccessable || closedNodes.Contains(neighbour))
                {
                    //discard node from calculation
                    continue;
                }
                //assign costs to neighbours based on moving from start node
                //g is path from start to current
                //h is distance to goal
                //f is g + h
                float costToNeighbour = currentNode.gCost + CalculateH(currentNode,neighbour);
                //updated here
                if(costToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = CalculateH(neighbour,endNode);
                    neighbour.parentNode = currentNode;

                    if(!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                        
                    }
                }

            }

        }
        Debug.Log("no path");
        return null;
    }

    private int CalculateH(AtlusNode A, AtlusNode B)
    {
        int distX = Mathf.Abs((int)A.arrayCoordinates.x - (int)B.arrayCoordinates.x);
        int distY = Mathf.Abs((int)A.arrayCoordinates.y - (int)B.arrayCoordinates.y);

        if(distX > distY)
        {
            return diagonalScore*distY + straightScore * (distX-distY);
        }
        else
        {
            return diagonalScore * distX + straightScore * (distY - distX);
        }

    }

    private List<AtlusNode> RetracePath(AtlusNode startNode, AtlusNode endNode)
    {
        List<AtlusNode> pathNodes = new List<AtlusNode>();
        //work backwards though parent nodes starting at the last node
        AtlusNode thisNode = endNode;
        while(thisNode != startNode)
        {           
            pathNodes.Add(thisNode);
            thisNode = thisNode.parentNode;
            
        }
        return pathNodes;
    }


}
