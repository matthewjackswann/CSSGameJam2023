using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
    [SerializeReference] private Client _client;
    [SerializeReference] private Host _host;

    private Camera mainCamera;

    void Start()
    {
        _client = FindObjectOfType<Client>();
        _host = FindObjectOfType<Host>();
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        mainCamera = Camera.main;
        UpdateColour();
    }

    void FixedUpdate()
    {
        rb.velocity = movement * speed;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if ((transform.position.x - worldPosition.x) * (transform.position.x - worldPosition.x) +
                (transform.position.y - worldPosition.y) * (transform.position.y - worldPosition.y) < 0.5)
            {
                if (_host != null)
                {
                    if (Disease.Red == disease) return;
                    // We play the infection particle effect
                    this.GetComponent<ParticleSystem>().Stop();
                    this.GetComponent<ParticleSystem>().Play();
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    if (disease == Disease.None) g--;
                    if (disease == Disease.Blue) b--;

                    r++;
                    _host.Infection(r, g, b);
                    disease = Disease.Red;
                    UpdateColour();
                } else if (_client != null)
                {
                    if (Disease.Blue == disease) return;
                    // We play the infection particle effect
                    this.GetComponent<ParticleSystem>().Stop();
                    this.GetComponent<ParticleSystem>().Play();
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    if (disease == Disease.Red) r--;
                    if (disease == Disease.None) g--;

                    b++;
                    _client.Infection(r, g, b);
                    disease = Disease.Blue;
                    UpdateColour();
                }
            }
        }


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
        Vector2 normal = collision.contacts[0].normal;


        // update movement vector
        movement = new Vector2(movement.x + Random.Range(-1f, 1f), movement.y +Random.Range(-1f, 1f)).normalized;
        Vector2 rNorm = Vector2.Reflect(movement,normal);
        movement = rNorm;

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
                    if (cont.disease == disease) return;

                    // We play the infection particle effect
                    this.GetComponent<ParticleSystem>().Stop();
                    this.GetComponent<ParticleSystem>().Play();

                    int r = 0;
                    int g = 0;
                    int b = 0;
                    if (disease == Disease.Red) r--;
                    if (disease == Disease.None) g--;
                    if (disease == Disease.Blue) b--;

                    if (cont.disease == Disease.Red) r++;
                    if (cont.disease == Disease.None) g++;
                    if (cont.disease == Disease.Blue) b++;
                    if (_client != null)
                        _client.Infection(r, g, b);
                    else if (_host != null)
                        _host.Infection(r, g, b);
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
