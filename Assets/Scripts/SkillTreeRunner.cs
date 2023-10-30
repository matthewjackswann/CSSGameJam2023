using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SkillTreeRunner : MonoBehaviour
{
    public List<float> resistanceProbs;
    public List<float> infectorProbs;
    public List<float> diseaseSpeeds;
    public List<float> diseaseSizes;
    public List<Disease> enums = new List<Disease>()
    {
        Disease.None, Disease.Blue, Disease.Red
    };

    [SerializeReference] private Client _client;
    [SerializeReference] private Host _host;
    

    public SkillTreeRunner()
    {
        resistanceProbs = new List<float>();
        infectorProbs = new List<float>();
        diseaseSizes= new List<float>();
        diseaseSpeeds = new List<float>();
    }

    private void Start()
    {
        _client = FindObjectOfType<Client>();
        _host = FindObjectOfType<Host>();
    }

    public void IncrementResistanceProbability(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        resistanceProbs[index] += 0.1f;
        if (_client != null)
            _client.IncrementResistanceProbability(incomingDisease);
            
        else if (_host != null)
            _host.IncrementResistanceProbability(incomingDisease);
            
    }

    public void IncrementInfectionProbability(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        infectorProbs[index] += 0.1f;    
        if (_client != null)
            _client.IncrementInfectionProbability(incomingDisease);
            
        else if (_host != null)
            _host.IncrementInfectionProbability(incomingDisease);
    }
    
    public void IncreaseSpeed(Disease incomingDisease)
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
        
        if (_client != null)
            _client.IncreaseSpeed(incomingDisease);
            
        else if (_host != null)
            _host.IncreaseSpeed(incomingDisease);
        
    }

    public void IncreaseSize(Disease incomingDisease)
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
        if (_client != null)
            _client.IncreaseSize(incomingDisease);
            
        else if (_host != null)
            _host.IncreaseSize(incomingDisease);
    }
    
}

//list goes - infection, resistance, speed, size


