using UnityEngine;
using System.Collections;

public class PlayerLog : MonoBehaviour 
{
	private static PlayerLog instance;

	[HideInInspector]
	public string playerName;
	[HideInInspector]
	public int playerPoints;

	public static PlayerLog Instance 
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<PlayerLog>();
			}

			return instance;
		}
	}
	public static string Name 
	{
		get
		{
			return Instance.playerName;
		}
	}
	public static int Points 
	{
		get
		{
			return Instance.playerPoints;
		}
	}

	void Start()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static void CreatePlayerLog(string playerName)
	{
		Instance.playerName = playerName;
		Instance.playerPoints = 0;
	}

	public static void UpdatePlayerLog(int points)
	{
		Instance.playerPoints = points;

		Ranking.AddHighscore (Instance.playerName, Instance.playerPoints);
	}
}