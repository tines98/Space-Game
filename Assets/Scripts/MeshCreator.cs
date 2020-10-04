using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
	Mesh mesh;
	public TerrainSettings settings;
	Vector3[] vertices;
	int[] triangles;
	public bool update = false;
	public float xResolution, zResolution;

	// Start is called before the first frame update
	void Awake()
	{
		xResolution = (float)settings.actualSizeX / (float)settings.xSize;
		zResolution = (float)settings.actualSizeZ / (float)settings.zSize;
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		StartCoroutine(CreateShape());
		//CreateShape();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateMesh();
		if (update) {
			Reconstruct();
			update = false;
		}
	}

	public void Reconstruct() {
		xResolution = (float)settings.actualSizeX / (float)settings.xSize;
		zResolution = (float)settings.actualSizeZ / (float)settings.zSize;

		StartCoroutine(CreateShape());
		//CreateShape();
		UpdateMesh();
	}

	//IEnumerator CreateShape() {
	IEnumerator CreateShape() {
		vertices = new Vector3[(settings.xSize + 1) * (settings.zSize + 1)];
		int i = 0;
		for (int z = 0; z <= settings.zSize; z++) {
			for (int x = 0; x <= settings.xSize; x++) {
				float realX = x * xResolution;
				float realZ = z * zResolution;
				float y = settings.getElevationAtPoint(transform.position.x + realX, transform.position.z + realZ);
				vertices[i++] = new Vector3(realX, y, realZ);
			}
		}
		triangles = new int[settings.xSize * settings.zSize * 6];

		int vert = 0, tris = 0;
		for (int z = 0; z < settings.zSize; z++) {
			for (int x = 0; x < settings.xSize; x++) {
				triangles[tris] = vert;
				triangles[tris + 1] = vert + settings.xSize + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + settings.xSize + 1;
				triangles[tris + 5] = vert + settings.xSize + 2;
				vert++;
				tris += 6;
			}
			vert++;
			yield return new WaitForSeconds(float.MinValue);
		}
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	

	void UpdateMesh() {
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		settings.UpdateColors();
		//GetComponent<MeshCollider>().sharedMesh = mesh;
	}
	/*
	private void OnDrawGizmos() {
		if (vertices == null)
			return;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(vertices[i], .1f);
		}
	}
	*/
	
}
