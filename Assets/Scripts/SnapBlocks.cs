using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapBlocks : MonoBehaviour
{
    void Update()
    {
        Vector3 snap;
        snap.x = Mathf.RoundToInt(transform.position.x - .5f);
        snap.z = Mathf.RoundToInt(transform.position.z - .5f);

        transform.position = new Vector3(snap.x + .5f, -0.5f, snap.z + 0.5f);
    }
}
