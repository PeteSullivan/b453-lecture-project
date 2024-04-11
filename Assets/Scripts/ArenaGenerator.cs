using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile tile;
    [SerializeField] private Tile testTile;
    [SerializeField] private GameObject[] bases;


    //parameters for generation
    [SerializeField] private float baseProximityMinimum;
    [SerializeField] private float tileProximityMinimum;

    //x- -11 to 11
    //y- -5 to 5

    // Start is called before the first frame update
    void Start()
    {
        GenerateArena();
    }

    public void GenerateArena()
    {
        //build map
        GenerateMap();

        //try to place bases
        bool success = AttemptBasePlacement();

        //if unsuccessful generation, repeat until successful. 
        //with a high enough success rate, this will not go infinite and freeze the game (right now, 90%+ success rate)
        while (!success) 
        {
            //build map
            GenerateMap();

            //try to place bases
            success = AttemptBasePlacement();
        }

    }

    public void GenerateMap()
    {
        tilemap.ClearAllTiles();


        int rectangleCount = 10;
        int rectangleSize = 5;

        //spawn rectangles on outer edges of arena
        for(int i = 0; i < rectangleCount; i++)
        {
            //choose random position and size for rectangle
            int randx = Random.Range(8, 11);
            int randy = Random.Range(3, 5);
            int flip = Random.Range(0, 2);
            //Debug.Log("flip: " +  flip);
            randx = (flip == 0) ? randx : randx * -1;
            flip = Random.Range(0, 2);
            randy = (flip == 0) ? randy : randy * -1;

            int width = Random.Range(1, rectangleSize);
            int height = Random.Range(1, rectangleSize);

            //fill rectangle
            for(float x = -width / 2; x <= width / 2; x = x + .5f)
            {
                for (float y = -height / 2; y < height / 2; y = y + .5f)
                {
                    Vector3Int currentCell = tilemap.WorldToCell(new Vector3(randx + x, randy + y, 0));
                    tilemap.SetTile(currentCell, tile);

                }
            }
                        
        }

        //place 10 random tiles
        for (int i = 0; i < 10; i++)
        {
            float randx = Random.Range(-11, 11);
            float randy = Random.Range(-5, 5);
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(randx, randy, 0));
            tilemap.SetTile(currentCell, tile);
        }

        //horizontal line
        int randLen = Random.Range(2, 6);
        float startx = Random.Range(-11, 7);
        float starty = Random.Range(-5, 5);
        for (int i = 0; i < randLen; i++)
        {
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(startx, starty, 0));
            tilemap.SetTile(currentCell, tile);
            startx += 0.5f;
        }

        //vertical line
        randLen = Random.Range(2, 6);
        startx = Random.Range(-11, 11);
        starty = Random.Range(-5, 2);
        for (int i = 0; i < randLen; i++)
        {
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(startx, starty, 0));
            tilemap.SetTile(currentCell, tile);
            starty += 0.5f;
        }


        BuildBorder();
    }

    public void BuildBorder()
    {
        
        //make sure border of map is filled in
        for (float i = -5; i <= 5; i = i + .5f) //fill left and right walls
        {
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(-11, i, 0));
            tilemap.SetTile(currentCell, tile);
            currentCell = tilemap.WorldToCell(new Vector3(11, i, 0));
            tilemap.SetTile(currentCell, tile);
        }
        for (float i = -11; i <= 11; i = i + .5f) //fill top and bottom walls
        {
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(i, -5, 0));
            tilemap.SetTile(currentCell, tile);
            currentCell = tilemap.WorldToCell(new Vector3(i, 5, 0));
            tilemap.SetTile(currentCell, tile);
        }

    }


    public bool AttemptBasePlacement() 
    {
        //attempts to place bases several times, returns if it was successful or not.
        for (int i = 0; i < 25; i++)
        {
            if (PlaceBases())
            {
                break; //if a successful placement is found, keep it
            }
        }

        //if any bases are still offscreen, there wasn't a successful placement, return false.
        for (int i = 0; i < 4; i++)
        {
            if (bases[i].transform.position == new Vector3(0, 20, 0))
            {
                return false;
            }
        }

        return true;
    }
    public bool PlaceBases()
    {
        //move bases offscreen to start
        for (int i = 0; i < 4; i++)
        {
            bases[i].transform.position = new Vector3(0, 20, 0);
        }


        for (int i = 0; i < 4; i++) //loop for all 4 bases
        {
            //check if base was placed
            bool successfulPlacement = false;


            for (int j = 0; j < 25; j++) //test 25 random spots per base
            {
                //pick a random spot to test
                float randx = Random.Range(-10, 10);
                float randy = Random.Range(-4, 4);
                Vector3 testPosition = new Vector3(randx, randy, 0);

                //if it's a valid spot, place it
                if (CheckBaseLocation(testPosition))
                {
                    bases[i].transform.position = testPosition;
                    successfulPlacement = true;
                    break;
                }
            }
            
            //if no placement was found, return false.
            if (!successfulPlacement)
            {
                return false;
            }

        }
        
        //if all bases were placed, return true.
        return true;
    }

    public bool CheckBaseLocation(Vector3 testPosition)
    {
        for (float i = -tileProximityMinimum; i <= tileProximityMinimum; i = i + 0.25f)
        {
            for (float j = -tileProximityMinimum; j <= tileProximityMinimum; j = j + 0.25f)
            {
                Vector3Int position = tilemap.WorldToCell(testPosition + new Vector3(i, j, 0));
                if (tilemap.GetTile(position) == tile || tilemap.GetTile(position) == testTile)
                {
                    return false; //if a tile is too close, return false
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            float distance = Vector3.Distance(testPosition, bases[i].transform.position);
            if (distance < baseProximityMinimum)
            {
                return false; //if any base is too close, return false
            }
        }

        //if no bases or tiles are too close, it is a good spot.
        return true;
    }


}


/*  generation methods


-----------------------LINE SPAWNING---------------------------------------------
        int randLen = Random.Range(2, 6);
        float startx = Random.Range(-11, 7);
        float starty = Random.Range(-5, 5);
        for (int i = 0; i < randLen; i++)
        {
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(startx, starty, 0));
            tilemap.SetTile(currentCell, tile);
            startx += 0.5f;
        }

--------------------------RECTANGLE SPAWNS---------------------------------------
        int rectangleCount = 10;
        int rectangleSize = 5;

        //spawn rectangles on outer edges of arena
        for(int i = 0; i < rectangleCount; i++)
        {
            //choose random position and size for rectangle
            int randx = Random.Range(8, 11);
            int randy = Random.Range(3, 5);
            int flip = Random.Range(0, 2);
            Debug.Log("flip: " +  flip);
            randx = (flip == 0) ? randx : randx * -1;
            flip = Random.Range(0, 2);
            randy = (flip == 0) ? randy : randy * -1;

            int width = Random.Range(1, rectangleSize);
            int height = Random.Range(1, rectangleSize);



            //fill rectangle
            for(float x = -width / 2; x <= width / 2; x = x + .5f)
            {
                for (float y = -height / 2; y < height / 2; y = y + .5f)
                {
                    Vector3Int currentCell = tilemap.WorldToCell(new Vector3(randx + x, randy + y, 0));
                    tilemap.SetTile(currentCell, tile);

                }
            }
                        
        }
  ----------------------------SNAKE------------------------------------
     
        float randx = Random.Range(-11, 11);
        float randy = Random.Range(-5, 5);
        Vector3 snakeHead = new Vector3(randx, randy, 0);
        snakeHead = new Vector3(0, 0, 0);

        int snakeTime = 80;
        for (int i = 0; i < snakeTime; i++)
        {
            float direction = Random.Range(0, 3);
            switch (direction)
            {
                case 0:
                    snakeHead += new Vector3(0, 0.5f, 0);
                    break;
                case 1:
                    snakeHead += new Vector3(0, -0.5f, 0);
                    break;
                case 2:
                    snakeHead += new Vector3(0.5f, 0, 0);
                    break;
                case 3:
                    snakeHead += new Vector3(-0.5f, 0, 0);
                    break;
            }
            Vector3Int currentCell = tilemap.WorldToCell(snakeHead);
            tilemap.SetTile(currentCell, testTile);

        }

 
 -----------------------------RANDOM TILES----------------------------------------------       
        //remove 10 random tiles
        for (int i = 0; i < 10; i++)
        {
            float randx = Random.Range(-11, 11);
            float randy = Random.Range(-5, 5);
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(randx, randy, 0));
            tilemap.SetTile(currentCell, null);
        }
        //place 10 random tiles
        for (int i = 0; i < 10; i++)
        {
            float randx = Random.Range(-11, 11);
            float randy = Random.Range(-5, 5);
            Vector3Int currentCell = tilemap.WorldToCell(new Vector3(randx, randy, 0));
            tilemap.SetTile(currentCell, tile);
        }

 */