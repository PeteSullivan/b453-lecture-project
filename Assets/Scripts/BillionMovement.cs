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
    [SerializeField] private float stopMovingRange;

    private Vector3 goalPosition;
    private GameObject currentFlag;
    private float totalDistance;
    private Vector3 direction = new Vector3(0,0,0);
    public float velocity = 0;

    private Rigidbody2D rb;


    void Start()
    {

        rb = gameObject.GetComponent<Rigidbody2D>();
        float flag1Distance = Vector3.Distance(flag1.transform.position, transform.position);
        float flag2Distance = Vector3.Distance(flag2.transform.position, transform.position);
        currentFlag = (flag1Distance < flag2Distance) ? flag1 : flag2;
        goalPosition = currentFlag.transform.position;
        totalDistance = (flag1Distance < flag2Distance) ? flag1Distance : flag2Distance;
        velocity = 0;

    }

    // Update is called once per frame
    void Update()
    {
        updateCloseFlag();

        if (currentFlag.transform.position.z == 0) //checks to see if current flag has been placed
        {
            MoveToFlag();
        }

    }
    private void updateCloseFlag() 
    {
        
        float flag1Distance = Vector3.Distance(flag1.transform.position, transform.position);
        float flag2Distance = Vector3.Distance(flag2.transform.position, transform.position);
        float currentDistance = Vector3.Distance(currentFlag.transform.position, transform.position);
        if (currentFlag.transform.position != goalPosition)
        {
            currentFlag = (flag1Distance < flag2Distance) ? flag1 : flag2;
            goalPosition = currentFlag.transform.position;
            totalDistance = (flag1Distance < flag2Distance) ? flag1Distance : flag2Distance;
            Vector3 newDirection = (currentFlag.transform.position - this.transform.position).normalized;
            float angle = Vector3.Angle(direction, newDirection);
            velocity = (180 - angle) * velocity / 180;
        }
        else if (flag1Distance < currentDistance)
        {
            currentFlag = flag1;
            goalPosition = currentFlag.transform.position;
            totalDistance = flag1Distance;
            Vector3 newDirection = (currentFlag.transform.position - this.transform.position).normalized;
            float angle = Vector3.Angle(direction, newDirection);
            velocity = (180 - angle) * velocity / 180;

        }
        else if (flag2Distance < currentDistance)
        {
            currentFlag = flag2;
            goalPosition = currentFlag.transform.position;
            totalDistance = flag1Distance;
            Vector3 newDirection = (currentFlag.transform.position - this.transform.position).normalized;
            float angle = Vector3.Angle(direction, newDirection);
            velocity = (180 - angle) * velocity / 180;

        }

    }


    private void MoveToFlag()   
    {
        float remainingDistance = Vector3.Distance(currentFlag.transform.position, transform.position);
        direction = (currentFlag.transform.position - this.transform.position).normalized;

        if (remainingDistance < (totalDistance) / 2 )
        {
            velocity = remainingDistance;
            if (remainingDistance < stopMovingRange) { velocity = 0; }
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
