using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainSettings : ScriptableObject {

	public int xSize = 20;
	public int zSize = 20;
	public int actualSizeX = 10;
	public int actualSizeZ = 10;
	public float height = 0;
	public NoiseSetting[] noiseSettings;
	[HideInInspector]
	public MinMax minMax = new MinMax();
	public Material material;
	public Gradient gradient;
	public Texture2D texture;
	public bool useFirstLayerAsMask = true;
	public int textureResolution = 50;

	public float getElevationAtPoint(float x, float z) {
		float y = height;
		float firstMask = 0;
		Vector2 noiseOffset = NoiseOffset(x, z, 0);
		y += doNoise(noiseOffset, 0);
		firstMask = y;
		for (int layer = 1; layer < noiseSettings.Length; layer++) {
			if (useFirstLayerAsMask) {
				noiseOffset = NoiseOffset(x, z, layer);
				y += doNoise(noiseOffset, layer) * firstMask;
			}
			else {
				noiseOffset = NoiseOffset(x, z, layer);
				y += doNoise(noiseOffset, layer);
			}
		}
		minMax.AddValue(y);
		return y;
	}

	float doNoise(Vector2 point, int layer) {
		float noise = Mathf.PerlinNoise(
			point.x + noiseSettings[layer].xOffset,
			point.y + noiseSettings[layer].zOffset
		);
		noise *= noiseSettings[layer].strength;
		noise = Mathf.Max(0, noise - noiseSettings[layer].minValue);
		noise = Mathf.Min(noise, noiseSettings[layer].maxValue);
		noise *= noiseSettings[layer].multiplier;
		return noise + noiseSettings[layer].height;
	}

	private Vector2 NoiseOffset(float x, float z, int layer) {
		Vector2 noiseOffset = new Vector2(
			(x * noiseSettings[layer].roughness) % 256f,
			(z * noiseSettings[layer].roughness) % 256f
			);
		if (noiseOffset.x < 0f) {
			noiseOffset = new Vector2(noiseOffset.x + 256f, noiseOffset.y);
		}
		if (noiseOffset.y < 0f) {
			noiseOffset = new Vector2(noiseOffset.x, noiseOffset.y + 256f);
		}
		return noiseOffset;
	}

	public void UpdateColors() {
		Color[] colors = new Color[textureResolution];
		for (int i = 0; i < textureResolution; i++) {
			colors[i] = gradient.Evaluate(i / (textureResolution - 1f));
		}
		texture.SetPixels(colors);
		texture.Apply();
		material.SetTexture("_texture", texture);
		material.SetVector("_MinMax", new Vector4(minMax.Min, minMax.Max, 0, 0));
	}

	[System.Serializable]
	public class NoiseSetting {
		public float xOffset = 0f;
		public float zOffset = 0f;
		public float roughness = 1f;
		public float strength = 1f;
		public float minValue = 0.25f;
		public float maxValue = 100f;
		public float multiplier = 1f;
		public float height = 0;
	}
}
