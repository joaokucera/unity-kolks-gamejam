using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum TileType
{
	Water,
	Ground
}

public enum TileSide
{
	Zero = 0,
	One = -90,
	Two = -180,
	Three = -270
}

[Serializable]
public class TileSet
{
	public bool isFixed;
	public TileType type;
	public TileSide side;
	public int position;
	public int mustBeHere;

	public bool Setted
	{
		get
		{
			return (type == TileType.Ground || (type == TileType.Water && side == TileSide.Zero && position == mustBeHere));
		}
	}
	public bool OnPlace
	{
		get
		{
			return (type == TileType.Water && side == TileSide.Zero && position == mustBeHere);
		}
	}
}

public class TileElement : MonoBehaviour, IPointerClickHandler
{
	#region [ FIELDS ]
	
	public TileSet tileSet = new TileSet();
	private Color originalColor;
	private Image image;
	private Button button;
	private const int zRotation = -90;
	private const float speedRotation = 5f;
	private const float rotationTime = 0.5f;
	private RectTransform rectTransform;
	private Hover childHover;
	private Check childCheck;

	#endregion

	#region [ PROPERTIES ]

	public Hover ChildHover
	{
		get 
		{
			if (childHover == null)
			{
				childHover = GetComponentInChildren<Hover>();
			}

			return childHover;
		}
	}
	public Check ChildCheck
	{
		get 
		{
			if (childCheck == null)
			{
				childCheck = GetComponentInChildren<Check>();
			}
			
			return childCheck;
		}
	}
	public RectTransform RectTransform 
	{
		get 
		{
			if (rectTransform == null)
			{
				rectTransform = transform as RectTransform;
			}
			
			return rectTransform;
		}
	}
	public Image Image 
	{
		get 
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}

			return image;
		}
	}
	public Button Button
	{
		get 
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
			
			return button;
		}
	}

	#endregion

	#region [ METHODS ]

	public void ResetTile()
	{
		tileSet = new TileSet();
	}
	
	public void ResetImage()
	{
		ChildCheck.HideImage();
	}

	public void ShowHover ()
	{
		ChildHover.EnableAnimatorAndImage (new Color(0.5f,0.5f,0.5f,0.5f));
	}

	public List<int> ShowMainHover (int centerLake, int max)
	{
		ChildHover.EnableAnimatorAndImage (Color.white);

		int current = tileSet.position;
		List<int> list;
		if (current == 0)
		{
			list = new List<int> { current + 1, current + 7, current + 8 };
		}
		else if (current == 6)
		{
			list = new List<int> { current - 1, current + 6, current + 7 };
		}
		else if (current == 35)
		{
			list = new List<int> { current - 7, current - 6, current + 1 };
		}
		else if (current == 41)
		{
			list = new List<int> { current - 8, current - 7, current - 1 };
		}
		else if (current % 7 == 0)
		{
			list = new List<int> { current - 7, current - 6, current + 1, current + 7, current + 8 };
		}
		else if ((current + 1) % 7 == 0)
		{
			list = new List<int> { current - 8, current - 7, current - 1, current + 6, current + 7 };
		}
		else
		{
			list = new List<int> { current - 8, current - 7, current - 6, current - 1, current + 1, current + 6, current + 7, current + 8 };
		}
		list.RemoveAll (l => l == centerLake || l < 0 || l >= max);

		return list;
	}

	public void HideHover ()
	{
		ChildHover.DisableAnimatorAndImage();
	}

	public void CheckPlace()
	{
		if (tileSet.OnPlace)
		{
			ChildCheck.ShowImage();
		}
		else
		{
			ChildCheck.HideImage();
		}
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Middle && !tileSet.isFixed)
		{
			RectTransform.Rotate(0, 0, zRotation);

			if (tileSet.side == TileSide.Zero)
			{
				tileSet.side = TileSide.One;
			}
			else if (tileSet.side == TileSide.One)
			{
				tileSet.side = TileSide.Two;
			}
			else if (tileSet.side == TileSide.Two)
			{
				tileSet.side = TileSide.Three;
			}
			else if (tileSet.side == TileSide.Three)
			{
				tileSet.side = TileSide.Zero;
			}

			CheckPlace();

			BoardManager.Instance.ResetMoveItem();
			GameManager.Instance.CheckLevel();
		}
	}

	#endregion
}