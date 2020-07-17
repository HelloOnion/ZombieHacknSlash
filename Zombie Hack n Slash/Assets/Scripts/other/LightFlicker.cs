using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light;
    [Tooltip("Turn　Lights On/Off")]
    [Header("On/Off")]
    public bool lightOn = true;
    [Tooltip("Turn Flickering Lights On/Off")]
    public bool flicker = true;
    [Header("Intensity")]
    public float minIntensity = 0;
    public float maxIntensity = 100f;
    [Tooltip("Smooth out Randomness: low value = sparks; high value = lantern")]
    [Range(1,50)]
    public int smoothing = 20;

    private Queue<float> smoothQueue;
    private float lastSum = 0;

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);

        if (light == null){ light = GetComponent<Light>(); }

        if(lightOn == false){ light.gameObject.SetActive(false); }
    }

    void Update()
    {
        if(light == null) return;

        if(lightOn && flicker)
        {
            while(smoothQueue.Count >= smoothing)
            {
                lastSum -= smoothQueue.Dequeue();
            }
            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothQueue.Enqueue(newVal);
            lastSum += newVal;

            light.intensity = lastSum / (float)smoothQueue.Count;
        }
        
    }
}
