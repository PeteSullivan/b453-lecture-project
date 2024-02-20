using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private GameObject billion;
    [SerializeField] private bool spawning = true;
    [SerializeField] private float spawnRate = 1;
    [SerializeField] private float spawnRange = .1f;
    [SerializeField] private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            timer += Time.deltaTime;
            if (timer > spawnRate)
            {
                //spawn billions randomly around the base
                float xNoise = Random.Range(-spawnRange, spawnRange);
                float yNoise = Mathf.Sqrt(spawnRange * spawnRange - (xNoise * xNoise));
                if (Random.Range(0, 1f) > 0.5f) { yNoise *= -1; }


                GameObject newBillion = Instantiate(billion, transform.position + new Vector3(xNoise, yNoise, 0), transform.rotation);

                //make new billion move based on goalAcceleration on billion prefab
                newBillion.GetComponent<BillionMovement>().SetAcceleration(); 
                timer = 0;
            }
        }
    }
}
