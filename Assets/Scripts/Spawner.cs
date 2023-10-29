using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    private List<GameObject> blobs;
    public Bounds spawnBox;
    public Disease initialDisease = Disease.None;

    public uint initialCount = 25;
    
    // Start is called before the first frame update
    void Start()
    {
        blobs = new List<GameObject>();
        for (int i = 0; i < initialCount; i++)
        {
            Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Spawn()
    {
        // Creates a new blob
        GameObject newBlob = Instantiate(this.prefab, this.gameObject.transform);
        
        // Assigns the new blob a random pooint in the spawn box of the spawner
        float x = Random.Range(spawnBox.center.x - spawnBox.extents.x, spawnBox.center.x + spawnBox.extents.x);
        float y = Random.Range(spawnBox.center.y - spawnBox.extents.y, spawnBox.center.y + spawnBox.extents.y);
        newBlob.transform.SetPositionAndRotation(
            new Vector3(x, y, newBlob.transform.position.z),
            newBlob.transform.rotation);
        
        // Gives the blob an initial disease
        newBlob.GetComponent<Contaminate>().disease = this.initialDisease;
        
        // Keeps a reference of the blob
        this.blobs.Add(newBlob);
    }
}
