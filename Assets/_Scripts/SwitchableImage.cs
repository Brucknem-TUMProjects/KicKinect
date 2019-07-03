using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SwitchableImage : MonoBehaviour
{
    private Vector2 pivot;
    private Image image;

    private static readonly Color visible = new Color(1, 1, 1, 1);
    private static readonly Color hidden = new Color(1, 1, 1, 0);

    private float duration;
    private float maxZoomToFadeRatio;
    private float maxZoomDepth;

    private float zoomDepth;

    private float fadeInEnd;
    private float fadeOutBegin;

    private float l = 1;

    private void Start()
    {
        image = GetComponent<Image>();
        image.rectTransform.anchorMin = Vector2.zero;
        image.rectTransform.anchorMax = Vector2.one;
    }

    public void Init(float duration, float maxZoomToFadeRatio, float maxZoomDepth)
    {
        this.duration = duration;
        this.maxZoomToFadeRatio = maxZoomToFadeRatio;
        this.maxZoomDepth = maxZoomDepth;
    }

    public void Begin(Sprite sprite)
    {
        if(image == null)
        {
            image = GetComponent<Image>();
        }

        l = 0;
        image.color = hidden;
        fadeInEnd = Random.Range(0.5f * (1 - maxZoomToFadeRatio), (1 - maxZoomToFadeRatio)) / 2f;
        fadeOutBegin = 1f - fadeInEnd;
        zoomDepth = Random.Range(0.5f * maxZoomDepth, maxZoomDepth);
        image.sprite = sprite;
        image.rectTransform.localScale = Vector2.one;
        image.rectTransform.pivot = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        if(l > 1)
        {
            return;
        }

        if(l < fadeInEnd)
        {
            // Fade in 
            float t1 = Normalize(l, 0, fadeInEnd);
            image.color = Color.Lerp(hidden, visible, t1);
        }
        else if(l > fadeOutBegin)
        {
            //Fade Out
            float t = Normalize(l, fadeOutBegin, 1);
            image.color = Color.Lerp(visible, hidden, t);
        }

        image.rectTransform.localScale = Vector3.Slerp(Vector2.one, Vector2.one * (1 + zoomDepth), Normalize(l, 0, 1));
     

        l += Time.deltaTime / duration;
    }

    private float Normalize(float l, float min, float max)
    {
        return (l - min) / (max - min) * 1.1f + 0.05f;
    }
}
