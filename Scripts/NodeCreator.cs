using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
    public Node[,] mazeNodes;
    int row;
    int columns;
//Extended Values for Wallsize*row
    int extendedRow;
    int extendedColumn;

    public bool Visualize = false;
    public float VisualizeHeight= 6f;
    public float SphereCheckSize = 0.1f;
    
    private readonly int Wallsize = 10;
    public void CreateMazeNodes(){
        var mazeSize = GetMazeSize();
        try{
            row = mazeSize["row"];
            columns = mazeSize["column"];
            extendedRow = row*Wallsize;
            extendedColumn = columns*Wallsize;
            mazeNodes = new Node[extendedRow,extendedColumn];
            
        }

        catch(KeyNotFoundException e){
            Debug.Log(e.ToString());
            
        }

        for(int i = 0 ; i<extendedRow ; i++){
            for (int j = 0 ; j<extendedColumn ; j++){
                mazeNodes[i,j] = new Node(i-4.5f,j-4.5f);
            }
        }

    }
    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;
				int checkX = Mathf.FloorToInt(node.x+4.5f) + x;
				int checkY = Mathf.FloorToInt(node.y+4.5f) + y;

				if (checkX >= 0 && checkX < extendedRow && checkY >= 0 && checkY < extendedColumn) {
					neighbours.Add(mazeNodes[checkX,checkY]);
				}
			}
		}
        return neighbours;
}



    void OnDrawGizmos() {
        Color a = Color.yellow;
        if(mazeNodes != null && Visualize == true){
            var path = this.gameObject.GetComponent<MazeNodeLogic>().path;
            for(int i = 0; i<extendedRow; i++){
                for(int j = 0 ; j<extendedColumn ; j++){
                    //Debug.Log("x,y:" + mazeNodes[i,j].x + "," + mazeNodes[i,j].y );
                    if(mazeNodes[i,j].passable)
                        a = Color.yellow;
                    else{
                        a = Color.red;
                    }
                    if(path != null && path.Contains(mazeNodes[i,j]) ){
                        a = Color.blue;
                    }

                    Gizmos.color = a;
                    Gizmos.DrawCube(new Vector3(mazeNodes[i,j].x,VisualizeHeight,mazeNodes[i,j].y),Vector3.one);
                    
                }
            }
        }
        
	}

    public void SetNodesPassable(){
        for(int i = 0; i<extendedRow; i++){
                for(int j = 0 ; j<extendedColumn ; j++){
                    Vector3 worldPoint = new Vector3(mazeNodes[i,j].x, Wallsize/2 , mazeNodes[i,j].y);
                    bool walkable = !(Physics.CheckSphere(worldPoint,SphereCheckSize));
                    mazeNodes[i,j].passable = walkable;
                    
                }
            }

    }
    public Node GetNodeFromRealWorldPosition(Vector3 position){
        if(mazeNodes != null){
            return mazeNodes[Mathf.FloorToInt(position.x+4.5f),Mathf.FloorToInt(position.z+4.5f)];
        }
        return null;
    }

    public Dictionary<string,int> GetMazeSize(){
        Dictionary<string,int> mazeSize = new Dictionary<string, int>();
        mazeSize.Add("row",this.gameObject.GetComponent<MazeLoader>().mazeRows);
        mazeSize.Add("column",this.gameObject.GetComponent<MazeLoader>().mazeColumns);
        return mazeSize;
    }

    /*private void FixedUpdate(){
        Node test = GetNodeFromRealWorldPosition(AIAgent.transform.position);
        if(test != null){
            Debug.Log("X,y: " + test.x + "," + test.y + " Passable: " + test.passable);
        }
    }*/
}
