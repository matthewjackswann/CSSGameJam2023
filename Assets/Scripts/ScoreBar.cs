using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBar : MonoBehaviour
{
    private Client _client;
    private Host _host;

    [SerializeReference] private Image red;
    [SerializeReference] private Image blue;

    private bool isHost;
    // Start is called before the first frame update
    void Start()
    {
        _client = FindObjectOfType<Client>();
        _host = FindObjectOfType<Host>();
        if (_host != null) isHost = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHost)
        {
            red.fillAmount = Mathf.Lerp(red.fillAmount, _host.red / (_host.red + _host.green + _host.blue), 0.7f);
            blue.fillAmount = Mathf.Lerp(blue.fillAmount, _host.blue / (_host.red + _host.green + _host.blue), 0.7f);
        }
        else
        {
            red.fillAmount = Mathf.Lerp(red.fillAmount, _client.red / (_client.red + _client.green + _client.blue), 0.7f);
            blue.fillAmount = Mathf.Lerp(blue.fillAmount, _client.blue / (_client.red + _client.green + _client.blue), 0.7f);
        }
    }
}
