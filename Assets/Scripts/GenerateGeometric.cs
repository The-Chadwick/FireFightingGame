using UnityEngine;
using System.Collections.Generic;

public class GenerateGeometric : MonoBehaviour
{

	public float noiseSeed = 0;
	public float maxHeight = 20;
	public float mapWidth = 10;
	public float mapDepth = 10;

	public Material groundMaterial;

	void Start()
	{
		noiseSeed = Random.Range(0, 1000);

		for(int x = 0; x < mapWidth; x++)
        {
			for(int y = 0; y < mapDepth; y++)
            {
				float height = Mathf.PerlinNoise((x / mapWidth) + noiseSeed, (y / mapDepth) + noiseSeed) * maxHeight;
				CreateCube(height, height, height, height, new Vector3(x, 0, y));
			}
        }
	}

	private void CreateCube(float z1, float z2, float z3, float z4, Vector3 position)
	{
		Vector3[] vertices = {
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (1, z1, 0),
			new Vector3 (0, z2, 0),
			new Vector3 (0, z3, 1),
			new Vector3 (1, z4, 1),
			new Vector3 (1, 0, 1),
			new Vector3 (0, 0, 1),
		};

		int[] triangles = {
			0, 2, 1, //face front
			0, 3, 2,
			2, 3, 4, //face top
			2, 4, 5,
			1, 2, 5, //face right
			1, 5, 6,
			0, 7, 4, //face left
			0, 4, 3,
			5, 4, 7, //face back
			5, 7, 6,
			0, 6, 7, //face bottom
			0, 1, 6
		};

		GameObject cube = new GameObject("Floor");
				
		cube.AddComponent<MeshRenderer>();
		MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		renderer.material = groundMaterial;

		cube.AddComponent<MeshFilter>();
		Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.Optimize();
		mesh.RecalculateNormals();

		cube.transform.position = position;
	}
}