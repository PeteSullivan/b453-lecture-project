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
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(color))
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
        if (Input.GetMouseButtonUp(color)) //on release, stop moving flag
        {
            holding = false;
            lr.SetPosition(0, new Vector3(-10, -10, -10)); //moves line offscreen
            lr.SetPosition(1, new Vector3(-10, -10, -10));


        }

        if (holding) //if holding down, move current flag
        {
            moveFlag(flagToBeMoved);
            lr.SetPosition(0, startingPosition - new Vector3(.35f, .35f, 0));
            lr.SetPosition(1, flagToBeMoved.transform.position - new Vector3(.35f, .35f, 0));
        }

    }

    private void moveFlag(GameObject flag)
    {
        //move flag to mouse's relative position
        flag.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        flag.transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y, 0) + new Vector3(.35f, .35f, 0); //add to place end of flag accurately
    }


}