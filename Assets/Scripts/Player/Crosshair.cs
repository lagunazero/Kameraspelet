using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crosshair : MonoBehaviour
{
    //public enum Appearance
    //{
    //    grabbable,
    //    grabbing,
    //    interactable,
    //    interacting
    //}
    //public Appearance appearance;
    //public List<CrosshairGfx> crosshairGfx;
	
	public Aim aim;
	public FPSWalker walker;

    public Texture aimTexture;
    public float aimAlpha = 1;
    public float aimScale = 0.05f;
    public float darkAlpha = 0.4f;
	public float touchScale = 0.4f;
	public float touchAlpha = 0.4f;
    public Texture grabbableTexture;
    public float grabbableAlpha = 0.7f;
    public float grabbableScale = 0.4f;
    public Texture grabbingTexture;
    public float grabAlpha = 0.8f;
    public float grabScale = 0.4f;
    public Texture interactTexture;
    public float interactAlpha = 0.8f;
    public float interactScale = 0.4f;
    public float fadeTime = 3;
    public bool leftie = false;
	
	public float hideThreshold = 3;
	public float hideTime = 1;
	private float hideTimer;
	
    private float scale;
    private float wantedScale;
    private float alpha;
    private float wantedAlpha;

    [HideInInspector]
    public Texture tex;

    private Rect position;
    private float halfTextureWidth;
    private float halfTextureHeight;
	
	private bool stillForLong;
	
    void Start()
    {
        position = new Rect(0, 0, 0, 0);
        halfTextureWidth = aimTexture.width * 0.5f;
        halfTextureHeight = aimTexture.height * 0.5f;
        tex = aimTexture;
		if(aim == null) Debug.LogWarning("No Aim component linked to Crosshair.");
		if(walker == null) Debug.LogWarning("No FPSWalker component linked to Crosshair.");
    }

    void OnGUI()
    {
        if (aim.hasControl)
        {
            Vector3 pos = InputHandler.MousePosition;

            if (!leftie)
            {
                position.x = pos.x + (halfTextureWidth * scale);
                position.width = -aimTexture.width * scale;
            }
            else
            {
                position.x = pos.x - (halfTextureWidth * scale);
                position.width = aimTexture.width * scale;
            }
            position.y = Screen.height - pos.y - (halfTextureHeight * scale);
            position.height = aimTexture.height * scale;

			GUI.color = new Color(255, 255, 255, alpha);
            GUI.DrawTexture(position, tex);
        }
    }

    void Update()
    {
        if (alpha != wantedAlpha)
            alpha = Mathf.Lerp(alpha, wantedAlpha, Time.deltaTime * fadeTime);
        if (scale != wantedScale)
            scale = Mathf.Lerp(scale, wantedScale, Time.deltaTime * fadeTime);
	
		if(InputHandler.DeltaMousePosition.magnitude<hideThreshold)
		{
			hideTimer+=Time.deltaTime;
			if(hideTimer>hideTime)
			{
				stillForLong=true;
			}
		}
		else
		{
			hideTimer=0;
			stillForLong=false;
		}
	}
	

    public void Aiming()
    {
        tex = aimTexture;
        wantedScale = aimScale;
        if (walker.IsMoving || stillForLong) wantedAlpha = aimAlpha;
        else wantedAlpha = darkAlpha;
		leftie=InputHandler.MousePosition.x<Screen.width/2;
    }
	
	public void Touching()
	{
		tex = aimTexture;
		wantedScale = touchScale;
		wantedAlpha = touchAlpha;
	}
	
    public void Grabbable() { tex = grabbableTexture; wantedAlpha = grabbableAlpha; wantedScale = grabbableScale; }
    public void Grabbing() { tex = grabbingTexture; wantedAlpha = grabAlpha; wantedScale = grabScale; }
    public void Interactable() { tex = interactTexture; wantedAlpha = interactAlpha; wantedScale = interactScale; }
}