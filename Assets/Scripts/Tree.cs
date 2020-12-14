using UnityEngine;

public class Tree
{
    private int positionX;
    private int positionY;
    private float fireProbability;

    private bool isOnFire;

    private GameObject treeObject;
    private Material burntMaterial;
    private GameObject smokeObject;

    public Tree(GameObject treeObject, Material burntMaterial, GameObject smokeObject, int x, int y)
    {
        this.treeObject = treeObject;
        this.burntMaterial = burntMaterial;
        this.smokeObject = smokeObject;
        this.positionX = x;
        this.positionY = y;
        this.fireProbability = 0;
    }

    public float getHeight()
    {
        return treeObject.transform.position.y;
    }

    public void setOnFire()
    {
        if (isOnFire) return;
        isOnFire = true;

        if (burntMaterial == null) Debug.LogError("Tree Material Not Found!");
        else treeObject.GetComponent<Renderer>().material = burntMaterial;

        smokeObject = GameObject.Instantiate(smokeObject, treeObject.transform.position, Quaternion.identity);
        Object.Destroy(smokeObject, 3.0f);
    }

    public bool getIsOnFire()
    {
        return isOnFire;
    }

    public void updateFireProbability()
    {
        fireProbability += Random.Range(0.0f, 0.25f);
        if (fireProbability >= 1) setOnFire();
    }

    public GameObject returnTree()
    {
        return treeObject;
    }
}