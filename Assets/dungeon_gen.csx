using System;
using System.Collections.Generic;
using UnityEngine;
 
public class Vector2
{
    public int x;
    public int z;

    public Vector2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class Cell{
    public Vector2 position;
    public double fcost=double.MaxValue;
    public double gcost=double.MaxValue;
    public double hcost=double.MaxValue;
    public Vector2 connection;
    public bool isWall;

    public int parent_i, parent_j;

    public Cell(Vector2 pos){
        position=pos;
    }
    public Cell(Vector2 pos,double fc){
        position=pos;
        fcost=fc;
    }
}

public class DungeonGenerator : MonoBehaviour
{
static Random random = new Random();

static int RandomRange(int min, int max) {
    return random.Next(min, max);
}

// static public int[,] SpawnDungeon(int _sizeX,int _sizeZ, int _nbrSquareForGeneration)
// {
//     int[,] _dungeonMap; // 2D Array of int that will hold the dungeon map information
 
//     _dungeonMap = CreateDungeonSquares(_sizeX,_sizeZ, _nbrSquareForGeneration); // Fill the array with different squares of various size
//     _nbrRoom = FindRooms (_dungeonMap,_sizeX, _sizeZ); // Loop through the dungeon map and find the square that form rooms together
//     if(_nbrRoom &gt; 1)
//     {
//         _dungeonMap = CreateDungeonHalls(_dungeonMap, _sizeX,_sizeZ,_nbrRoom);  // Add halls between the created room
//     }
 
//     SpawnEnvironment(_dungeonMap, _sizeX, _sizeZ, _nbrRoom); // Create environment(Spider eggs for now)
 
//     _ListOfWallsToCreate = EvaluateWallToBuild(_dungeonMap, _sizeX, _sizeZ); // Loop through the dungeon map to find all the wall that need to be created
//     InstantiateDungeonWalls(_ListOfWallsToCreate); // Instantiate all the wall found by the EvaluateWallToBuild function
 
//     return _dungeonMap;
// }using System;

// Random instance for generating random numbers

// Function to create a dungeon grid
static int[,] CreateDungeonSquares(int _sizeX, int _sizeZ, int _squareNbr)
{
    int[,] _newMap = new int[_sizeX, _sizeZ];

    for (int i = 0; i < _squareNbr; i++)
    {
        // Calculate random room size and position. Ensure the room is inside the map.
        int _roomSizeX = random.Next(10, _sizeX / 5);
        int _roomSizeZ = random.Next(10, _sizeZ / 5);
        int _roomPosX  = random.Next(1, _sizeX - _roomSizeX);
        int _roomPosZ  = random.Next(1, _sizeZ - _roomSizeZ);

        for (int j = _roomPosX; j < _roomPosX + _roomSizeX; j++)
        {
            for (int k = _roomPosZ; k < _roomPosZ + _roomSizeZ; k++)
            {
                _newMap[j, k] = 1;
            }
        }
    }
    return _newMap;
}

static (int[,],int) findDungeonRooms(int [,]dungeonMap,int sizeX,int sizeZ){
    int [,]newMap=new int[sizeX, sizeZ];
    List<Vector2> testCoords = new List<Vector2>(); //test whether surrounding pixels are walls or rooms
    int roomFound = 0;
    System.Array.Copy(dungeonMap,newMap, sizeX*sizeZ);

    for (int i=0;i<sizeX;i++){
        for (int j =0; j<sizeZ;j++){
            if(dungeonMap[i,j]==1){
                testCoords.Add(new Vector2(i,j));
                roomFound++;

                Console.Write(roomFound);

                while (testCoords.Count()>0){
                    int x=(int) testCoords[0].x;
                    int z=(int) testCoords[0].z;
                    testCoords.RemoveAt(0);

                    
                    newMap[x,z]=roomFound;

                    for(int k=x-1;k<=x+1;k++){
                        for(int l=z-1;l<=z+1;l++){
                            if(dungeonMap[k,l]==1){
                                testCoords.Add(new Vector2(k,l));
                                dungeonMap[k,l]=0;
                            }
                        }
                    }
                }
            }
        }
    }
    return (newMap,roomFound);
}

static bool isEdgeCoordinateValid(int[,] dungeonMap,int room,int x, int z){
    if(dungeonMap[x,z]==room){
        for(int k=x-1;k<=x+1;k++){
                        for(int l=z-1;l<=z+1;l++){
                             if(dungeonMap[k,l]==0){
                                return true;
                            }
                        }
                    }
        return false;
    }
    else{
        return false;
    }
}

public static bool IsValid(int x, int z, int sizeX, int sizeZ)
    {
        return (x >= 0) && (x < sizeX) && (z >= 0) && (z < sizeZ);
    }
public static bool IsUnBlocked(int[,] dungeon, int x, int z)
    {
        // Returns true if the cell is not blocked
        bool block=true;
        if(dungeon[x,z]!=0 && dungeon[x,z]!=-1) block=false;
        return block;
    }
public static bool IsDestination(int x, int z, Vector2 dest)
    {
        return (x == dest.x && z == dest.z);
    }
public static double CalculateHValue(int x, int z, Vector2 dest)
    {
        // manhattan distance to goal
        return Math.Abs (x - dest.x) +  Math.Abs (z - dest.z);
    }
static Stack<Cell> astar_corridors(int[,] dungeon, Vector2 start, Vector2 dest){
    Stack<Cell> path = new Stack<Cell>();

    int sizeX=dungeon.GetLength(0);
    int sizeZ=dungeon.GetLength(1);

    
    bool [,] closedList=new bool[sizeX,sizeZ];
    Cell [,] cell_info = new Cell[sizeX,sizeZ];

    for (int i=0;i<sizeX;i++){
        for(int j=0;j<sizeZ;j++){
            cell_info[i, j] = new Cell(new Vector2(i, j));
            cell_info[i,j].parent_i=-1;
            cell_info[i,j].parent_j=-1;
        }
    }
    cell_info[start.x,start.z].gcost=0;
    cell_info[start.x,start.z].hcost=0; 
    cell_info[start.x,start.z].fcost=0;

    SortedSet <Cell> openSet=new SortedSet<Cell>(
            Comparer<Cell>.Create((a, b) => a.fcost.CompareTo(b.fcost)));

    openSet.Add(new Cell(start,0.0));
    bool foundDest=false;
    while (openSet.Count > 0){
            Cell  c = openSet.Min;
            openSet.Remove(c);
            Vector2 pos=c.position;

            int x = pos.x;
            int z = pos.z;
            closedList[x, z] = true;

            // find adjacent cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    // if((i==x-1&&j==z-1)||(i==x+1&&j==z-1)||(i==x-1&&j==z+1)||(i==x+1&&j==z+1))
                    //     continue;
                    if (Math.Abs(i) == 1 && Math.Abs(j) == 1)
                        continue;

                    int newX = x + i;
                    int newZ = z + j;

                    if (IsValid(newX, newZ, sizeX, sizeZ))
                    {
                        if (IsDestination(newX, newZ, dest))
                        {
                            cell_info[newX, newZ].parent_i = x;
                            cell_info[newX, newZ].parent_j = z;
                            Console.WriteLine("dest found");
                            path=TracePath(cell_info, dest);
                            foundDest = true;
                            return path;
                        }

                        // Ignore unavailable cells
                        if (!closedList[newX, newZ] && IsUnBlocked(dungeon, newX, newZ))
                        {
                            double gNew = cell_info[x, z].gcost + 1.0;
                            double hNew = CalculateHValue(newX, newZ, dest);
                            double fNew = gNew + hNew;

                            if (cell_info[newX, newZ].fcost == double.MaxValue || cell_info[newX, newZ].fcost > fNew)
                            {
                                Vector2 newCoords=new Vector2(newX,newZ);
                                openSet.Add((new Cell (newCoords,fNew)));

                                // Update the details of this cell
                                cell_info[newX, newZ].fcost = fNew;
                                cell_info[newX, newZ].gcost= gNew;
                                cell_info[newX, newZ].hcost = hNew;
                                cell_info[newX, newZ].parent_i = x;
                                cell_info[newX, newZ].parent_j = z;
                            }
                        }
                    }
                }
            }
        }
        if (!foundDest)
            Console.WriteLine("Failed to find dest");

    


    return path;
}

 public static Stack<Cell> TracePath(Cell[,] cell_info, Vector2 dest)
    {
        Console.WriteLine("\nThe Path is ");
        
        int sizeX = cell_info.GetLength(0);
        int sizeZ = cell_info.GetLength(1);

        int x = dest.x;
        int z = dest.z;
        

        Stack<Cell> path = new Stack<Cell>();

        bool notFirst=false;
        while (cell_info[x, z].parent_i != -1 && cell_info[x, z].parent_j != -1)
        {
            path.Push(new Cell(new Vector2(x, z)));
            int temp_x = cell_info[x, z].parent_i;
            int temp_z = cell_info[x, z].parent_j;
            x = temp_x;
            z = temp_z;
        }
        
        path.Push(new Cell(new Vector2(x,z)));

        // while (Path.Count > 1)
        // {
        //     Cell p = Path.Peek();
        //     Path.Pop();
        //     Console.Write(" -> ({0},{1}) ", p.position.x, p.position.z);
        // }
        return path;
    }

static void markCorridorsMap(int[,]dungeonMap,Stack<Cell>path){
    int sizeX=dungeonMap.GetLength(0);
    int sizeZ=dungeonMap.GetLength(1);

    while(path.Count>1){
        Cell p=path.Peek();
        path.Pop();
        if(dungeonMap[p.position.x,p.position.z]==0) dungeonMap[p.position.x,p.position.z]=-1;
        // Console.Write(p.position.x);
        // Console.Write(' ');
        // Console.WriteLine(p.position.z);
    }
}
static int[,] createCorridors(int[,]dungeonMap,int sizeX,int sizeZ, int roomNumber){
    int [,]newMap=new int [sizeX,sizeZ];
    System.Array.Copy(dungeonMap,newMap, sizeX*sizeZ);
    if(roomNumber>1){
        for (int i=1;i<=roomNumber;i++){
            int xStart,zStart;
            do{
                    xStart=random.Next(0,sizeX);
                    zStart=random.Next(0,sizeZ);
            }while(!isEdgeCoordinateValid(dungeonMap,i,xStart,zStart));

                Console.Write(xStart);
                Console.Write(' ');
                Console.Write(zStart);
                Console.Write(' ');
                Console.WriteLine(dungeonMap[xStart,zStart]);

            for (int j=1;j<=roomNumber;j++){
                if(j!=i){
                    int xEnd,zEnd;
                    do{
                        xEnd=random.Next(0,sizeX);
                        zEnd=random.Next(0,sizeZ);
                    }while(!isEdgeCoordinateValid(dungeonMap,j,xEnd,zEnd));
                    Stack<Cell> path = new Stack<Cell>();
                    path=astar_corridors(dungeonMap,new Vector2(xStart,zStart),new Vector2(xEnd,zEnd));
                    markCorridorsMap(newMap,path);
                }
            }
        }
    }
    return newMap;
}

void generateWalls(int[,] dungeon, GameObject wallPrefab, float tileSize)
{
    int width = dungeon.GetLength(0);
    int height = dungeon.GetLength(1);

    for (int x = 0; x < width; x++)
    {
        for (int z = 0; z < height; z++)
        {
            // Only place walls in empty spaces
            if (dungeon[x, z] == 0 && HasAdjacentRoomOrCorridor(dungeon, x, z))
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                Instantiate(wallPrefab, position, Quaternion.identity);
            }
        }
    }
}

bool hasAdjacentRoomOrCorridor(int[,] dungeon, int x, int z)
{
    int width = dungeon.GetLength(0);
    int height = dungeon.GetLength(1);

    int[] dx = { 0, 0, -1, 1 };
    int[] dz = { -1, 1, 0, 0 };

    for (int i = 0; i < 4; i++)
    {
        int newX = x + dx[i];
        int newZ = z + dz[i];

        if (newX >= 0 && newX < width && newZ >= 0 && newZ < height)
        {
            if (dungeon[newX, newZ] > 0 || dungeon[newX, newZ] == -1)
                return true;
        }
    }
    return false;
}

// int[,] dungeon = CreateDungeonSquares(50, 50, 8);

// int [,]unmoddedMap=new int[50,50];
// System.Array.Copy(dungeon,unmoddedMap, 50*50);

// var dungeonRooms=findDungeonRooms(dungeon,50,50);
// int[,] modifiedDungeon=dungeonRooms.Item1;
// int roomFound=dungeonRooms.Item2;

// Console.Write(roomFound);
// Console.WriteLine();
// int[,] mapWithCorridors=createCorridors(modifiedDungeon,50,50,roomFound);

// for (int z = 0; z < 50; z++)
// {
//     for (int x = 0; x < 50; x++)
//     {
//         Console.Write(mapWithCorridors[x,z]);
        
//     }
//     Console.WriteLine();
// }

// Stack<Cell> path = new Stack<Cell>();
// path=astar_corridors(dungeon,new Vector2(0,1),new Vector2(49,10));
}