using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
	public Transform midPoint, radiusPoint;
	public Stats stats;
	public Transform hitPoint;
	float size;
	int numOfParticles = 0;
	public ChunkLoader chunkLoader;
	ParticleSystem part;
	ParticleSystem.Particle[] particles;

	// Start is called before the first frame update
	void Start()
	{
		size = (midPoint.position - radiusPoint.position).magnitude;
		Move();
	}

	// Update is called once per frame
	void Update()
	{
		if (part != null) {
			transform.LookAt(part.transform);
		}
	}

	private void OnParticleCollision(GameObject other) {
		Debug.Log("pew");
		if (other.tag == "Laser") {
			part = other.GetComponent<ParticleSystem>();
			particles = new ParticleSystem.Particle[part.main.maxParticles];
			numOfParticles = part.GetParticles(particles);
			for (int i = 0; i < numOfParticles; i++) {
				int scoreUp = getScore(particles[i].position);
				if (scoreUp > 0) {
					stats.Score += scoreUp;
					hitPoint.gameObject.SetActive(true);
					hitPoint.position = particles[i].position;
					Move();
					return;
				}
			}
		}
	}

	private void Move() {
		transform.position += Random.onUnitSphere * Random.Range(50, 200);
		float y = chunkLoader.getElevationAtPoint(transform.position.x, transform.position.z);
		Vector3 newPos = new Vector3(transform.position.x, y+20f, transform.position.z);
		transform.position = newPos;
		hitPoint.gameObject.SetActive(false);
	}

	private int getScore(Vector3 point) {
		float distanceToMid = (point-midPoint.position).magnitude;
		int score;
		if (distanceToMid <= .25f * size) {
			score = 10;
		}
		else if (distanceToMid <= .5f * size) {
			score =6;
		}
		else if (distanceToMid <= .75f * size) {
			score = 3;
		}
		else if (distanceToMid <= size) {
			score = 1;
		}
		else {
			score = 0;
		}
		return score;
	}
}
