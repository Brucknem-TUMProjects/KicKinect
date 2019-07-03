using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageSwitcher : MonoBehaviour
{
    [Range(1, 30)]
    public float duration = 10;
    [Range(0, 1)]
    public float crossfade = 0.2f;
    [Range(0, 1)]
    public float maxZoomToFadeRatio = 0.75f;
    [Range(0, 1)]
    public float maxZoomDepth = 0.5f;
    
    public Sprite[] images;
    public GameObject framesPrefab;

    private SwitchableImage[] frames;

    private float lastUpdate;
    private int imageIndex = 0;
    private int currentForeground = 0;

    private void Start()
    {
        frames = new SwitchableImage[2];
        frames[0] = Instantiate(framesPrefab, transform).GetComponent<SwitchableImage>();
        frames[1] = Instantiate(framesPrefab, transform).GetComponent<SwitchableImage>();

        frames[0].Init(duration, maxZoomToFadeRatio, maxZoomDepth);
        frames[1].Init(duration, maxZoomToFadeRatio, maxZoomDepth);

        frames[0].Begin(images[0]);
        lastUpdate = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;
        if(currentTime - lastUpdate > duration * (1f - crossfade))
        {
            lastUpdate = currentTime;

            imageIndex++;
            imageIndex %= images.Length;

            currentForeground++;
            currentForeground %= frames.Length;

            frames[currentForeground].Begin(images[imageIndex]);
        }
    }
}
