using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contaminate : MonoBehaviour
{
    
    public float accelerationTime = 2f;
    public float speed = 5f;
    private Vector2 movement;
    private float timeLimit = 1000000000;
    public Disease disease = Disease.None;
    
    [SerializeReference] public Rigidbody2D rb;

    void Start()
    {
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    void FixedUpdate()
    {
        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        // update movement vector
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        // if a person see if you can infect them
        Contaminate cont = collision.gameObject.GetComponent<Contaminate>();
        if (cont != null)
        {
            if (cont.disease == Disease.None)
            {
                int probability = GetProbability();
            }
            else if (cont.disease == rb.gameObject.GetComponent<Contaminate>().disease)
            {
                int probability = GetProbability();
            }
            else
            {
                int probability = GetProbability();
            }
            
            
        }
        
        
        
        return;
    }

    int GetProbability()
    {
        return 1;
    }
}
