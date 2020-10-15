using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SnapFigures : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Vector3 snap;
        snap.x = Mathf.RoundToInt(transform.position.x - .5f);
        snap.z = Mathf.RoundToInt(transform.position.z - .5f);

        transform.position = new Vector3(snap.x + .5f, 0.85f, snap.z + .5f);
    }
}
