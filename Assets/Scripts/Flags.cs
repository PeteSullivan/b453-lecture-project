using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour
{
    //references
    [SerializeField] GameObject flag1;
    [SerializeField] GameObject flag2;
    [SerializeField] int color;
    private LineRenderer lr;

    private int clicks = 0;
    private bool holding = false;
    private GameObject flagToBeMoved;
    private Vector3 startingPosition;
    private int selectedColor = 0;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }
    void Update()
    {
        CheckSelectedColor();
        if (selectedColor == color)
        {
            FlagMovement();
        }
    }

    private void CheckSelectedColor()
    {
        /*if 1 is pressed, move red
         * if 2, move blue
         * if 3, move green
         * if 4, move yellow
         */
        if (Input.GetKeyDown("1"))
        {
            selectedColor = 1;
        }
        else if (Input.GetKeyDown("2"))
        {
            selectedColor = 2;
        }
        else if (Input.GetKeyDown("3"))
        {
            selectedColor = 3;
        }
        else if (Input.GetKeyDown("4"))
        {
            selectedColor = 4;
        }
    }

    private void FlagMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clicks = clicks + 1;
            if (clicks == 1) //if first click, place first flag
            {
                moveFlag(flag1);
            }
            else if (clicks == 2) //if second click, place second flag
            {
                moveFlag(flag2);
            }
            else //else, identify closest flag and start holding it
            {

                float flag1Distance = Vector3.Distance(flag1.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                float flag2Distance = Vector3.Distance(flag2.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                flagToBeMoved = flag1Distance < flag2Distance ? flag1 : flag2;
                holding = true;
                startingPosition = flagToBeMoved.transform.position;

            }
        }
        if (Input.GetMouseButtonUp(0)) //on release, stop moving flag
        {
            holding = false;
            lr.SetPosition(0, new Vector3(-10, -10, -10)); //moves line offscreen
            lr.SetPosition(1, new Vector3(-10, -10, -10));
            if (clicks > 2)
            {
                moveFlag(flagToBeMoved);
            }


        }

        if (holding) //if holding down, move current flag
        {
            //moveFlag(flagToBeMoved);
            lr.SetPosition(0, startingPosition);
            lr.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void moveFlag(GameObject flag)
    {
        //move flag to mouse's relative position
        flag.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        flag.transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y, 0); //add to place end of flag accurately
    }


}