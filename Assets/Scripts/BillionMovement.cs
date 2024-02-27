using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BillionMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public int color;
    [SerializeField] private Sprite[] spriteColors;

    //finding how to move
    [SerializeField] private GameObject flag1;
    [SerializeField] private GameObject flag2;
    [SerializeField] private float accelerationWhileInPlay;
    [SerializeField] private float stopMovingRange;
    [SerializeField] private float turretRotationSpeed;

    private Vector3 goalPosition;
    private GameObject currentFlag;
    private float totalDistance;
    private Vector3 direction = new Vector3(0,0,0);
    public float velocity = 0;
    private float acceleration = 0;


    //health
    private Transform HealthBar;
    private int health;
    private int maxHealth = 100;



    void Start()
    {

        //start flag movement
        rb = gameObject.GetComponent<Rigidbody2D>();
        float flag1Distance = Vector3.Distance(flag1.transform.position, transform.position);
        float flag2Distance = Vector3.Distance(flag2.transform.position, transform.position);
        currentFlag = (flag1Distance < flag2Distance) ? flag1 : flag2;
        goalPosition = currentFlag.transform.position;
        totalDistance = (flag1Distance < flag2Distance) ? flag1Distance : flag2Distance;
        velocity = 0;


        //set health variables
        health = maxHealth;
        HealthBar = transform.GetChild(0);
        HealthBar.GetComponent<SpriteRenderer>().sprite = spriteColors[color];
                    
        



    }

    // Update is called once per frame
    void Update()
    {
        updateCloseFlag();

        if (currentFlag.transform.position.z == 0) //checks to see if current flag has been placed
        {
            MoveToFlag();
        }
        AimTurret();


    }

    private void AimTurret()
    {
        GameObject[] billions = GameObject.FindGameObjectsWithTag("Billion");
        float closestDistance = 9999;
        GameObject closestEnemy = null;
        
        foreach (GameObject billion in billions) //loop through all billions in scene
        {
            if (color != billion.GetComponent<BillionMovement>().color && !billion.transform.parent)
            {
               
                float distance = Vector3.Distance(transform.position, billion.transform.position);
                if (distance < closestDistance)
                {
                    //if billion is a different color, not a starting billion, AND is closest, set it as the closest
                    closestDistance = Vector3.Distance(transform.position, billion.transform.position);
                    closestEnemy = billion;
                }
                
            }
        }
        if (closestEnemy)
        {
            //if there are billions, find the angle to face it and rotate towards that angle
            float angle = Mathf.Atan2(closestEnemy.transform.position.y - transform.position.y, closestEnemy.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = rotation;
        }
        else
        {
            //if there are no billions, spin around
            transform.Rotate(new Vector3(0, 0, turretRotationSpeed * 2 * Time.deltaTime));
        }

    }

    public void SetAcceleration()
    {
        //original reference don't move
        Debug.Log("set");
        acceleration = accelerationWhileInPlay;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Billion"))
        {
            if (collision.gameObject.GetComponent<BillionMovement>().color != color)
            {
                //if you hit an enemy billion, take 25 damage.
                TakeDamage(25);
            }
        }
    }

   
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //resize healthbar based on percentage of health left
            float newSize = Mathf.Lerp(.2f, 1f, (float) health / (float) maxHealth);
            Debug.Log(newSize);
            HealthBar.transform.localScale = new Vector3(newSize, newSize, 1);

        }
    }




    private void updateCloseFlag() 
    {
        //find closest flag
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
