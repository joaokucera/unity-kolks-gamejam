using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public struct Highscore
{
	public string username;
	public int score;
	public DateTime date;

	public Highscore (string _username, int _score, DateTime _date)
	{
		username = _username;
		score = _score;
		date = _date;
	}
}

public class Ranking : MonoBehaviour 
{
	private static Ranking instance;
	public DisplayHighscore displayHighscore;

	private const string WebPage = "http://dreamlo.com/lb/wOjycPiLHkOveR7oW-3TGgrrSrOg7zF0qYx9tdOMV6QQ";
	private const string PrivateCode = "wOjycPiLHkOveR7oW-3TGgrrSrOg7zF0qYx9tdOMV6QQ";
	private const string PublicCode = "553d56ab6e51b61f1c89a825";
	private const string WebUrl = "http://dreamlo.com/lb/";
	private Highscore[] highscores;

	void Awake()
	{
		instance = this;
	}

	public static void AddHighscore(string username, int score)
	{
		instance.StartCoroutine(instance.UploadHighscore(username, score));
	}

	IEnumerator UploadHighscore(string username, int score)
	{
		WWW www = new WWW (WebUrl + PrivateCode + "/add/" + WWW.EscapeURL (username) + "/" + score);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) 
		{
			print("Success upload");
			DownloadHighscore();
		}
		else
		{
			print("Error uploading: " + www.error);
		}
	}

	public void DownloadHighscore()
	{
		StartCoroutine(DownloadHighscoreFromDataBase());
	}

	IEnumerator DownloadHighscoreFromDataBase()
	{
		WWW www = new WWW (WebUrl + PublicCode + "/pipe/");
		yield return www;
		
		if (string.IsNullOrEmpty(www.error))
		{
			print("Success download: " + www.text);
			if (displayHighscore != null)
			{
				FormatHighscore(www.text);
				displayHighscore.OnHighscoresDownloaded(highscores);
			}
		}
		else
		{
			print("Error download: " + www.error);
		}
	}

	void FormatHighscore(string textStream)
	{
		string[] entries = textStream.Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		highscores = new Highscore[entries.Length];

		for (int i = 0; i < entries.Length; i++)
		{
			string[] entryInfo = entries[i].Split (new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);

			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			DateTime date = DateTime.Parse(entryInfo[3]);
			
			highscores[i] = new Highscore(username, score, date);
		}
	}
}