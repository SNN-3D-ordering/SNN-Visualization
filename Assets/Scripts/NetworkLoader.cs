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
    public float layerWidth = 10f; // Adjustable width for neuron spread in each layer
    public float layerHeight = 10f; // Adjustable height for neuron spread in each layer

    // Start is called before the first frame update
    void Start()
    {
        ParseJson(jsonFile);
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

            // Set the position of the layer along the Z-axis
            layers[i].transform.localPosition = new Vector3(0, 0, zPosition);
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

            // Create a list to store neuron positions for centering
            List<Vector3> neuronPositions = new List<Vector3>();

            foreach (var neuron in layer.neurons)
            {
                // Adjust position based on layerWidth and layerHeight
                float xPosition = neuron.position[0] * layerWidth;
                float yPosition = neuron.position[1] * layerHeight;
                Vector3 position = new Vector3(xPosition, yPosition, 0);
                neuronPositions.Add(position);

                // Instantiate the neuron prefab and parent it to the layer object
                GameObject neuronInstance = Instantiate(neuronPrefab, position, Quaternion.identity, layerObject.transform);
                neuronInstance.name = $"Neuron_{neuron.id}";

                // Set the color of the neuron based on the heat value
                float heat = Mathf.Clamp(neuron.heat, -1, 3000); // Clamp heat between -1 and 3000
                float alpha = CalculateAlpha(heat);

                Debug.Log("Heat: " + heat + " Alpha: " + alpha);

                Color color = GetJetColor(heat, alpha);
                SetNeuronColor(neuronInstance.transform.GetChild(0).gameObject, color);
            }

            // Center neurons around the layer's local center
            CenterLayer(layerObject, neuronPositions);

            // Add this layer to the list for positioning later
            layerGameObjects.Add(layerObject);
        }

        // Position layers in 3D space after all neurons have been instantiated
        PositionLayers(layerGameObjects);
    }

    // Centers the neurons within a layer by adjusting their positions
    private void CenterLayer(GameObject layerObject, List<Vector3> neuronPositions)
    {
        // Calculate the center point of all neuron positions
        Vector3 center = Vector3.zero;
        foreach (Vector3 pos in neuronPositions)
        {
            center += pos;
        }
        center /= neuronPositions.Count;

        // Offset each neuron to center around (0,0,0) in the layer's local space
        foreach (Transform neuron in layerObject.transform)
        {
            neuron.localPosition -= center;
        }
    }

    // Maps heat value to a color on the jet color scale
   private Color GetJetColor(float heat, float alpha)
    {
        float normalizedHeat = Mathf.InverseLerp(-1, 3000, heat); // Map heat to range [0, 1]

        // Define jet color scale with interpolation points
        if (normalizedHeat <= 0.25f) // Blue to Cyan
            return Color.Lerp(new Color(Color.blue.r, Color.blue.g, Color.blue.b, alpha), 
                            new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, alpha), 
                            normalizedHeat / 0.25f);
        else if (normalizedHeat <= 0.5f) // Cyan to Green
            return Color.Lerp(new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, alpha), 
                            new Color(Color.green.r, Color.green.g, Color.green.b, alpha), 
                            (normalizedHeat - 0.25f) / 0.25f);
        else if (normalizedHeat <= 0.75f) // Green to Yellow
            return Color.Lerp(new Color(Color.green.r, Color.green.g, Color.green.b, alpha), 
                            new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alpha), 
                            (normalizedHeat - 0.5f) / 0.25f);
        else // Yellow to Red
            return Color.Lerp(new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alpha), 
                            new Color(Color.red.r, Color.red.g, Color.red.b, alpha), 
                            (normalizedHeat - 0.75f) / 0.25f);
    }


    // Sets the color of the neuron material
    private void SetNeuronColor(GameObject neuron, Color color)
    {
        Renderer renderer = neuron.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color; // Set the color based on the heat-mapped jet color
        }
    }

    // Calculate alpha based on heat value, mapping -1 to 3000 range to 255 to 0 alpha
    private float CalculateAlpha(float heat)
    {
        // Map heat (-1 to 3000) to a normalized range (0 to 1)
        float normalizedAlpha = Mathf.InverseLerp(-1, 3000, heat);
        
        // Invert and scale normalized alpha to range between 255 (opaque) and 0 (transparent)
        return Mathf.Lerp(255f, 0f, normalizedAlpha);
    }
    // Sets the alpha value of the neuron material
    private void SetNeuronAlpha(GameObject neuron, float alpha)
    {
        Renderer renderer = neuron.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Assuming the neuron material supports transparency
            Color color = renderer.material.color;
            color.a = alpha; // Set the alpha based on the heat-mapped alpha value
            renderer.material.color = color;
        }
    }
}