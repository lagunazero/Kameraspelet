using UnityEngine;
using System.Collections;
using System;

public class Fade : MonoBehaviour
{
    public Texture fadeTexture;
    public Color fadeColor;

    private float alpha = 0;
    private WaitForEndOfFrame wait = new WaitForEndOfFrame();

    public IEnumerator FadeOut(Func<IEnumerator> onComplete, float fadeTime)
    {
        float timer = 0;
        while (alpha != 1)
        {
            timer += Time.deltaTime;
            alpha = Mathf.Min(1, timer / fadeTime);
            DrawFade();
            yield return wait;
        }
        if (onComplete != null)
            StartCoroutine(onComplete());
    }

    public IEnumerator FadeIn(Func<IEnumerator> onComplete, float fadeTime)
    {
        float timer = fadeTime;
        while (alpha != 0)
        {
            timer -= Time.deltaTime;
            alpha = Mathf.Max(0, timer / fadeTime);
            DrawFade();
            yield return wait;
        }
        if (onComplete != null)
            StartCoroutine(onComplete());
    }

    private void DrawFade()
    {
        GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture, ScaleMode.StretchToFill, true);
    }
}
