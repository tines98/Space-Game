using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject toSpawn;
    public Vector3 location;
    public ChunkLoader chunkLoader;
    public Transform spawnNear;
    public float minRange = 1f, maxRange = 10f;
    
    public void Spawn() {
        float y = chunkLoader.getElevationAtPoint(location.x, location.z);
        Vector3 newLoc = new Vector3(location.x, location.y + y, location.z);
        Instantiate(toSpawn, newLoc, Quaternion.identity);
	}

    public void SpawnRandomWithin() {
        Vector3 randPos = spawnNear.position + (Random.insideUnitSphere * Random.Range(minRange,maxRange));
        float y = chunkLoader.getElevationAtPoint(randPos.x, randPos.z) + location.y;
        randPos.y = y;
        Instantiate(toSpawn, randPos, Quaternion.identity);
    }
}
