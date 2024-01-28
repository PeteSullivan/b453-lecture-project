using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private GameObject billion;
    [SerializeField] private bool spawning = true;
    [SerializeField] private float spawnRate = 1;
    [SerializeField] private float spawnRange = .5f;
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
                
                float xNoise = Random.Range(-spawnRange, spawnRange);
                float yNoise = Mathf.Sqrt(spawnRange * spawnRange - (xNoise * xNoise));
                if (Random.Range(0, 1f) > 0.5f) { yNoise *= -1; }
                Instantiate(billion, transform.position + new Vector3(xNoise, yNoise, 0), transform.rotation);
                timer = 0;
            }
        }
    }
}
