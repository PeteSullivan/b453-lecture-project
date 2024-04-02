using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    //spawning billions
    [SerializeField] private GameObject billion;
    [SerializeField] private bool spawning = true;
    [SerializeField] private float spawnRate = 1;
    [SerializeField] private float spawnRange = .1f;
    [SerializeField] private float spawnTimer = 0;

    //turret
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] public int color;
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float firingSpeed;
    [SerializeField] private float range;
    private bool isAiming = false;
    private bool inRange = false;
    private float angle = 0f;
    private float shootTimer = 0f;

    [SerializeField] private GameObject healthbar;
    [SerializeField] private GameObject XPbar;
    [SerializeField] private TextMeshPro rankDisplay;
    private int health = 250;
    private int maxHealth = 250;
    private int xp = 0;
    private int maxXP = 100;
    private int rank = 1;




    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
        healthbar.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Arc1", 0); //start with full health
        XPbar.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Arc1", 360); //start with no XP

    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnRate)
            {
                //spawn billions randomly around the base
                float xNoise = Random.Range(-spawnRange, spawnRange);
                float yNoise = Mathf.Sqrt(spawnRange * spawnRange - (xNoise * xNoise));
                if (Random.Range(0, 1f) > 0.5f) { yNoise *= -1; }


                GameObject newBillion = Instantiate(billion, transform.position + new Vector3(xNoise, yNoise, 0), transform.rotation);

                //make new billion move based on goalAcceleration on billion prefab
                newBillion.GetComponent<BillionMovement>().SetRank(rank);
                spawnTimer = 0;
            }
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
            newBullet.transform.position += new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * 1.2f;

            //set bullet's stats
            newBullet.GetComponent<Bullet>().setStats(angle, bulletSpeed, color, 75, 10, 2.5f, this.gameObject);
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

        if (closestEnemy)
        {
            // Calculate the angle between current forward direction and direction to the target
            Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
            float goalAngle = Vector3.SignedAngle(transform.up, direction, Vector3.forward) * Mathf.Rad2Deg;
            angle = transform.rotation.eulerAngles.z + 90;

            // Rotate towards the target direction at a fixed speed
            if (goalAngle > 0)
            {
                transform.Rotate(Vector3.forward, Mathf.Min(goalAngle, turretRotationSpeed * Time.deltaTime));
            }
            else
            {
                transform.Rotate(Vector3.forward, Mathf.Max(goalAngle, -turretRotationSpeed * Time.deltaTime));

            }

            isAiming = true;
            inRange = (closestDistance < range) ? true : false; //check if in range

        }
        else
        {
            //if there are no billions, spin around in circles
            transform.Rotate(new Vector3(0, 0, turretRotationSpeed * Time.deltaTime));
            isAiming = false;
        }
        
        healthbar.transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);

    }
    public void TakeDamage(int damage, GameObject enemyBase)
    {
        health -= damage;
        if (health <= 0)
        {
            enemyBase.GetComponent<Base>().GetXP(50);
            Destroy(this.gameObject);
        }
        else
        {
            //Debug.Log("Health: " + health);

            //set healthbar size
            float arcSize = health * 360 / maxHealth;
            healthbar.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Arc1", 360 - arcSize);



        }
    }

    public void GetXP(int newXP)
    {

        xp += newXP;
        if (xp > maxXP) //rank up if enough XP
        {
            xp -= maxXP;
            rank++;
            maxXP = (int) (maxXP * 1.5f);
            rankDisplay.text = rank.ToString();
            Debug.Log("rank up! rank: " + rank);
            Debug.Log("new rank cost: " + maxXP);
        }
        //set XP bar size

        float arcSize = xp * 360 / maxXP;
        XPbar.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Arc1", 360 - arcSize);

    }

}
