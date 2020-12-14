using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamable : MonoBehaviour
{

    private int fireStatus; // 0: living - 1: burning - 2: smoldering

    // probabilty of setting a light
    private float fireProbability;
    public float minSpreadProbability;
    public float maxSpreadProbability;

    // material for stages of fire
    public Material burntMaterial;
    public GameObject smokeObject;

    private void OnMouseDown()
    {
        // save the trees!
        if(fireStatus < 1) Destroy(gameObject);
    }

    public int getFireStatus()
    {
        return fireStatus;
    }

    public void setOnFire()
    {
        if (fireStatus > 0) return;
        fireStatus = 1;

        // change tree Material
        GetComponent<Renderer>().material = burntMaterial;

        // add smoke particles that disapear after time
        smokeObject = GameObject.Instantiate(smokeObject, transform.position, Quaternion.identity);
        Object.Destroy(smokeObject, 5.0f);
    }

    public bool updateFireProbability()
    {
        // update the probability of setting on fire
        if (fireStatus > 0) return false;

        fireProbability += Random.Range(minSpreadProbability, maxSpreadProbability);
        if(fireProbability > 1)
        {
            setOnFire();
            return true;
        }
        return false;
    }

}
