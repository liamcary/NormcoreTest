
using Normal.Realtime;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static Normal.Realtime.Realtime;

public class Game : MonoBehaviour
{
	[SerializeField] Realtime _realtime;
	[SerializeField] string _roomName = "Test";

	Player _player;

	const int _buttonWidth = 256;
	const int _buttonHeight = 60;
	GUIStyle _style;

	void Reset()
	{
		_realtime = GetComponent<Realtime>();
	}

	void Start()
	{
		_style = new GUIStyle("button") {
			fontSize = 36
		};

		_realtime.didConnectToRoom += HandleConnectedToRoom;
	}

	void OnDestroy()
	{
		_realtime.didConnectToRoom -= HandleConnectedToRoom;
	}

	void HandleConnectedToRoom(Realtime realtime)
	{
		var playerInstance = Realtime.Instantiate("Player", new InstantiateOptions {
			ownedByClient = true,
			preventOwnershipTakeover = true,
			destroyWhenOwnerLeaves = true,
			destroyWhenLastClientLeaves = true
		});

		_player = playerInstance.GetComponent<Player>();
	}

	void OnGUI()
	{
		bool hasConnected = _realtime.connected || _realtime.connecting;

		GUI.enabled = !hasConnected;

		if (GUI.Button(new Rect(0 * _buttonWidth, 0f, _buttonWidth, _buttonHeight), "Join Room", _style)) {
			_realtime.Connect(_roomName);
		}

		GUI.enabled = hasConnected;

		if (GUI.Button(new Rect(1 * _buttonWidth, 0f, _buttonWidth, _buttonHeight), "Leave Room", _style)) {
			_realtime.Disconnect();
		}

		GUI.enabled = _realtime.connected && _player != null && !_player.IsToyCar && !_player.IsAvatar;

		if (GUI.Button(new Rect(2 * _buttonWidth, 0f, _buttonWidth, _buttonHeight), "Join As Avatar", _style)) {
			_player.JoinAsAvatar();
		}

		if (GUI.Button(new Rect(3 * _buttonWidth, 0, _buttonWidth, _buttonHeight), "Join As Toy Car", _style)) {
			_player.JoinAsToyCar();
		}

		GUI.enabled = true;
	}
}