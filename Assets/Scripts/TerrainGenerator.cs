﻿using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {
	public GameObject chunkPrefab;
	public static List<TerrainChunk> spawnedChunks = new List<TerrainChunk>();
	public static List<Vector3> validChunkPosns = new List<Vector3>();
	public static float randZ;
	public static Vector3 terrainOrigin;
	public static int globalTimer = 0;
	
	public static float GetGlobalNoise(float x, float y, float z){
		return SimplexNoise.Noise.Generate (x / 40f, y / 40f, z) / 2f;
	}
	
	public static float GetGlobalNoiseFringe(float x, float y){
		return SimplexNoise.Noise.Generate (x / 5f, y / 5f, 0) / 2f;
	}
	// Use this for initialization
	void Start () {
		randZ = Random.Range (0, 100000);
		float offset = -TerrainChunk.mapWidth * TerrainChunk.tileSize / 2f;
		Vector3 firstChunkPos = new Vector3(offset, -offset, 0);
		terrainOrigin = firstChunkPos;
		GameObject spawnedChunk = Instantiate(chunkPrefab, firstChunkPos, Quaternion.identity) as GameObject;
		TerrainChunk firstChunk = spawnedChunk.GetComponent<TerrainChunk>();
		//validChunkPosns.Add(spawnedChunk.transform.position);
		spawnedChunks.Add(firstChunk);
	}
	
	void Update() {
		globalTimer++;
		CalculateValidChunkPosns();
		
		GenerateMissingChunks();
	}
	
	void CalculateValidChunkPosns() {
		TerrainChunk playerChunk = null;
		foreach (TerrainChunk t in spawnedChunks) {
			if (t.containsPlayer) {
				playerChunk = t;
			}
		}
		
		if (playerChunk == null) {
			return;
		}
		
		Vector3 pChunkPos = playerChunk.transform.position;
		
		validChunkPosns.Clear();
		
		// add center
		AddToValidList(pChunkPos);
		
		// generate top-left
		AddToValidList(new Vector3(
			pChunkPos.x - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
		
		// generate top
		AddToValidList(new Vector3(
			pChunkPos.x,
			pChunkPos.y + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
		
		// generate top-right
		AddToValidList(new Vector3(
			pChunkPos.x + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
		
		// generate left
		AddToValidList(new Vector3(
			pChunkPos.x - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y,
			0f)
		);
		
		// generate right
		AddToValidList(new Vector3(
			pChunkPos.x + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y,
			0f)
		);
		
		// generate bottom-left
		AddToValidList(new Vector3(
			pChunkPos.x - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
		
		// generate bottom
		AddToValidList(new Vector3(
			pChunkPos.x,
			pChunkPos.y - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
		
		// generate bottom-right
		AddToValidList(new Vector3(
			pChunkPos.x + (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			pChunkPos.y - (TerrainChunk.mapWidth * TerrainChunk.tileSize),
			0f)
		);
	}
	
	void AddToValidList(Vector3 pos) {
		if (!validChunkPosns.Contains(pos)) {
			validChunkPosns.Add(pos);
		}
	}
	
	void GenerateMissingChunks() {
		// positions of existing chunks
		List<Vector3> posns = new List<Vector3>();
		
		// populate existing chunk positions
		foreach (TerrainChunk t in spawnedChunks) {
			posns.Add(t.transform.position);
		}
		
		List<Vector3> toSpawn = new List<Vector3>();
		
		// generate chunks that are missing
		foreach (Vector3 pos in validChunkPosns) {
			if (!posns.Contains(pos)) {
				toSpawn.Add(pos);
			}
		}
		
		foreach (Vector3 s in toSpawn) {
			GameObject spawnedChunk = Instantiate(chunkPrefab, s, Quaternion.identity) as GameObject;
			TerrainChunk firstChunk = spawnedChunk.GetComponent<TerrainChunk>();
			spawnedChunks.Add(firstChunk);
		}
	}
}
