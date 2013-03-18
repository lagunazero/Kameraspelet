using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundPrefab : MonoBehaviour
{
    public enum Materials
    {
        None = 0,
        Wood = 1,
        Metal = 2,
        Stone,
        Plastic,
        Rubber,
        Ceramic,
        Glass,
        Pad,
        Cloth,
        Flesh,
        CUSTOM
    }
    [System.NonSerialized]
    public Materials? material;

    public List<AudioClip> mat0;
    public List<AudioClip> mat1;
    public List<AudioClip> mat2;
    public List<AudioClip> mat3;
    public List<AudioClip> mat4;
    public List<AudioClip> mat5;
    public List<AudioClip> mat6;
    public List<AudioClip> mat7;
    public List<AudioClip> mat8;
    public List<AudioClip> mat9;

    public void SetSoundMaterial(Materials t)
    {
        material = t;
		Play();
    }

    void Start()
    {
        //Play();
	}
	void Play(){
		if (material.HasValue && material != SoundPrefab.Materials.None)
        {
            switch (material)
            {
                case Materials.None: break;
                case Materials.Wood: if (mat1.Count != 0) audio.clip = mat1[Random.Range(0, mat1.Count-1)]; break;
                case Materials.Metal: if (mat2.Count != 0) audio.clip = mat2[Random.Range(0, mat2.Count-1)]; break;
                case Materials.Stone: if (mat3.Count != 0) audio.clip = mat3[Random.Range(0, mat3.Count-1)]; break;
                case Materials.Plastic: if (mat4.Count != 0) audio.clip = mat4[Random.Range(0, mat4.Count-1)]; break;
                case Materials.Rubber: if (mat5.Count != 0) audio.clip = mat5[Random.Range(0, mat5.Count-1)]; break;
                case Materials.Ceramic: if (mat6.Count != 0) audio.clip = mat6[Random.Range(0, mat6.Count-1)]; break;
                case Materials.Glass: if (mat7.Count != 0) audio.clip = mat7[Random.Range(0, mat7.Count-1)]; break;
                case Materials.Pad: if (mat8.Count != 0) audio.clip = mat8[Random.Range(0, mat8.Count-1)]; break;
                case Materials.Cloth: if (mat9.Count != 0) audio.clip = mat9[Random.Range(0, mat9.Count-1)]; break;
                case Materials.Flesh: if (mat0.Count != 0) audio.clip = mat0[Random.Range(0, mat0.Count-1)]; break;
                default: break;
            }
        }
        if (audio.clip == null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            audio.Play();
            if (!audio.loop)
            {
                GameObject.Destroy(gameObject, audio.clip.length);
            }
        }
    }
}