using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Points : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    public static Points Instance {get; private set;}

    public int points {
        get {
            if (client != null) {
                return client.money;
            }
            return host.money;
        }
        set {
            if (client != null) {
                client.money = value;
                return;
            }
            host.money = value;
        }
    }

    private Client client;
    private Host host;

    private void Start()
    {
        Instance = this;
        client = FindObjectOfType<Client>();
        host = FindObjectOfType<Host>();
        pointsText.text = points.ToString();
    }


    public bool TrySpend(int cost) 
    {
        if (points >= cost) 
        {
            points -= cost;
            pointsText.text = points.ToString();
            return true;
        }

        return false;
    }
}
