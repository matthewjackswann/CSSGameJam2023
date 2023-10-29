using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private bool isHost;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Contaminate c = collision.gameObject.GetComponent<Contaminate>();
        if (c == null) return;

        var o = collision.gameObject;
        float y = o.transform.position.y;
        Vector2 movement = c.movement;
        Disease d = c.disease;

        Message m = new Message(y, movement, d);
        Destroy(o);

        if (isHost)
        {
            Host host = FindObjectOfType<Host>();
            // send msg from host to client
            host.SendMessage(m);
        }
        else
        {
            Client client = FindObjectOfType<Client>();
            // send msg from client to host
            client.SendMessage(m);
        }
    }
}
