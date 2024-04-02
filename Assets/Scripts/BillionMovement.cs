using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BillionMovement : MonoBehaviour
{
    //references
    private Rigidbody2D rb;
    [SerializeField] public int color;
    [SerializeField] private Sprite[] spriteColors;
    [SerializeField] private GameObject homeBase;

    //finding how to move
    [SerializeField] private GameObject flag1;
    [SerializeField] private GameObject flag2;
    [SerializeField] private float accelerationWhileInPlay;
    [SerializeField] private float stopMovingRange;

    //turret/bullet stuff
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float firingSpeed;
    [SerializeField] private float range;
    private bool isAiming = false;
    private bool inRange = false;
    private float angle = 0f;
    private float shootTimer = 0f;
    

    //movement
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
    private int damage = 25;





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


        foreach (GameObject billionBase in GameObject.FindGameObjectsWithTag("Base"))
        {
            if (color == billionBase.GetComponent<Base>().color)
            {
                homeBase = billionBase;
                break;
            }
        }


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
        Shoot();


    }

    private void Shoot()
    {
        //only shoots if there's a billion in range, it is aiming at it, and it's off cooldown.
        shootTimer += Time.deltaTime;
        if (isAiming && inRange && shootTimer > firingSpeed)
        {
            shootTimer = 0;

            //instantiate new bullet and place it in front of the turret's current aiming angle
            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.transform.position = transform.position;
            newBullet.transform.position += new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * 0.3f;

            //set bullet's stats
            newBullet.GetComponent<Bullet>().setStats(angle, bulletSpeed, color, damage, 4, 1, homeBase);
        }
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

        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
        foreach (GameObject billionBase in bases) 
        {
            if (color != billionBase.GetComponent<Base>().color)
            {

                float distance = Vector3.Distance(transform.position, billionBase.transform.position);
                if (distance < closestDistance)
                {

                    //if billion is a different color, not a starting billion, AND is closest, set it as the closest
                    closestDistance = Vector3.Distance(transform.position, billionBase.transform.position);
                    closestEnemy = billionBase;
                }
            }
        }


        if (closestEnemy)
        {
            //if there are billions, find the angle to face it and rotate towards that angle
            angle = Mathf.Atan2(closestEnemy.transform.position.y - transform.position.y, closestEnemy.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = rotation;
            isAiming = true;
            inRange = (closestDistance < range) ? true : false;
            

        }
        else
        {
            //if there are no billions, spin around
            transform.Rotate(new Vector3(0, 0, turretRotationSpeed * 2 * Time.deltaTime));
            isAiming = false;
        }

    }

    public void SetRank(int rank)
    {
        //keep reference billions offscreen
        acceleration = accelerationWhileInPlay;

        //Debug.Log("new billion rank: " + rank);

        //scale health, damage, and size based on rank
        maxHealth = maxHealth * (rank / 2 + 1);
        damage = damage * (rank / 2 + 1);
        transform.localScale += new Vector3((float)rank / 10, (float) rank / 10, (float) rank / 10);

    }



    public bool TakeDamage(int damage, GameObject enemyBase)
    {
        health -= damage;
        if (health <= 0)
        {
            if (enemyBase)
                enemyBase.GetComponent<Base>().GetXP(25);
            Destroy(this.gameObject);
        }
        else
        {
            //resize healthbar based on percentage of health left
            float newSize = Mathf.Lerp(.2f, 1f, (float) health / (float) maxHealth);
            //Debug.Log(newSize);
            HealthBar.transform.localScale = new Vector3(newSize, newSize, 1);
            return false;
        }
        return true;
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
