using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NetworkLoader : MonoBehaviour
{

    public string jsonFile; // Drag your JSON file here in the Unity Inspector
    public GameObject neuronPrefab; // Drag your prefab here in the Unity Inspector

    public float maxDepth = 20; // Define the maximum allowable depth for the model
    public float layerSpacing = 4f; // Base distance between layers

    // Start is called before the first frame update
    void Start()
    {
        ParseJson(jsonFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Position each layer along the Z-axis with scaling if necessary
    private void PositionLayers(List<GameObject> layers)
    {
        // Calculate the total depth of the ANN
        float annDepth = (layers.Count - 1) * layerSpacing;

        // Get the scaling factor based on annDepth and maxDepth
        float scaleFactor = CalculateScaleFactor(annDepth, maxDepth);

        // Calculate the starting Z position to center the layers
        float startingZPosition = -0.5f * annDepth * scaleFactor;

        // Position each layer along the Z-axis and apply scaling
        for (int i = 0; i < layers.Count; i++)
        {
            // Calculate Z position based on layer index, spacing, and scale factor
            float zPosition = startingZPosition + i * layerSpacing * scaleFactor;

            // Set the position of the layer
            layers[i].transform.position = new Vector3(0, 0, zPosition);

            // Scale the layer according to the scale factor
            // layers[i].transform.localScale = Vector3.one * scaleFactor;
        }
    }
    
    private float CalculateScaleFactor(float annDepth, float maxDepth)
    {
        return annDepth > maxDepth ? maxDepth / annDepth : 1f;
    }

    // Parses the JSON and instantiates neuron prefabs at initial positions
    private void ParseJson(string jsonFile)
    {
        // Deserialize the JSON into the NeuralNetwork structure
        string jsonString = File.ReadAllText(jsonFile);
        Debug.Log(jsonString);
        Network neuralNetwork = JsonUtility.FromJson<Network>(jsonString);

        // Create a list to hold layers of neuron GameObjects
        List<GameObject> layerGameObjects = new List<GameObject>();

        // Iterate through each layer and spawn neuron prefabs
        for (int i = 0; i < neuralNetwork.layers.Count; i++)
        {
            Layer layer = neuralNetwork.layers[i];

            // Create an empty GameObject to hold neurons for this layer
            GameObject layerObject = new GameObject($"Layer_{i}");
            layerObject.transform.parent = transform; // Parent it to the main object

            foreach (var neuron in layer.neurons)
            {
                // Determine initial position (Y-axis for neuron spacing, Z=0 initially)
                Vector3 position = new Vector3(neuron.position[0], neuron.position[1], 0);
                
                // Instantiate the neuron prefab and parent it to the layer object
                GameObject neuronInstance = Instantiate(neuronPrefab, position, Quaternion.identity, layerObject.transform);
                neuronInstance.name = $"Neuron_{neuron.id}";
            }

            // Add this layer to the list for positioning later
            layerGameObjects.Add(layerObject);
        }

        // Position layers in 3D space after all neurons have been instantiated
        PositionLayers(layerGameObjects);
    }

}