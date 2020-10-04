using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnObject))]
public class Spawner : Editor
{
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		SpawnObject spawnObject = (SpawnObject)target;
		if (GUILayout.Button("Spawn")) {
			spawnObject.Spawn();
		}
		if (GUILayout.Button("Spawn Random")) {
			spawnObject.SpawnRandomWithin();
		}
	}
}
