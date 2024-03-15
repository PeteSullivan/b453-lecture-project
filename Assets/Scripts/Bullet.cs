using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    private Vector3 direction = new Vector3(0, 0, 0);
    private float speed = 0;
    private int color = 1;
    private float lifespan = 8f;
    private float lifetime = 0;
    private int damage = 0;


    // Update is called once per frame
    void Update()
    {

        transform.position  += direction * speed * Time.deltaTime; //move based on direction and speed

        lifetime += Time.deltaTime; //despawn after 8 seconds
        if (lifetime > lifespan)
        {
            Destroy(this.gameObject);
        }
    }

    public void setStats(float angle, float speed, int color, int damage, float lifespan, float size)
    {
        direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0); //calculate direction to move in
        this.speed = speed;
        this.color = color;
        this.damage = damage;
        this.lifespan = lifespan;
        transform.localScale = transform.localScale * size;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); //rotate bullet to face target


        switch (color) //change color based on original billion color
        {
            case 1:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case 2:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case 3:
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case 4:
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Billion"))
        {
            if (collision.gameObject.GetComponent<BillionMovement>().color != color)
            {
                //if you hit an enemy billion, it takes 25 damage.
                collision.gameObject.GetComponent<BillionMovement>().TakeDamage(damage);
            }
        }
        Destroy(this.gameObject); //despawn on collision regardless of what it hits
        
    }
}
