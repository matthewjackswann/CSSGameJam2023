using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Contaminate : MonoBehaviour
{

    public float speed = 2.5f;
    public Vector2 movement;
    private float timeLimit = 1000000000;
    public Disease disease = Disease.None;
    public float infectionProb = 0;
    public List<float> resistanceProbs;
    public List<float> infectorProbs;
    public List<float> diseaseSpeeds;
    public List<float> diseaseSizes;

    public List<Disease> enums = new List<Disease>()
    {
        Disease.None, Disease.Blue, Disease.Red
    };



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
                float probability = GetProbability(disease, cont.disease);
                // do nothing, the other person has not given you a disease.
            }
            else if (cont.disease == disease)
            {
                float probability = GetProbability(disease, cont.disease);
                // do nothing, the other person has the same disease as you.
            }
            else
            {
                float probability = GetProbability(disease, cont.disease);
                if (probability > infectionProb)
                {
                    disease = cont.disease;
                    UpdateColour();
                }
            }
        }
    }

    void IncrementResistanceProbability(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        resistanceProbs[index] += 0.1f;
    }

    void IncrementInfectionProbability(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        infectorProbs[index] += 0.1f;    }
    void IncreaseSpeed(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        float nspeed = diseaseSpeeds[index];
        Contaminate[] arr = Object.FindObjectsOfType<Contaminate>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].disease == incomingDisease)
            {
                arr[i].speed = nspeed;
            }
        }
    }

    void IncreaseSize(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        float size = diseaseSizes[index];
        Contaminate[] arr = Object.FindObjectsOfType<Contaminate>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].disease == incomingDisease)
            {
                arr[i].rb.gameObject.transform.localScale *= size;
            }
        }
    }

    float GetProbability(Disease infector, Disease infectee)
    {
        return 1;
        int infectorIndex = enums.IndexOf(infector);
        int infecteeIndex = enums.IndexOf(infectee);

        float lim = infectorProbs[infectorIndex] - resistanceProbs[infecteeIndex];

        return Random.Range(lim,1);
    }
}
