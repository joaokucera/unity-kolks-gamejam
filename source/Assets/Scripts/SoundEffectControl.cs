using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum SoundEffectClip
{
	ButtonClick,
	GameOver,
	StageCompleted
}

[RequireComponent(typeof(AudioSource))]
public class SoundEffectControl : MonoBehaviour 
{
	#region [ FIELDS ]
	
	private static SoundEffectControl instance;
	
	[SerializeField]
	private AudioClip buttonClickClip;
	[SerializeField]
	private AudioClip gameOverClip;
	[SerializeField]
	private AudioClip stageCompletedClip;

	private Dictionary<SoundEffectClip, AudioClip> clipDictionary;
	[SerializeField]
	private AudioSource musicSource;

	#endregion

	#region [ PROPERTIES ]
	
	public static SoundEffectControl Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<SoundEffectControl>();
			}
			
			return instance;
		}
	}

	#endregion

	#region [ METHODS ]
	
	void Start()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);

			StartCoroutine (PlayMusic ());
			CreateDictionary();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	IEnumerator PlayMusic()
	{
		musicSource.volume = 0f;
		musicSource.loop = true;
		musicSource.Play();
		
		while (musicSource.volume < 0.5f)
		{
			musicSource.volume += Time.deltaTime / 10f;
			
			yield return 0;
		}
	}

	public void ContinueMusic()
	{
		musicSource.Play ();
	}
	
	public void PlaySound(SoundEffectClip soundEffectClip)
	{
		if (soundEffectClip == SoundEffectClip.GameOver)
		{
			musicSource.Stop();
		}

		AudioClip originalClip;
		
		if (clipDictionary.TryGetValue(soundEffectClip, out originalClip))
		{
			MakeSound(originalClip);
		}
	}

	private void CreateDictionary()
	{
		clipDictionary = new Dictionary<SoundEffectClip, AudioClip>();

		clipDictionary.Add(SoundEffectClip.ButtonClick, buttonClickClip);
		clipDictionary.Add(SoundEffectClip.GameOver, gameOverClip);
		clipDictionary.Add(SoundEffectClip.StageCompleted, stageCompletedClip);
	}

	private void MakeSound(AudioClip originalClip)
	{
		audio.PlayOneShot (originalClip);
	}

	#endregion
}