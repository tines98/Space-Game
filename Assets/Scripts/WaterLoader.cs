using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLoader : MonoBehaviour
{
	public WaterSettings settings;
	public GameObject prefab;
	public List<GameObject> chunks;
	public Transform player;
	public Vector3 prevChunk, currentChunk;
	public int chunkRadius = 1;
	int chunksLoaded = 0;

	void Start() {
		prevChunk = Vector3.zero;
		StartCoroutine(loadChunks());
	}

	void Update() {
		currentChunk = getCurrentChunkPosition();
		if (prevChunk != currentChunk) {
			prevChunk = currentChunk;
			StartCoroutine(UnloadChunks());
			StartCoroutine(loadChunks());
		}
	}

	bool isChunkOnPosition(Vector3 point) {
		//foreach (WaterChunk chunk in chunks) {
		foreach (GameObject chunk in chunks) {
			if (chunk.transform.position.x == point.x) {
				if (chunk.transform.position.z == point.z) {
					return true;
				}
			}
		}
		return false;
	}

	/*
	Vector3 getCurrentChunkPosition() {
		float x = player.position.x + (-player.position.x % settings.actualSizeX);
		float z = player.position.z + (-player.position.z % settings.actualSizeZ);
		if (player.position.x < 0) {
			x -= settings.actualSizeX;
		}
		if (player.position.z < 0) {
			z -= settings.actualSizeZ;
		}
		return new Vector3(x, settings.height, z);
	}
	*/

	Vector3 getCurrentChunkPosition() {
		//BYTT UT MESH MED PLANE FOR Å TESTE OM DET VIL FUNKE. PLANE ER 10 stor, MULTIPLY MED TING FOR Å FÅ 256 elns
		float x = player.position.x + (-player.position.x % settings.actualSizeX);
		float z = player.position.z + (-player.position.z % settings.actualSizeZ);
		if (player.position.x < 0) {
			x -= settings.actualSizeX;
		}
		if (player.position.z < 0) {
			z -= settings.actualSizeZ;
		}
		return new Vector3(x, settings.height, z);
	}

	Vector3 getChunk(int x, int z) {
		Vector3 current = getCurrentChunkPosition();
		float newX = current.x + (x * settings.actualSizeX);
		float newZ = current.z + (z * settings.actualSizeZ);
		return new Vector3(newX, settings.height, newZ);
	}

	bool chunkOutOfRange(float x, float z) {
		if (Mathf.Abs(Mathf.Abs(currentChunk.x) - Mathf.Abs(x)) >= (chunkRadius + 1) * settings.actualSizeX ||
			Mathf.Abs(Mathf.Abs(currentChunk.z) - Mathf.Abs(z)) >= (chunkRadius + 1) * settings.actualSizeZ) {
			return true;
		}
		return false;
	}

	IEnumerator UnloadChunks() {
		for (int i = chunksLoaded - 1; i >= 0; i--) {
			if (chunkOutOfRange(chunks[i].transform.position.x, chunks[i].transform.position.z)) {
				GameObject toDestroy = chunks[i];
				chunks.RemoveAt(i);
				Destroy(toDestroy.gameObject);
				chunksLoaded--;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator loadChunks() {
		Vector3 scale = new Vector3(settings.actualSizeX / 10f, 1, settings.actualSizeZ / 10f);
		for (int z = -chunkRadius; z <= chunkRadius; z++) {
			for (int x = -chunkRadius; x <= chunkRadius; x++) {
				Vector3 pos = getChunk(x, z);
				if (!isChunkOnPosition(pos)) {
					GameObject newChunk = Instantiate(prefab, pos, Quaternion.identity, transform);
					newChunk.transform.localScale = scale;
					chunks.Add(newChunk);//.GetComponent<WaterChunk>());
					chunksLoaded++;
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
