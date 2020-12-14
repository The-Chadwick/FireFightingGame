// Brandon Walker

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateForestFloor : MonoBehaviour
{

    public int floorWidth;
    public int floorHeight;
    public float maxHeight;
    public float minHeight;
    public Material material_forestFloor;

    public float noiseSeed;
    [Range(0.0f, 20.0f)]
    public float amplitude = 1; // intensity of noise wave
    [Range(0.0f, 1.0f)]
    public float frequency; // number of noise cycles in a wave
    public float persistence; // wave interferance
    public float lacunarity; // NOT WORKING
    [Range(0.0f, 5.0f)]
    public float scale; // amount of noise sampled

    public float octaves;

    private Vector3[] verticies;
    private int[] triangles;
    private int vertexIndex = 0;
    private int triangleIndex = 0;

    public GameObject tree;
    [Range(0.0f, 1.0f)]
    public float treeDensity;
    private GameObject[,] flamables;

    public GameObject house;
    public GameObject fireHouse;

    private int treesBurnt;
    private List<GameObject> buildings;
    public GameObject scoreText;

    // Start is called before the first frame update
    void Start()
    {

        verticies = new Vector3[floorWidth * floorHeight];
        triangles = new int[(floorWidth - 1) * (floorHeight - 1) * 6];

        if (noiseSeed == 0) noiseSeed = Random.Range(0.0f, 1000.0f);

        treeDensity = 0.5f;
        flamables = new GameObject[floorWidth, floorHeight];
        treesBurnt = 0;
        buildings = new List<GameObject>();

        drawMap();

        // start simulation
        GameObject fireStart = null;
        while (fireStart == null)
        {
            fireStart = flamables[Random.Range(0, floorWidth), Random.Range(0, floorHeight)];
        }
        fireStart.GetComponent<Flamable>().setOnFire();
        InvokeRepeating("spreadFire", 3.0f, 0.25f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            // reset scene
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    private void drawMap()
    {

        float treeAverage = 0;
        float[,] treeArrayProbability = new float[floorWidth, floorHeight];
        float[,] heightMap = new float[floorWidth, floorHeight];

        for (int x = 0; x < floorWidth; x++)
        {
            for (int y = 0; y < floorHeight; y++)
            {
                float height = 0;
                float changingAplitude = amplitude;

                for (int i = 0; i < octaves; i++)
                {
                    // perlin noise terrain
                    float xCoordinate = ((x / scale) * frequency) + noiseSeed;
                    float yCoordinate = ((y / scale) * frequency) + noiseSeed;
                    height += ((Mathf.PerlinNoise(xCoordinate, yCoordinate) * 2) - 1) * changingAplitude;

                    changingAplitude *= persistence;
                    frequency *= lacunarity; // TODO: fix lacunarity
                }

                // add height to mesh
                verticies[vertexIndex] = new Vector3(x, height, y);
                if ((x < (floorWidth - 1)) && (y < (floorHeight - 1)))
                {
                    addTriangle(vertexIndex, vertexIndex + floorWidth + 1, vertexIndex + floorWidth);
                    addTriangle(vertexIndex + floorWidth + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;

                // add random trees (simple noise)
                float treeProbability = Mathf.PerlinNoise(((x / scale) * frequency) + noiseSeed, ((y / scale) * frequency) + noiseSeed);
                heightMap[x, y] = height;
                treeArrayProbability[x, y] = treeProbability;
                treeAverage += treeProbability;
            }
        }

        // spawn tree if average is above density
        treeAverage /= (floorWidth * floorHeight);
        for (int x = 0; x < floorWidth; x++)
        {
            for (int y = 0; y < floorHeight; y++)
            {
                if (treeArrayProbability[x, y] > treeAverage)
                {
                    // place tree
                    /*treeArray[x, y] = new Tree(GameObject.Instantiate(tree, new Vector3(x, heightMap[x, y], y), Quaternion.identity), burntMaterial, smokeObject, x , y);*/
                    flamables[x, y] = Instantiate(tree, tree.transform.position + new Vector3(x, heightMap[x, y], y), Quaternion.identity);
                }
            }
        }

        // spawn village (center must be 5 units from edge)
        int villageCenterX = Random.Range(15, (floorWidth - 15));
        int villageCenterY = Random.Range(15, 25);
        int houseCount = Random.Range(1, 4);

        float angle = Mathf.PI / (houseCount + 1);
        float randomRadius = Random.Range(2, 4);

        // instantiate firehouse
        int buildingX = villageCenterX + (int)(randomRadius * Mathf.Cos(0));
        int buildingY = villageCenterY + (int)(randomRadius * Mathf.Sin(0));
        Vector3 buildingSpawn = new Vector3(buildingX, heightMap[buildingX, buildingY], buildingY);
        Collider[] treesInRange = Physics.OverlapSphere(buildingSpawn, 6);
            
        foreach (Collider tree in treesInRange) Destroy(tree.gameObject);

        buildings.Add(Instantiate(fireHouse, buildingSpawn, Quaternion.identity));

        for (int i = 1; i < houseCount + 1; i++)
        {
            // instantiate houses
            angle *= i;
            randomRadius = Random.Range(5, 10);

            buildingX = villageCenterX + (int)(randomRadius * Mathf.Cos(angle));
            buildingY = villageCenterY + (int)(randomRadius * Mathf.Sin(angle));
            buildingSpawn = new Vector3(buildingX, heightMap[buildingX, buildingY], buildingY);

            treesInRange = Physics.OverlapSphere(buildingSpawn, 4);
            foreach (Collider tree in treesInRange) Destroy(tree.gameObject);

            buildings.Add(Instantiate(house, buildingSpawn, Quaternion.identity));
        }

        
        


        // draw forest floor
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<Renderer>().material = material_forestFloor;
    }

    private void addTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    private void spreadFire()
    {
        bool fireSpreading = false;
        for (int x = 0; x < floorWidth; x++)
        {
            for (int y = 0; y < floorHeight; y++)
            {
                // change the fire probability of neighbors (cellular automata)
                if (flamables[x, y])
                {
                    if(flamables[x, y].GetComponent<Flamable>().getFireStatus() > 0)
                    {
                        for(int i = -1; i < 2; i++)
                        {
                            for(int j = -1; j < 2; j++)
                            {
                                int neighborX = x + i;
                                int neighborY = y + j;
                                if (neighborX < 0 || neighborX > (floorWidth - 1) || neighborY < 0 || neighborY > (floorHeight - 1)) continue;
                                if(flamables[neighborX, neighborY])
                                {
                                    if(flamables[neighborX, neighborY].GetComponent<Flamable>().updateFireProbability())
                                    {
                                        treesBurnt++;
                                        fireSpreading = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        scoreText.GetComponent<Text>().text = "Trees burnt: " + treesBurnt;
        foreach(GameObject building in buildings)
        {
            Collider[] trees = Physics.OverlapSphere(building.transform.position, 10);
            foreach(Collider tree in trees)
            {
                if (tree.GetComponent<Flamable>())
                {
                    if (tree.gameObject.GetComponent<Flamable>().getFireStatus() > 0)
                    {
                        scoreText.GetComponent<Text>().text += "\nThe Town Burnt Down\nPress 'R' to retry";
                        CancelInvoke("spreadFire");
                    }
                }
            }
        }

        if(!fireSpreading)
        {
            scoreText.GetComponent<Text>().text += "\n you saved the town!\nPress 'R' to retry";
        }
    }
}