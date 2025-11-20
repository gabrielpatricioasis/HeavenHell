using UnityEngine;
using System.Collections.Generic;

public class SmartSpawner : MonoBehaviour
{
    [Header("Prefab Configuration")]
    public GameObject[] prefabsToSpawn;

    [Header("Spawn Settings")]
    public int numberOfObjects = 50;
    public Vector2 xRange = new Vector2(-50, 50);
    public Vector2 zRange = new Vector2(-50, 50);

    [Header("Height Settings")]
    public float minHeight = 5f; 
    public float maxHeight = 20f;

    void Start()
    {
        SpawnObjects();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        if (prefabsToSpawn.Length == 0) return;

        // --- SORT LISTS ---
        List<GameObject> spherePrefabs = new List<GameObject>();
        List<GameObject> otherPrefabs = new List<GameObject>();

        foreach (var p in prefabsToSpawn)
        {
            if (p.name.Contains("Sphere")) spherePrefabs.Add(p);
            else otherPrefabs.Add(p);
        }

        if (spherePrefabs.Count == 0 || otherPrefabs.Count == 0)
        {
            Debug.LogError("Need both Spheres and non-Spheres!");
            return;
        }

        // --- CALCULATE COUNTS ---
        int sphereCount = numberOfObjects / 5; 
        int otherCount = numberOfObjects - sphereCount;

        List<GameObject> spawnQueue = new List<GameObject>();
        for (int i = 0; i < sphereCount; i++) spawnQueue.Add(spherePrefabs[Random.Range(0, spherePrefabs.Count)]);
        for (int i = 0; i < otherCount; i++) spawnQueue.Add(otherPrefabs[Random.Range(0, otherPrefabs.Count)]);
        
        ShuffleList(spawnQueue);

        // --- SPAWN LOOP ---
        foreach (GameObject selectedPrefab in spawnQueue)
        {
            // Position
            float randomX = Random.Range(xRange.x, xRange.y);
            float randomZ = Random.Range(zRange.x, zRange.y);
            float heightY = Random.Range(minHeight, maxHeight);
            if (selectedPrefab.name.Contains("Particle")) heightY = 6f;

            Vector3 spawnPos = new Vector3(randomX, heightY, randomZ);

            // Rotation
            Quaternion spawnRot = Quaternion.identity;
            if (selectedPrefab.name.Contains("Particle")) spawnRot = Quaternion.Euler(90, 0, 0);
            else if (selectedPrefab.name.Contains("Capsule")) spawnRot = Quaternion.Euler(Random.Range(40f, 70f), Random.Range(0f, 360f), 0);
            else spawnRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Quaternion finalRot = selectedPrefab.transform.rotation * spawnRot;
            if (selectedPrefab.name.Contains("Particle")) finalRot = spawnRot;

            GameObject newObj = Instantiate(selectedPrefab, spawnPos, finalRot, transform);
            
            // --- ADD MOVEMENT AND SET LIMITS ---
            GentleFloater floater = newObj.AddComponent<GentleFloater>();
            
            // THIS IS THE NEW PART: Pass the boundaries from Spawner to Floater
            floater.xLimits = xRange;
            floater.zLimits = zRange;
        }
    }

    void ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}