using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChunkLoader : MonoBehaviour {
	public GameObject prefab;
	public List<MeshCreator> chunks;
	public TerrainSettings settings;
	public Transform player;
	public Vector3 prevChunk, currentChunk;
	public int chunkRadius = 1;
	int chunksLoaded=0;

	void Start() {
		settings = prefab.GetComponent<MeshCreator>().settings;
		prevChunk = Vector3.zero;
		StartCoroutine(loadChunks());
		if (settings.texture == null) {
			settings.texture = new Texture2D(256, 1);
		}
		settings.material.SetVector("_MinMax", new Vector4(settings.minMax.Min, settings.minMax.Max));
		UpdateColors();
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
		foreach (MeshCreator chunk in chunks) {
			if (chunk.transform.position.x == point.x) {
				if (chunk.transform.position.z == point.z) {
					return true;
				}
			}
		}
		return false;
	}

	Vector3 getCurrentChunkPosition() {
		float x = player.position.x + (-player.position.x % settings.actualSizeX);
		float z = player.position.z + (-player.position.z % settings.actualSizeZ);
		if (player.position.x < 0) {
			x -= settings.actualSizeX;
		}
		if (player.position.z < 0) {
			z -= settings.actualSizeZ;
		}
		return new Vector3(x, 0, z);
	}

	Vector3 getChunk(int x, int z) {
		Vector3 current = getCurrentChunkPosition();
		float newX = current.x + (x * settings.actualSizeX);
		float newZ = current.z + (z * settings.actualSizeZ);
		return new Vector3(newX, 0, newZ);
	}

	bool chunkOutOfRange(float x, float z) {
		if (Mathf.Abs(Mathf.Abs(currentChunk.x) - Mathf.Abs(x)) >= (chunkRadius+1) * settings.actualSizeX ||
			Mathf.Abs(Mathf.Abs(currentChunk.z) - Mathf.Abs(z)) >= (chunkRadius+1) * settings.actualSizeZ) {
			return true;
		}
		return false;
	}

	IEnumerator UnloadChunks() {
		for (int i = chunksLoaded-1; i > 0; i--) {
			if (chunkOutOfRange(chunks[i].transform.position.x, chunks[i].transform.position.z)) {
				if (i < 0) {
					Debug.Log("i is negative " + i);
				}
				else if (i >= chunksLoaded) {
					Debug.Log("i is too big " + i + "  " + chunksLoaded);
				}
				MeshCreator toDestroy = chunks[i];
				chunks.RemoveAt(i);
				Destroy(toDestroy.gameObject);
				chunksLoaded--;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator loadChunks() {
		for (int z = -chunkRadius; z <= chunkRadius; z++) {
			for (int x = -chunkRadius; x <= chunkRadius; x++) {
				Vector3 pos = getChunk(x, z);
				if (!isChunkOnPosition(pos)) {
					GameObject newChunk = Instantiate(prefab, pos, Quaternion.identity, transform);
					chunks.Add(newChunk.GetComponent<MeshCreator>());
					chunksLoaded++;
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		UpdateColors();
	}

	public void UpdateColors() {
		Color[] colors = new Color[settings.textureResolution];
		for (int i = 0; i < settings.textureResolution; i++) {
			colors[i] = settings.gradient.Evaluate(i / (settings.textureResolution- 1f));
		}
		settings.texture.SetPixels(colors);
		settings.texture.Apply();
		settings.material.SetTexture("_texture", settings.texture);
	}

	public float getElevationAtPoint(float x, float z) {
		return settings.getElevationAtPoint(x,z);
	}

	private void OnDrawGizmos() {
		Gizmos.DrawLine(currentChunk,player.position);
	}
}
