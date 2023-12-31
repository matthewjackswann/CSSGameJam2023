﻿using System;
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
        if (_host != null)
            _host.skilltree = this;
        else
        {
            _client.skilltree = this;
        }
       
        
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
        diseaseSpeeds[index] += 0.5f;
        float nspeed = diseaseSpeeds[index];
        print(nspeed);

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
        diseaseSizes[index] += 0.1f;
        float size = diseaseSizes[index];
        
        Contaminate[] arr = Object.FindObjectsOfType<Contaminate>();
        
        if (_client != null)
            _client.IncreaseSize(incomingDisease);
            
        else if (_host != null)
            _host.IncreaseSize(incomingDisease);
    }
    
    

    public void IncrementResistanceProbability2(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        resistanceProbs[index] += 0.1f;
            
    }

    public void IncrementInfectionProbability2(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        infectorProbs[index] += 0.1f;    
    }
    
    public void IncreaseSpeed2(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        diseaseSpeeds[index] += 0.5f;
        float nspeed = diseaseSpeeds[index];
        print(nspeed);

        Contaminate[] arr = Object.FindObjectsOfType<Contaminate>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].disease == incomingDisease)
            {
                arr[i].speed = nspeed;
            }
        }
        
    }

    public void IncreaseSize2(Disease incomingDisease)
    {
        int index = enums.IndexOf(incomingDisease);
        diseaseSizes[index] += 0.1f;
        float size = diseaseSizes[index];
        
        Contaminate[] arr = Object.FindObjectsOfType<Contaminate>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].disease == incomingDisease)
            {
                arr[i].transform.localScale = Vector3.one * size;
            }
        }
    }

    public float GetSpeed(Disease disease)
    {
        return diseaseSpeeds[enums.IndexOf(disease)];
    }
    
    public float GetSize(Disease disease)
    {
        return diseaseSizes[enums.IndexOf(disease)];
    }
}

//list goes - infection, resistance, speed, size


