using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
	Mesh mesh;
	Vector3[] vertices;
	public WaterSettings settings;
	int[] triangles;

	// Start is called before the first frame update
	void Start()
    {
		mesh = new Mesh();
		mesh.name = "Water Mesh";
		GetComponent<MeshFilter>().mesh = mesh;
		StartCoroutine(CreateShape());
	}

	IEnumerator CreateShape() {
		float xResolution = settings.actualSizeX / settings.sizeX;
		float zResolution = settings.actualSizeZ / settings.sizeZ;
		vertices = new Vector3[(settings.sizeX + 1) * (settings.sizeZ + 1)];
		int i = 0;
		for (int z = 0; z <= settings.sizeZ; z++) {
			for (int x = 0; x <= settings.sizeX; x++) {
				float realX = x * xResolution;
				float realZ = z * zResolution;
				vertices[i++] = new Vector3(realX, 0, realZ);
			}
		}
		triangles = new int[settings.sizeX * settings.sizeZ * 6];

		int vert = 0, tris = 0;
		for (int z = 0; z < settings.sizeZ; z++) {
			for (int x = 0; x < settings.sizeX; x++) {
				triangles[tris] = vert;
				triangles[tris + 1] = vert + settings.sizeX + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + settings.sizeX + 1;
				triangles[tris + 5] = vert + settings.sizeX + 2;
				vert++;
				tris += 6;
			}
			vert++;
			yield return new WaitForSeconds(float.MinValue);
		}
		UpdateMesh();
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	void UpdateMesh() {
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}
}
