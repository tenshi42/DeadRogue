using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class MapGenerator : MonoBehaviour {
    public enum RoomType
    {
        Start,
        Chest,
        Exit,
        Boss
    }

    [Serializable]
    public class Room
    {
        public int Width;
        public int Height;
        public RoomType type;
    }

    public class EnemyToInstanciate
    {
        public GameObject gameObject;
        public int x;
        public int y;

        public EnemyToInstanciate(GameObject gameObject, int x, int y)
        {
            this.gameObject = gameObject;
            this.x = x;
            this.y = y;
        }
    }

    public GameObject player;

    public List<GameObject> weakEnemies;
    public List<GameObject> middleEnemies;
    public List<GameObject> strongEnemies;
    public int maxNbEnemies;
    private int instanciedEnemiesCount = 0;

    public List<GameObject> Tiles;
    private Dictionary<string, GameObject> tiles;

    public int maxRoomCount = 10;
    public int width = 100;
    public int height = 100;
    private int spawnOffset = 4;

    private string[,] map_tiles;

    public Room[] rooms;

    private int[][] instanciedRooms;
    private int instanciedRoomsCount = 0;

    private List<string> specials;

    private GameObject[] instanciedEnemies;

    // Use this for initialization
    void Start () {
        specials = new List<string>();
        specials.Add("Start");
        specials.Add("Exit");
        specials.Add("Chest");

        tiles = new Dictionary<string, GameObject>();
        foreach(GameObject g in Tiles)
        {
            tiles.Add(g.name, g);
        }

        // Generate
        GenerateMap(width, height, maxRoomCount);
        GenerateCorridors();
        GenerateTravesables();
        EnemyToInstanciate[] tmpEnemiesToInstanciate = GenerateEnemies();

        // Draw
        Vector3 playerPos = DrawMap(1);
        DrawPlayer(playerPos);
        DrawEnemies(tmpEnemiesToInstanciate);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private Transform GetFirstConnectorOfType(GameObject g, string connector)
    {
        Transform tmpConnectors = g.transform.Find("Connectors");
        int tmpConnectorsCount = tmpConnectors.childCount;
        for (int i = 0; i < tmpConnectorsCount; i++)
        {
            string tmpConnectorName = tmpConnectors.GetChild(i).name;
            if (tmpConnectorName == connector)
                return tmpConnectors.GetChild(i);
        }
        return null;
    }

    public void GenerateMap(int sizeX, int sizeY, int nbRooms)
    {
        map_tiles = new string[sizeY, sizeX];
        instanciedRooms = new int[nbRooms+2][];

        int startSizeX = 2;
        int startSizeY = UnityEngine.Random.Range(1, 3);
        int startX = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(1) / 4);
        int startY = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(0) - startSizeY - spawnOffset);

        bool startTile = false;
        for(int i = 0; i < startSizeY; i++)
        {
            for(int j = 0; j < startSizeX; j++)
            {
                if (!startTile && i == startSizeY - 1)
                {
                    int tmpRand = UnityEngine.Random.Range(0, 100);
                    if(tmpRand >= 50)
                    {
                        map_tiles[startY + i, startX + j] = "Start";
                        startTile = true;
                        continue;
                    }
                }

                map_tiles[startY + i, startX + j] = "Empty";
            }
        }
        if (!startTile)
        {
            map_tiles[startY + startSizeY - 1, startX + startSizeX - 1] = "Start";
            startTile = true;
        }

        instanciedRooms[0] = new int[5] { startX, startY, startSizeX, startSizeY, 0 };

        bool exitRoom = false;
        int maxTries = 10;
        int tries = 0;
        while (!exitRoom && tries < maxTries)
        {
            int exitSizeX = 2;
            int exitSizeY = UnityEngine.Random.Range(1, 3);
            int exitX = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(1) / 4);
            int exitY = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(0) - exitSizeY - spawnOffset);

            // check
            bool canBuild = true;
            for (int y = -1; y < exitSizeY + 1; y++)
            {
                for (int x = -1; x < exitSizeX + 1; x++)
                {
                    if (map_tiles[exitY + y, exitX + x] != null)
                    {
                        canBuild = false;
                        y = 1000000;
                        tries++;
                        break;
                    }
                }
            }
            if (!canBuild)
                break;

            tries = 0;

            bool exitTile = false;
            for (int i = 0; i < exitSizeY; i++)
            {
                for (int j = 0; j < exitSizeX; j++)
                {
                    if (!exitTile && i == exitSizeY - 1)
                    {
                        int tmpRand = UnityEngine.Random.Range(0, 100);
                        if (tmpRand >= 50)
                        {
                            map_tiles[exitY + i, exitX + j] = "Exit";
                            exitTile = true;
                            continue;
                        }
                    }

                    map_tiles[exitY + i, exitX + j] = "Empty";
                }
            }
            if (!exitTile)
            {
                map_tiles[exitY + exitSizeY - 1, exitX + exitSizeX - 1] = "Exit";
                exitTile = true;
            }
            exitRoom = true;

            instanciedRooms[1] = new int[5] { exitX, exitY, exitSizeX, exitSizeY, 0 };
        }

        maxTries = 10;
        tries = 0;
        while(instanciedRoomsCount < nbRooms && tries < maxTries)
        {
            int tmpRandRoom = UnityEngine.Random.Range(0, rooms.Length);
            Room room = rooms[tmpRandRoom];

            int tmpRandRoomY = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(0) - room.Height - spawnOffset);
            int tmpRandRoomX = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(1) - room.Width - spawnOffset);

            // check
            bool canBuild = true;
            for (int y = -1; y < room.Height + 1; y++)
            {
                for (int x = -1; x < room.Width + 1; x++)
                {
                    if(map_tiles[tmpRandRoomY + y, tmpRandRoomX + x] != null)
                    {
                        canBuild = false;
                        y = 1000000;
                        tries++;
                        break;
                    }
                }
            }
            if (!canBuild)
                break;

            tries = 0;

            // create
            for (int y = 0; y < room.Height; y++)
            {
                for(int x = 0; x < room.Width; x++)
                {
                    map_tiles[tmpRandRoomY + y, tmpRandRoomX + x] = "Empty";
                }
            }
            instanciedRooms[2 + instanciedRoomsCount] = new int[5] { tmpRandRoomX, tmpRandRoomY, room.Width, room.Height, 0 };
            instanciedRoomsCount++;
        }
    }

    public void GenerateCorridors()
    {
        for(int r1 = 0; r1 < 2 + instanciedRoomsCount; r1++)
        {
            int r2 = (r1 + UnityEngine.Random.Range(1, 2 + instanciedRoomsCount)) % (2 + instanciedRoomsCount);

            Debug.Log("room : " + r1);
            Debug.Log("room : " + r2);

            int tmpX = instanciedRooms[r1][0] + instanciedRooms[r1][2] / 2;
            int tmpY = instanciedRooms[r1][1] + instanciedRooms[r1][3] / 2;

            int destX = instanciedRooms[r2][0] + instanciedRooms[r2][2] / 2;
            int destY = instanciedRooms[r2][1] + instanciedRooms[r2][3] / 2;

            int tmpOffsetDir = (int)Mathf.Sign(UnityEngine.Random.Range(0, 100) - 50);
            int tmpStartDir = UnityEngine.Random.Range(0, 100) - 50;

            if (instanciedRooms[r1][1] < instanciedRooms[r2][1])
            {
                if(tmpOffsetDir < 0)
                {
                    destX -= instanciedRooms[r2][2] / 2 + 1;
                }
                else
                {
                    destX += Mathf.Max(1, instanciedRooms[r2][2] / 2) + (instanciedRooms[r2][2] % 2);
                }
                tmpStartDir = -1;
            }
            else
            {
                if (tmpOffsetDir < 0)
                {
                    tmpX -= instanciedRooms[r1][2] / 2 + 1;
                }
                else
                {
                    tmpX += Mathf.Max(1, instanciedRooms[r1][2] / 2) + (instanciedRooms[r1][2] % 2);
                }
                tmpStartDir = -1;
            }

            int dirX = (int)Mathf.Sign(destX - tmpX);
            int dirY = (int)Mathf.Sign(destY - tmpY);

            while(tmpX != destX || tmpY != destY)
            {
                dirX = (int)Mathf.Sign(destX - tmpX);
                dirY = (int)Mathf.Sign(destY - tmpY);

                /*if(dirY < 0 && tmpStartDir < 0)
                {
                    tmpStartDir = -tmpStartDir;
                }*/

                if (map_tiles[tmpY, tmpX] == null)
                {
                    map_tiles[tmpY, tmpX] = "Corridor";
                }
                else if(specials.Contains(map_tiles[tmpY, tmpX]))
                {
                    //tmpX += dirX != 0 ? dirX : 1;
                }
                if (tmpStartDir >= 0)
                {
                    if(tmpX != destX)
                    {
                        tmpX += dirX;
                    }
                    else if (tmpY != destY)
                    {
                        tmpY += dirY;
                    }
                }
                else
                {
                    if (tmpY != destY)
                    {
                        tmpY += dirY;
                    }
                    else if (tmpX != destX)
                    {
                        tmpX += dirX;
                    }
                }
            }
        }
    }

    /*public void GenerateTravesables()
    {
        for (int y = 1; y < map_tiles.GetLength(0) - 1; y++)
        {
            for (int x = 1; x < map_tiles.GetLength(1) - 1; x++)
            {
                if(map_tiles[y, x] == "Corridor" || map_tiles[y, x] == "Empty")
                {
                    if(((map_tiles[y - 1, x] != null && map_tiles[y - 1, x] != "Empty") || (map_tiles[y - 1, x] != null && (map_tiles[y - 1, x - 1] != null || map_tiles[y - 1, x + 1] != null))) &&
                        (map_tiles[y, x - 1] == null || map_tiles[y, x + 1] == null || map_tiles[y, x - 1] == "Corridor" || map_tiles[y, x + 1] == "Corridor"))
                    {
                        map_tiles[y, x] = "Top";
                        if ((map_tiles[y, x - 1] == null && map_tiles[y, x + 1] != null) || map_tiles[y - 1, x + 1] == "TopLeftTraversable")
                            map_tiles[y, x] += "Left";
                        else if ((map_tiles[y, x - 1] != null && map_tiles[y, x + 1] == null && map_tiles[y, x - 1] != "TopTraversable") || map_tiles[y - 1, x - 1] == "TopRightTraversable")
                            map_tiles[y, x] += "Right";
                        map_tiles[y, x] += "Traversable";
                    }
                }
            }
        }
    }*/

    public void GenerateTravesables()
    {
        for (int y = 1; y < map_tiles.GetLength(0) - 2; y++)
        {
            for (int x = 1; x < map_tiles.GetLength(1) - 2; x++)
            {
                if (map_tiles[y, x] == "Corridor" && map_tiles[y - 1, x] != null)
                {
                    map_tiles[y, x] = "Top";
                    map_tiles[y, x] += "Traversable";
                }
                else if(map_tiles[y, x] != null && !specials.Contains(map_tiles[y, x]) && map_tiles[y, x] != "Corridor" && map_tiles[y - 1, x] != null)
                {
                    if(map_tiles[y - 1, x] == "TopTraversable" || map_tiles[y - 1, x] == "TopLeftTraversable" || map_tiles[y - 1, x] == "TopRightTraversable")
                    {
                        if (map_tiles[y, x + 1] == null || map_tiles[y, x + 1] == "Corridor")
                        {
                            map_tiles[y, x] = "TopRightTraversable";
                        }
                        else if (map_tiles[y, x - 1] == null || map_tiles[y, x - 1] == "Corridor")
                        {
                            map_tiles[y, x] = "TopLeftTraversable";
                        }
                    }
                }
                else if(map_tiles[y, x] == "Corridor" && map_tiles[y, x - 1] == "TopTraversable" && map_tiles[y, x + 1] == "Corridor" && map_tiles[y - 1, x - 1] != null && map_tiles[y - 1, x + 1] != null)
                {
                    map_tiles[y, x] = null;
                }
            }
        }
    }

    public Vector3 DrawMap(int theme)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        float tileSize = 3.2f;
        float baseX = 0;
        float baseY = tileSize * map_tiles.GetLength(0);

        float playerX = 0;
        float playerY = 0;

        for (int y = 0; y < map_tiles.GetLength(0); y++)
        {
            for(int x = 0; x < map_tiles.GetLength(1); x++)
            {
                GameObject tmpTile;
                String tmpTileName = "";

                //Debug.Log(map_tiles[y, x]);

                if(map_tiles[y, x] != null) {
                    tmpTileName = map_tiles[y, x];
                    if (tmpTileName == "Start")
                    {
                        playerX = baseX + x * tileSize + 1.6f;
                        playerY = baseY - y * tileSize + 1.6f;
                    }
                }
                else if (map_tiles[y, x] == null)
                {
                    if(y < map_tiles.GetLength(0)-1 && map_tiles[y+1,x] != null)
                    {
                        tmpTileName += "Bottom";
                    }
                    if (y > 0 && map_tiles[y - 1, x] != null)
                    {
                        tmpTileName += "Top";
                    }
                    if (x > 0 && map_tiles[y, x - 1] != null)
                    {
                        tmpTileName += "Left";
                    }
                    if (x < map_tiles.GetLength(1) - 1 && map_tiles[y, x + 1] != null)
                    {
                        tmpTileName += "Right";
                    }
                }

                if (tmpTileName != "")
                {
                    //Debug.Log(tmpTileName);
                    tmpTile = GameObject.Instantiate(tiles[tmpTileName + theme.ToString()], new Vector3(baseX + x * tileSize, baseY - y * tileSize), new Quaternion(), transform);
                }
            }
        }

        return new Vector3(playerX, playerY);
    }

    public void DrawPlayer(Vector3 pos)
    {
        GameObject tmpPlayer = GameObject.Instantiate(player, pos, new Quaternion());
        Camera.main.GetComponent<Camera2DFollow>().target = tmpPlayer.transform;
        Camera.main.transform.position = pos + new Vector3(0, 0, -10);
    }

    public EnemyToInstanciate[] GenerateEnemies()
    {
        int nbEnemies = UnityEngine.Random.Range(maxNbEnemies / 2, maxNbEnemies + 1);
        instanciedEnemiesCount = 0;
        EnemyToInstanciate[] enemiesToInstanciate = new EnemyToInstanciate[nbEnemies];

        while(instanciedEnemiesCount < nbEnemies)
        {
            int tmpGroupNb = UnityEngine.Random.Range(1, Mathf.Min(4, nbEnemies - instanciedEnemiesCount));
            int tmpLocalInstancied = 0;

            int tmpX = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(1) - spawnOffset);
            int tmpY = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(0) - spawnOffset);

            while(map_tiles[tmpY, tmpX] == null)
            {
                tmpX = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(1) - spawnOffset);
                tmpY = UnityEngine.Random.Range(spawnOffset, map_tiles.GetLength(0) - spawnOffset);
            }

            int maxTries = 10;
            int tries = 0;

            while (tmpLocalInstancied < tmpGroupNb && tries < maxTries)
            {
                int tmpSubX = tmpX + UnityEngine.Random.Range(0, spawnOffset * 2) - spawnOffset;
                int tmpSubY = tmpY + UnityEngine.Random.Range(0, spawnOffset * 2) - spawnOffset;

                if (map_tiles[tmpSubY, tmpSubX] == "Corridor" || (map_tiles[tmpSubY + 1, tmpSubX] != "Empty" && map_tiles[tmpSubY + 1, tmpSubX] != "Corridor" && (map_tiles[tmpSubY + 2, tmpSubX] != "TopTraversable" && map_tiles[tmpSubY + 2, tmpSubX] != "TopLeftTraversable" && map_tiles[tmpSubY + 2, tmpSubX] != "TopRightTraversable") && !specials.Contains(map_tiles[tmpSubY + 1, tmpSubX]) && map_tiles[tmpSubY, tmpSubX] != null)){
                    tries = 0;
                    Debug.Log(map_tiles[tmpSubY, tmpSubX]);

                    GameObject tmpEnemy = null;
                    int tmpEnemyType = UnityEngine.Random.Range(0, 100);

                    if (tmpEnemyType > 90 && strongEnemies.Count > 0)
                    {
                        int tmpEnemyIndex = UnityEngine.Random.Range(0, strongEnemies.Count);
                        tmpEnemy = strongEnemies[tmpEnemyIndex];
                    }
                    else if(tmpEnemyType > 60 && middleEnemies.Count > 0)
                    {
                        int tmpEnemyIndex = UnityEngine.Random.Range(0, middleEnemies.Count);
                        tmpEnemy = middleEnemies[tmpEnemyIndex];
                    }
                    else
                    {
                        int tmpEnemyIndex = UnityEngine.Random.Range(0, weakEnemies.Count);
                        tmpEnemy = weakEnemies[tmpEnemyIndex];
                    }

                    enemiesToInstanciate[instanciedEnemiesCount] = new EnemyToInstanciate(tmpEnemy, tmpSubX, tmpSubY);
                    tmpLocalInstancied++;
                    instanciedEnemiesCount++;
                }
                tries++;
            }
        }

        return enemiesToInstanciate;
    }

    public void DrawEnemies(EnemyToInstanciate[] enemiesToInstanciate)
    {
        float tileSize = 3.2f;
        float baseX = 0;
        float baseY = tileSize * map_tiles.GetLength(0);

        foreach (EnemyToInstanciate g in enemiesToInstanciate)
        {
            GameObject tmpEnemy = GameObject.Instantiate(g.gameObject, new Vector3(baseX + g.x * tileSize + 1.6f , baseY - g.y * tileSize + 1.3f), new Quaternion());
        }
    }
}
