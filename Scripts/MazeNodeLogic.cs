using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class MazeNodeLogic : MonoBehaviour
{
    public GameObject cube;
    public GameObject cubeGroup;
    public GameObject targetObject;
    public GameObject startObject;
    public GameObject cubeMover;
    public float CubeSpeed = 0.04f;
    public bool MoveCube = false;
    public bool Heuristic1=false,Heuristic2=true,Heuristic3=false;

    public bool UpdateALL = false;
    
    private bool MapComplete = false;
    void Start()
    {   this.gameObject.GetComponent<MazeLoader>().StartBuildingMaze();
        this.gameObject.GetComponent<NodeCreator>().CreateMazeNodes();
        StartCoroutine("Logic");

    }



    private Node[,] mazeNodes;
    IEnumerator Logic(){
        yield return new WaitForEndOfFrame();
        mazeNodes = this.gameObject.GetComponent<NodeCreator>().mazeNodes;
        this.gameObject.GetComponent<NodeCreator>().SetNodesPassable(); //Set nodes passable and color their gizmos.
        /*Debug.Log("Original: " + mazeNodes[5,5].x+ ", " + mazeNodes[0,0].y);
        var a =this.gameObject.GetComponent<NodeCreator>().GetNeighbours(mazeNodes[0,0]);
        foreach(Node n in a){
            Debug.Log("x,y: " + n.x + ", " + n.y);
        }*/
        MapComplete = true;
        /*var a = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        PathFind(mazeNodes[0,5],mazeNodes[68,98]);
        var b = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Debug.Log("Run Time: "+ (b-a));*/
    }
 
    public List<Node> path = new List<Node>();

    private int openCounter = 0;
    public void PathFind(Node start, Node target){
        path.Clear();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>(); 
        openList.Add(start); //Add Start Node to openList
        while(openList.Count>0){
            Node node = openList[0];
            for(int i = 1; i<openList.Count ; i++){
                if(openList[i].F<node.F || openList[i].F==node.F){
                    if(openList[i].H<node.H){
                        node = openList[i];
                    }
                }
            }
            openList.Remove(node);
            closedList.Add(node);
            if(node == target ){
                //Retrace
                RetracePath(start,target);
                sw.Stop();
                if(sw.ElapsedMilliseconds != 0)
                    Debug.Log("Runtime: " + sw.ElapsedMilliseconds +" ms");
                    Debug.Log("Path Node Count: " + path.Count);
                    Debug.Log("OpenList Op: " + openCounter);
                    openCounter = 0;
                return;
            }
            foreach (Node neighbour in this.gameObject.GetComponent<NodeCreator>().GetNeighbours(node)) {
				if (!neighbour.passable || closedList.Contains(neighbour)) {
					continue;
				}

				float newCostToNeighbour = node.G + Heuristic(node, neighbour);
				if (newCostToNeighbour < neighbour.G || !openList.Contains(neighbour)) {
					neighbour.G = newCostToNeighbour;
					neighbour.H = Heuristic(neighbour, target);
					neighbour.ParentNode = node;

					if (!openList.Contains(neighbour))
						openList.Add(neighbour);
                        openCounter ++;
				}
			}
        }
    }
    void RetracePath(Node startNode, Node endNode) {
		Node currentNode = endNode;
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.ParentNode;
		}
		path.Reverse();
	}
    int counter = 0;
    void Update(){
        if(Input.GetKeyDown(KeyCode.Q) || UpdateALL){
            if(targetObject != null && startObject != null && MapComplete  ){
                PathFind(this.gameObject.GetComponent<NodeCreator>().GetNodeFromRealWorldPosition(startObject.transform.position),this.gameObject.GetComponent<NodeCreator>().GetNodeFromRealWorldPosition(targetObject.transform.position) );
                counter = 0;
            }
        }
        if(MoveCube){
            if(path.Count>1 && counter<path.Count){
                var targetPos = new Vector3(path[counter].x,0,path[counter].y);
                if(Vector3.Distance(targetPos,cubeMover.transform.position) > 0.1f){
                    cubeMover.transform.position = Vector3.MoveTowards(cubeMover.transform.position,targetPos,CubeSpeed*Time.deltaTime);
                }
                else{
                    counter++;
                }
            }
        }



    }
     bool HasMouseMoved()
     {
         return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
     }



    public float Heuristic(Node start,Node goal){

        if(Heuristic1){ // 10 10   5 5   h: 10
            var dx = goal.x - start.x;
            var dy = goal.y - start.y;
            return Mathf.Abs(dx) + Mathf.Abs(dy);
        }
        if(Heuristic2){// 10 10 
            var dstX = Mathf.Abs(start.x - goal.x);
            var dstY = Mathf.Abs(start.y - goal.y);

            if (dstX > dstY)
                return 14*dstY + 10 * (dstX-dstY);
            return 14*dstX + 10 * (dstY-dstX);
        }
        if(Heuristic3){
            var dx = goal.x-start.x;
            var dy = goal.y - start.y;
            return Mathf.Sqrt(Mathf.Pow(Mathf.Abs(dx),2)+Mathf.Pow(Mathf.Abs(dy),2));
            
        }
        return 0f;
	}

    

}
