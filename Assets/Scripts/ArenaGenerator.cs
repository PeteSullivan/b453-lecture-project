using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile tile;
    [SerializeField] private GameObject[] bases;

    //x- -11 to 11
    //y- -5 to 5

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateArena()
    {
        //build map
        GenerateMap();

        //try to place bases several times
        for (int i = 0; i < 10; i++)
        {
            if (PlaceBases())
            {
                Debug.Log("placed Bases!");
                break;
            }
            else
            {
                Debug.Log("failed!");
            }
        }
        
    }

    public void GenerateMap()
    {
        Debug.Log("making map");


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
    }
    
    public bool PlaceBases()
    {

        for (int i = 0; i < 4; i++) //loop for all 4 bases
        {
            
            for (int j = 0; j < 25; j++) //test 25 random spots per base
            {
                //pick a random spot to test
                float randx = Random.Range(-10, 10);
                float randy = Random.Range(-4, 4);
                Vector3 testPosition = new Vector3(randx, randy, 0);

                //check if it is a valid spot
                if (CheckBaseLocation(testPosition))
                {
                    Debug.Log("placing base: " + i);
                    bases[i].transform.position = testPosition;
                    break;
                }
            }

        }


        return true;
    }

    public bool CheckBaseLocation(Vector3 testPosition)
    {

        return true;
    }


}
