using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BillionMovement : MonoBehaviour
{
    //finding how to move
    [SerializeField] private GameObject flag1;
    [SerializeField] private GameObject flag2;
    [SerializeField] private float acceleration;
    
    private Vector3 flag1Position;
    private Vector3 flag2Position;
    private GameObject closerFlag;
    private float totalDistance;
    private Vector3 direction = new Vector3(0,0,0);
    public float velocity = 0;

    private Rigidbody2D rb;


    void Start()
    {

        rb = this.gameObject.GetComponent<Rigidbody2D>();
        flag1Position = flag1.transform.position;
        flag2Position = flag2.transform.position;
        float flag1Distance = Vector3.Distance(flag1Position, transform.position);
        float flag2Distance = Vector3.Distance(flag2Position, transform.position);
        closerFlag = (flag1Distance < flag2Distance) ? flag1 : flag2;
        totalDistance = (flag1Distance < flag2Distance) ? flag1Distance : flag2Distance;

    }

    // Update is called once per frame
    void Update()
    {
        //find closer flag if a flag was moved
        if (flag1.transform.position != flag1Position || flag2.transform.position != flag2Position)
        {
            flag1Position = flag1.transform.position;
            flag2Position = flag2.transform.position;
            float flag1Distance = Vector3.Distance(flag1Position, transform.position);
            float flag2Distance = Vector3.Distance(flag2Position, transform.position);
            closerFlag = (flag1Distance < flag2Distance) ? flag1 : flag2;
            totalDistance = (flag1Distance < flag2Distance) ? flag1Distance : flag2Distance;
        }

        MoveToFlag();
        
    }
    private void MoveToFlag()   
    {
        float remainingDistance = Vector3.Distance(closerFlag.transform.position, transform.position);
        direction = (closerFlag.transform.position - this.transform.position).normalized;

        if (remainingDistance < (totalDistance) / 2 )
        {
            velocity -= velocity * 0.9f * Time.deltaTime;
            transform.position += direction * velocity * Time.deltaTime;
                
        }
        else
        {
            velocity += acceleration * Time.deltaTime;
            transform.position += direction * velocity * Time.deltaTime;

            //transform.position = closerFlag.transform.position;

        }

    }
}
