using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NetworkLoader : MonoBehaviour
{

    public string jsonFile; // Drag your JSON file here in the Unity Inspector
    public GameObject neuronPrefab; // Drag your prefab here in the Unity Inspector

    // Start is called before the first frame update
    void Start()
    {
        ParseJson(jsonFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ParseJson(string jsonFile)
    {
        // Deserialize the JSON into the NeuralNetwork structure
        string jsonString = File.ReadAllText(jsonFile);
        Debug.Log(jsonString);
        Network neuralNetwork = JsonUtility.FromJson<Network>(jsonString);

        // Iterate through each layer and neuron to spawn prefabs at their positions
        foreach (var layer in neuralNetwork.layers)
        {
            foreach (var neuron in layer.neurons)
            {
                Vector3 position = new Vector3(neuron.position[0], neuron.position[1], 0); // Z-axis set to 0 for 2D
                GameObject neuronInstance = Instantiate(neuronPrefab, position, Quaternion.identity);
                
                // Optional: Set the neuron ID or active status on the instantiated object, if needed
                neuronInstance.name = $"Neuron_{neuron.id}";
            }
        }
    }
}
