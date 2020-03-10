using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public readonly float x,y;  //Real World Position
    public bool passable;
    public float G { get;  set; }
    public float H { get;  set; }
    public float F { get { return this.G + this.H; } }
    public Node ParentNode{get; set;}

    public Node(float x, float y){
        this.x = x;
        this.y = y;
        this.passable = true;
    }
    public void SetPassable(bool isPassable){
        this.passable = isPassable;
    }

    public void PrintNodeLocation(){
        Debug.Log("x:" +x + " y: " + y + " Passable: " + passable);

    }
}
