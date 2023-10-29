using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Contaminate : MonoBehaviour
{
    
    public float speed = 2.5f;
    private Vector2 movement;
    private float timeLimit = 1000000000;
    public Disease disease = Disease.None;
    public float infectionProb = 0;
    
    [SerializeReference] public Rigidbody2D rb;

    void Start()
    {
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        UpdateColour();
    }

    void FixedUpdate()
    {
        rb.velocity = movement * speed;
    }

    void UpdateColour()
    {
        if (disease == Disease.None)
        {
            // green
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (disease == Disease.Blue)
        {
            // blue
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            // red
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // update movement vector
        movement = new Vector2(-movement.x + Random.Range(-1f, 1f), -movement.y +Random.Range(-1f, 1f));

        // if a person see if you can infect them
        Contaminate cont = collision.gameObject.GetComponent<Contaminate>();
        if (cont != null)
        {
            if (cont.disease == Disease.None)
            {
                int probability = GetProbability();
                // do nothing, the other person has not given you a disease.
            }
            else if (cont.disease == disease)
            {
                int probability = GetProbability();
                // do nothing, the other person has the same disease as you.
            }
            else
            {
                int probability = GetProbability();
                if (probability > infectionProb)
                {
                    disease = cont.disease;
                    UpdateColour();
                    Points.Instance.AddPoints(1);
                }
            }
            
            
        }
        
        
        
        return;
    }

    int GetProbability()
    {
        return 1;
    }
}
