using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public static class SFXMaster
{
    public static void Create(AudioClip sound, Vector3 pos, float volume, float pitch)
    {
        GameObject i = new GameObject("SoundEffect");
        i.AddComponent<AudioSource>();
        i.audio.volume = volume;
        i.audio.clip = sound;
        i.audio.pitch = pitch;
        i.AddComponent<SoundPrefab>();
    }
    public static void CreateFromPrefab(SoundPrefab sound, Vector3 pos, SoundPrefab.Materials soundMaterial, float volume = 1)
    {
        if (sound != null)
        {
            SoundPrefab i = (SoundPrefab)Object.Instantiate(sound, pos, Quaternion.identity);
            i.SetSoundMaterial(soundMaterial);
			i.audio.volume = volume;
			i.audio.Play();
        }
    }

    private static Dictionary<string, SoundPrefab.Materials> materialNames
    = new Dictionary<string, SoundPrefab.Materials>()
    {
        {SoundPrefab.Materials.Wood.ToString(), SoundPrefab.Materials.Wood},
        {SoundPrefab.Materials.Metal.ToString(), SoundPrefab.Materials.Metal},
        {SoundPrefab.Materials.Stone.ToString(), SoundPrefab.Materials.Stone},
        {SoundPrefab.Materials.Plastic.ToString(), SoundPrefab.Materials.Plastic},
        {SoundPrefab.Materials.Rubber.ToString(), SoundPrefab.Materials.Rubber},
        {SoundPrefab.Materials.Ceramic.ToString(), SoundPrefab.Materials.Ceramic},
        {SoundPrefab.Materials.Glass.ToString(), SoundPrefab.Materials.Glass},
        {SoundPrefab.Materials.Pad.ToString(), SoundPrefab.Materials.Pad},
        {SoundPrefab.Materials.Cloth.ToString(), SoundPrefab.Materials.Cloth}
    };

    public static SoundPrefab.Materials MaterialPhysicsToSound(PhysicMaterial mat)
    {
        SoundPrefab.Materials result;
        if (mat != null && materialNames.TryGetValue(mat.name, out result))
		{	
			return result;
		}
		else
		{
            return SoundPrefab.Materials.None;
		}
	}

    public static SoundPrefab.Materials MaterialPhysicsToSound(string name)
    {
        SoundPrefab.Materials result;
        if (materialNames.TryGetValue(name, out result))
		{	
    	    return result;
		}
		else
		{	
            return SoundPrefab.Materials.None;
		}
    }
}