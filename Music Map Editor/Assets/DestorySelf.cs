using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestorySelf : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform self;

    private void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (self.position.y < -30f)
        {
            Destroy(gameObject);
        }
    }
}