using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour 
{
	#region [ FIELDS ]

	private static BoardManager instance;

	private const string ResourceTilesPath = "Tiles/";
	private const string ResourceWatersPath = "Waters/";

	private static TileElement from;

	private static TileElement to;
	private List<TileElement> elements;
	private int[] places;
	private List<int> tileMovement;
	private int centerLake;

	#endregion

	#region [ PROPERTIES ]

	public static BoardManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<BoardManager>();
			}
			
			return instance;
		}
	}

	private List<TileElement> Elements
	{
		get 
		{
			if (elements == null)
			{
				elements = GetComponentsInChildren<TileElement>().ToList();
			}

			return elements;
		}
	}

	#endregion

	#region [ METHODS ]

	public void SetupBoard(int level)
	{
		ClearElements (level > 1);
		Initialize ();
		Randomize ();
	}

	void ClearElements(bool cleanHover)
	{
		Elements.ForEach (e => 
		{
			e.ResetTile();

			if (cleanHover)
			{
				e.ResetImage();
			}
		});
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!EventSystem.current.IsPointerOverGameObject (-1))
			{
				ResetMoveItem ();
			}
		}
	}

	void Initialize()
	{
		List<Sprite> tileParts = Resources.LoadAll<Sprite> (ResourceTilesPath).ToList();

		for (int i = 0; i < Elements.Count; i++) 
		{
			var element = Elements[i];

			int index = Random.Range(0, tileParts.Count);

			element.Image.sprite = tileParts[index];
			element.tileSet.type = TileType.Ground;
			element.tileSet.position = i;
		}
	}

	public bool CheckLevel ()
	{
		bool all = Elements.All(e => e.tileSet.Setted);

		return all;
	}

	public void MoveTile(TileElement clicked)
	{
		if (clicked.tileSet.isFixed) return;

		if (from == null)
		{
			from = clicked;

			tileMovement = from.ShowMainHover(centerLake, Elements.Count);

			for (int i = 0; i < tileMovement.Count; i++) 
			{
				Elements[tileMovement[i]].ShowHover();
			}
		}
		else if (to == null)
		{
			if (tileMovement.Contains(clicked.tileSet.position))
			{
				to = clicked;
			}
			else
			{
				ResetMoveItem();
			}
		}

		if (from != null && to != null)
		{
			TileSet tmpTileSet = new TileSet
			{
				isFixed = to.tileSet.isFixed,
				type = to.tileSet.type,
				side = to.tileSet.side,
				position = from.tileSet.position,
				mustBeHere = to.tileSet.mustBeHere
			};
			Sprite tmpSprite = to.Image.sprite;
			Quaternion tmpRotation = to.RectTransform.rotation;

			to.tileSet = new TileSet
			{
				isFixed = from.tileSet.isFixed,
				type = from.tileSet.type,
				side = from.tileSet.side,
				position = to.tileSet.position,
				mustBeHere = from.tileSet.mustBeHere
			};
			to.Image.sprite = from.Image.sprite;
			to.RectTransform.rotation = from.RectTransform.rotation;

			from.tileSet = tmpTileSet;
			from.Image.sprite = tmpSprite;
			from.RectTransform.rotation = tmpRotation;

			from.CheckPlace();
			to.CheckPlace();

			GameManager.Instance.CheckLevel();

			ResetMoveItem ();
		}
	}

	public void ResetMoveItem ()
	{
		if (from != null)
		{
			from.HideHover();

			for (int i = 0; i < tileMovement.Count; i++)
			{
				Elements[tileMovement[i]].HideHover();
			}

			tileMovement = null;
		}

		from = null;
		to = null;
	}

	void Randomize()
	{
		List<Sprite> watersParts = Resources.LoadAll<Sprite> (ResourceWatersPath).ToList();

		var centerItem = watersParts.FirstOrDefault(i => i.name == "lake center");
		if (centerItem != null)
		{
			int[] values = new int[] { 8, 9, 10, 11, 12, 15, 16, 17, 18, 19, 22, 23, 24, 25, 26, 29, 30, 31, 32, 33 };
			centerLake = values[Random.Range(0, values.Length)];
			places = new int[] { centerLake - 8, centerLake - 7, centerLake - 6, centerLake - 1, centerLake + 1, centerLake + 6, centerLake + 7, centerLake + 8 };
			
			Elements[centerLake].Image.sprite = centerItem;
			Elements[centerLake].tileSet.isFixed = true;
			Elements[centerLake].tileSet.type = TileType.Water;
			Elements[centerLake].tileSet.mustBeHere = Elements[centerLake].tileSet.position;
			Elements[centerLake].CheckPlace();

			watersParts.Remove (centerItem);
		}

		List<int> randomList = new List<int> ();
		elements.ForEach (e => randomList.Add (e.tileSet.position));
		randomList.Remove(centerLake);

		for (int i = 0; i < watersParts.Count; i++)
		{
			var item = watersParts[i];

			int[] values = new int[] { 0, -90, -180, -270 };
			int side = values[Random.Range(0, values.Length)];

			int index = randomList[Random.Range(0, randomList.Count)];
			randomList.Remove(index);

			Elements[index].Image.sprite = item;
			Elements[index].RectTransform.Rotate(0, 0, side);
			Elements[index].tileSet.side = (TileSide)side;
			Elements[index].tileSet.type = TileType.Water;

			if (item.name == "left up")
			{
				Elements[index].tileSet.mustBeHere = places[0];
			}
			else if (item.name == "center up")
			{
				Elements[index].tileSet.mustBeHere = places[1];
			}
			else if (item.name == "right up")
			{
				Elements[index].tileSet.mustBeHere = places[2];
			}
			else if (item.name == "left side")
			{
				Elements[index].tileSet.mustBeHere = places[3];
			}
			else if (item.name == "right side")
			{
				Elements[index].tileSet.mustBeHere = places[4];
			}
			else if (item.name == "left down")
			{
				Elements[index].tileSet.mustBeHere = places[5];
			}
			else if (item.name == "center down")
			{
				Elements[index].tileSet.mustBeHere = places[6];
			}
			else if (item.name == "right down")
			{
				Elements[index].tileSet.mustBeHere = places[7];
			}

			Elements[index].CheckPlace();
		}
	}

	#endregion
}