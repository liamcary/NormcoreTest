using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor
{
	Red,
	Orange,
	Yellow,
	Green,
	Blue,
	Indigo,
	Violet,
	Black
}

public static class PlayerColorExtensions
{
	static readonly Dictionary<PlayerColor, Color> _colorMap;

	static PlayerColorExtensions()
	{
		_colorMap = new Dictionary<PlayerColor, Color> {
			{ PlayerColor.Red, Color.red },
			{ PlayerColor.Orange, new Color(1.0f, 0.65f, 0f, 1f)},
			{ PlayerColor.Yellow, Color.yellow},
			{ PlayerColor.Green, Color.green},
			{ PlayerColor.Blue, Color.blue},
			{ PlayerColor.Indigo, new Color(0.3f, 0f, 0.5f, 1f)},
			{ PlayerColor.Violet, new Color(0.5f, 0f, 0.5f, 1f)},
			{ PlayerColor.Black, Color.black},
		};
	}

	public static Color ToColor(this PlayerColor playerColor)
	{
		return _colorMap[playerColor];
	}
}