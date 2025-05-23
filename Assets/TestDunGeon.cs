using UnityEngine;
using System.Collections.Generic;
using System;
using System.Drawing;
using Unity.Entities.UniversalDelegates;
using JetBrains.Annotations;
public class TestDunGeon : MonoBehaviour
{

    public int mapWidth = 50;
    public int mapHeight = 50;
    public int tilesToGenerate = 10;
    public float cellSize = 1f;
    public float tileSize = 1f;

    [Header("Wall Settings")]
    public GameObject wallPrefab;
    public float wallThickness = 0.1f;
    public float wallHeight = 2f;

    [Header("Room Settings")]
    public int minRoomSize = 5;

    [Header("Spawn and exit")]
    public GameObject startPrefab;
    public GameObject endPrefab;

    [Header("Enemy types")]
    public GameObject enemyType1Pref;
    public GameObject enemyType2Pref;
    public GameObject enemyType3Pref;

    public int numEnemiesMin;

    public int numEnemiesMax;

    [Header("itemSpawns")]
    public int numPotionsMin;
    public int numPotionsMax;

    public GameObject healthPotionPrefab;
    public int numUpgradesMin;
    public int numUpgradesMax;
    public GameObject attackUpgradePrefab;

    public class Room
    {
        public int Id { get; set; }
        public List<Point> Cells { get; set; } = new List<Point>();
        public List<Point> Perimeter { get; set; } = new List<Point>();
        public Point Center { get; set; }
    }

    public class Edge
    {
        public int A { get; }
        public int B { get; }
        public float Weight { get; }

        public Edge(int a, int b, float weight)
        {
            A = a;
            B = b;
            Weight = weight;
        }
    }

    public class DungeonGenerator
    {
        public int[,] GenerateCorridors(int[,] map)
        {
            var rooms = FindRooms(map);
            var mst = MinimumSpanningTree(rooms);
            foreach (var edge in mst)
            {
                var roomA = rooms[edge.A];
                var roomB = rooms[edge.B];
                var (pointA, pointB) = FindClosestPerimeterPoints(roomA, roomB);
                CreateCorridor(map, pointA, pointB);
            }
            return map;
        }

        private List<Room> FindRooms(int[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            bool[,] visited = new bool[width, height];
            List<Room> rooms = new List<Room>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!visited[x, y] && map[x, y] != 0)
                    {
                        Room room = FloodFill(map, visited, x, y);
                        CalculateRoomCenter(room);
                        FindRoomPerimeter(map, room);
                        rooms.Add(room);
                    }
                }
            }
            return rooms;
        }

        private Room FloodFill(int[,] map, bool[,] visited, int x, int y)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            Room room = new Room { Id = map[x, y] };
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(x, y));
            visited[x, y] = true;

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                room.Cells.Add(p);

                foreach (var neighbor in GetNeighbors(p))
                {
                    if (neighbor.X >= 0 && neighbor.X < width && neighbor.Y >= 0 && neighbor.Y < height)
                    {
                        if (!visited[neighbor.X, neighbor.Y] && map[neighbor.X, neighbor.Y] == room.Id)
                        {
                            visited[neighbor.X, neighbor.Y] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return room;
        }

        private IEnumerable<Point> GetNeighbors(Point p)
        {
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X, p.Y - 1);
            yield return new Point(p.X, p.Y + 1);
        }

        private void CalculateRoomCenter(Room room)
        {
            int sumX = 0, sumY = 0;
            foreach (var p in room.Cells)
            {
                sumX += p.X;
                sumY += p.Y;
            }
            room.Center = new Point(sumX / room.Cells.Count, sumY / room.Cells.Count);
        }

        private void FindRoomPerimeter(int[,] map, Room room)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            foreach (var p in room.Cells)
            {
                foreach (var neighbor in GetNeighbors(p))
                {
                    if (neighbor.X < 0 || neighbor.X >= width || neighbor.Y < 0 || neighbor.Y >= height || map[neighbor.X, neighbor.Y] == 0)
                    {
                        room.Perimeter.Add(p);
                        break;
                    }
                }
            }
        }

        private List<Edge> MinimumSpanningTree(List<Room> rooms)
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    float dx = rooms[i].Center.X - rooms[j].Center.X;
                    float dy = rooms[i].Center.Y - rooms[j].Center.Y;
                    edges.Add(new Edge(i, j, (float)Math.Sqrt(dx * dx + dy * dy)));
                }
            }
            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));

            int[] parent = new int[rooms.Count];
            for (int i = 0; i < parent.Length; i++) parent[i] = i;

            List<Edge> mst = new List<Edge>();
            foreach (var edge in edges)
            {
                int aRoot = Find(parent, edge.A);
                int bRoot = Find(parent, edge.B);
                if (aRoot != bRoot)
                {
                    mst.Add(edge);
                    parent[aRoot] = bRoot;
                }
            }
            return mst;
        }

        private int Find(int[] parent, int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent, parent[x]);
            return parent[x];
        }

        private (Point, Point) FindClosestPerimeterPoints(Room a, Room b)
        {
            Point bestA = a.Perimeter[0], bestB = b.Perimeter[0];
            float minDist = float.MaxValue;

            foreach (var pa in a.Perimeter)
            {
                foreach (var pb in b.Perimeter)
                {
                    float dx = pa.X - pb.X;
                    float dy = pa.Y - pb.Y;
                    float dist = dx * dx + dy * dy;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestA = pa;
                        bestB = pb;
                    }
                }
            }
            return (bestA, bestB);
        }

        private void CreateCorridor(int[,] map, Point start, Point end)
        {
            // HOR FIRST
            if (start.X != end.X)
            {
                int step = start.X < end.X ? 1 : -1;
                for (int x = start.X; x != end.X; x += step)
                {
                    if (map[x, start.Y] == 0)
                        map[x, start.Y] = -1; // corridor
                }
            }

            if (start.Y != end.Y)
            {
                int step = start.Y < end.Y ? 1 : -1;
                for (int y = start.Y; y != end.Y; y += step)
                {
                    if (map[end.X, y] == 0)
                        map[end.X, y] = -1; // corridor
                }
            }
        }
    }


    public class IntVector2
    {
        public int x;
        public int z;

        public IntVector2(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }
    public class Cell {
        public IntVector2 position;
        public double fcost = double.MaxValue;
        public double gcost = double.MaxValue;
        public double hcost = double.MaxValue;
        public IntVector2 connection;
        public bool isWall;

        public int parent_i, parent_j;

        public Cell(IntVector2 pos) {
            position = pos;
        }
        public Cell(IntVector2 pos, double fc) {
            position = pos;
            fcost = fc;
        }
    }
    public System.Random random = new System.Random();
    public int RandomRange(int min, int max) {
        return random.Next(min, max);
    }
    public int[,] corridorsDungeon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("1");
        int[,] dungeonMap = CreateDungeonSquares(mapWidth, mapHeight, tilesToGenerate);

        var roomsDungeon = findDungeonRooms(dungeonMap, mapWidth, mapHeight);

        int[,] modifiedDungeon = roomsDungeon.Item1;
        int roomFound = roomsDungeon.Item2;

        // corridorsDungeon = createCorridors(dungeonMap, mapWidth, mapHeight, roomFound);
        DungeonGenerator generator = new DungeonGenerator();
        corridorsDungeon = generator.GenerateCorridors(modifiedDungeon);

        Debug.Log("Corridors Dungeon Map:");

        for (int x = 0; x < corridorsDungeon.GetLength(0); x++)
        {
            string row = "";
            for (int z = 0; z < corridorsDungeon.GetLength(1); z++)
            {
                row += corridorsDungeon[x, z] + " ";
            }
            Debug.Log(row);
        }
        CreateWalls();
        // generateWalls(corridorsDungeon, wallPrefab, tileSize);
        SpawnExits(corridorsDungeon, startPrefab, endPrefab, roomFound);

        int numEnemies = random.Next(numEnemiesMin, numEnemiesMax);
        SpawnEnemies(corridorsDungeon, numEnemies, roomFound, enemyType1Pref, enemyType2Pref, enemyType3Pref);
        SpawnItems(corridorsDungeon, numPotionsMin, numPotionsMax, healthPotionPrefab,numUpgradesMin,numUpgradesMax,attackUpgradePrefab);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public int[,] CreateDungeonSquares(int _sizeX, int _sizeZ, int _squareNbr)
    {
        int[,] _newMap = new int[_sizeX, _sizeZ];

        for (int i = 0; i < _squareNbr; i++)
        {
            // Calculate random room size and position. Ensure the room is inside the map.
            // int _roomSizeX = random.Next(10, _sizeX / 5);
            // int _roomSizeZ = random.Next(10, _sizeZ / 5);
            int _roomSizeX = random.Next(minRoomSize, _sizeX / 5);
            int _roomSizeZ = random.Next(minRoomSize, _sizeZ / 5);
            int _roomPosX = random.Next(minRoomSize, _sizeX - _roomSizeX);
            int _roomPosZ = random.Next(minRoomSize, _sizeZ - _roomSizeZ);

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
    public (int[,], int) findDungeonRooms(int[,] dungeonMap, int sizeX, int sizeZ) {
        int[,] newMap = new int[sizeX, sizeZ];
        List<IntVector2> testCoords = new List<IntVector2>(); //test whether surrounding pixels are walls or rooms
        int roomFound = 0;
        System.Array.Copy(dungeonMap, newMap, sizeX * sizeZ);

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeZ; j++) {
                if (dungeonMap[i, j] == 1) {
                    testCoords.Add(new IntVector2(i, j));
                    roomFound++;

                    Debug.Log(roomFound);

                    while (testCoords.Count > 0) {
                        int x = (int)testCoords[0].x;
                        int z = (int)testCoords[0].z;
                        testCoords.RemoveAt(0);


                        newMap[x, z] = roomFound;

                        for (int k = x - 1; k <= x + 1; k++) {
                            for (int l = z - 1; l <= z + 1; l++) {
                                if (k >= 0 && k < sizeX && l >= 0 && l < sizeZ) {
                                    if (dungeonMap[k, l] == 1) {
                                        testCoords.Add(new IntVector2(k, l));
                                        dungeonMap[k, l] = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return (newMap, roomFound);
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
    void CheckWall(int x, int z, int dx, int dz)
    {
        int neighborX = x + dx;
        int neighborZ = z + dz;
        bool needsWall = false;

        if (neighborX < 0 || neighborX >= corridorsDungeon.GetLength(0) ||
           neighborZ < 0 || neighborZ >= corridorsDungeon.GetLength(1))
        {
            needsWall = true;
        }
        else
        {
            needsWall = corridorsDungeon[neighborX, neighborZ] == 0;
        }

        if (needsWall)
        {
            Vector3 position = new Vector3(
                x * cellSize + dx * (cellSize / 2),
                wallHeight / 2,
                z * cellSize + dz * (cellSize / 2)
            );

            Vector3 scale = new Vector3(
                dx != 0 ? wallThickness : cellSize,
                wallHeight,
                dz != 0 ? wallThickness : cellSize
            );

            GameObject wall = Instantiate(wallPrefab, transform);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
        }
    }
    void CreateWalls()
    {
        int width = corridorsDungeon.GetLength(0);
        int height = corridorsDungeon.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (corridorsDungeon[x, z] == 0) continue;

                CheckWall(x, z, 1, 0);
                CheckWall(x, z, -1, 0);
                CheckWall(x, z, 0, 1);
                CheckWall(x, z, 0, -1);
            }
        }
    }
    void generateWalls(int[,] dungeon, GameObject wallPrefab, float tileSize)
    {
        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // place walls in empty spaces
                if (dungeon[x, z] == 0)
                {
                    if (hasAdjacentRoomOrCorridor(dungeon, x, z))
                    {
                        Vector3 position = new Vector3(x * tileSize, wallHeight, z * tileSize);
                        Instantiate(wallPrefab, position, Quaternion.identity);
                        Debug.Log("Wall placed at: (" + x + ", " + z + ")");
                    }
                }
            }
        }
    }
    void SpawnExits(int[,] dungeon, GameObject startPrefab, GameObject endPrefab, int roomNum) {
        Debug.Log("spawn exits");
        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);


        while (true) {
            int x = random.Next(0, width);
            int z = random.Next(0, height);
            float y = (float)-0.5;
            if (dungeon[x, z] == 1) {
                Vector3 position = new Vector3(x, y, z);
                Instantiate(startPrefab, position, Quaternion.identity);
                Debug.Log("Start placed at: (" + x + "," + z + ")");
                Debug.Log(startPrefab.name);
                break;
            }
        }
        while (true) {
            int x = random.Next(0, width);
            int z = random.Next(0, height);
            float y = (float)0;
            if (dungeon[x, z] == roomNum) {
                Vector3 position = new Vector3(x, y, z);
                Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);
                Instantiate(endPrefab, position, rotation);
                Debug.Log(endPrefab.name);
                break;
            }
        }
    }
    public void SpawnEnemies(int[,] dungeon, int numEnemies, int roomFound, GameObject enemyType1Pref, GameObject enemyType2Pref, GameObject enemyType3Pref)
    {
        Debug.Log("spawn enemies");
        Debug.Log(roomFound);

        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);

        int[] enemyRooms = new int[numEnemies];
        int[] enemyTypes = new int[numEnemies];

        for (int i = 0; i < numEnemies; i++)
        {
            if (roomFound > 1)
            {

                enemyRooms[i] = random.Next(2, roomFound);
            }
            else
            {
                enemyRooms[i] = roomFound;
            }
            enemyTypes[i] = random.Next(1, 4);

            // int x = random.Next(0, width);
            // int z = random.Next(0, height);

            while (true) {
                int x = random.Next(0, width);
                int z = random.Next(0, height);
                float y = (float)0;

                if (dungeon[x, z] == enemyRooms[i])
                {
                    Vector3 position = new Vector3(x, y, z);
                    if (enemyTypes[i] == 1)
                    {
                        Instantiate(enemyType1Pref, position, Quaternion.identity);
                    }
                    else if (enemyTypes[i] == 2)
                    {
                        Instantiate(enemyType2Pref, position, Quaternion.identity);
                    }
                    else
                    {
                        position = new Vector3(x, (float)0.5, z);
                        Instantiate(enemyType3Pref, position, Quaternion.identity);
                    }
                    Debug.Log("enemy placed at: (" + x + "," + z + ")");
                    Debug.Log(enemyTypes[i]);
                    break;
                }
            }

        }

    }

    public void SpawnItems(int[,] dungeon, int numPotionsMin, int numPotionsMax, GameObject healthPotionPrefab,
    int numUpgradesMin, int numUpgradesMax,GameObject attackUpgradePrefab)
    {
        int width = dungeon.GetLength(0);
        int height = dungeon.GetLength(1);

        int numPotions = random.Next(numPotionsMin, numPotionsMax);
        int numUpgrades = random.Next(numUpgradesMin, numUpgradesMax);

        for (int i = 0; i < numPotions; i++)
        {
            while (true)
            {
                int x = random.Next(0, width);
                int z = random.Next(0, height);
                int y = 0;

                if (dungeon[x, z] != 0 && dungeon[x, z] != -1)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Instantiate(healthPotionPrefab, position, Quaternion.identity);
                    Debug.Log("health potion placed at: (" + x + "," + z + ")");
                    break;
                }
            }


        }
        for (int i = 0; i < numUpgrades; i++)
        {
            while (true)
            {
                int x = random.Next(0, width);
                int z = random.Next(0, height);
                float y = (float)0.5;

                if (dungeon[x, z] != 0 && dungeon[x, z] != -1)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Instantiate(attackUpgradePrefab, position, Quaternion.identity);
                    Debug.Log("upgrade placed at: (" + x + "," + z + ")");
                    break;
                }
            }

            
        }
}

}

