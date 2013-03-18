using UnityEngine;
using System.Collections;

public class TextPrompt : MonoBehaviour
{
    public AnimationCurve positionX, positionY;
    public AnimationCurve alpha;
    public Color color;
    public float introDuration = 0.5f;
    public float loopDuration = 1;
    public float outroDuration = 0.25f;
    public bool flicker;
    public string[] texts;

    private bool display;

    public void Awake()
    {
    }

    public bool SetText(int index)
    {
        if (texts.Length <= index)
            return false;

        guiText.text = texts[index];
        guiText.enabled = true;
        guiText.material.color = color;
        display = true;
        StartCoroutine(UpdateText());
        return true;
    }

    public void Disable()
    {
        display = false;
    }

    private IEnumerator UpdateText()
    {
        Vector3 originalPos = transform.position;
        float firstFrameAlpha = alpha[0].value;
        float timer = 0;
        while (timer <= introDuration)
        {
            guiText.material.color = new Color(color.r, color.g, color.b,
                ((timer += Time.deltaTime) / introDuration) * firstFrameAlpha);
            if (flicker)
                transform.position = new Vector3(originalPos.x + positionX.Evaluate(timer),
                    originalPos.y + positionY.Evaluate(timer), 0);
            yield return 0;
        }

        timer = 0;
        while (display)
        {
            guiText.material.color = new Color(color.r, color.g, color.b,
                alpha.Evaluate(timer += Time.deltaTime / loopDuration));
            if (flicker)
                transform.position = new Vector3(originalPos.x + positionX.Evaluate(timer),
                    originalPos.y + positionY.Evaluate(timer), 0);
            yield return 0;
        }

        float lastFrameAlpha = guiText.material.color.a;
        timer = 0;
        while (timer <= outroDuration)
        {
            guiText.material.color = new Color(color.r, color.g, color.b,
                (1 - (timer += Time.deltaTime) / outroDuration) * lastFrameAlpha);
            if (flicker)
                transform.position = new Vector3(originalPos.x + positionX.Evaluate(timer),
                    originalPos.y + positionY.Evaluate(timer), 0);
            yield return 0;
        }

        guiText.text = "";
        guiText.enabled = false;
    }
}
