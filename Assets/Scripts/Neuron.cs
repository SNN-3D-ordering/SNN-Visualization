using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Neuron
{

    public int id;
    public List<int> position;
    public bool active;
    public List<int> connectedTo;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
