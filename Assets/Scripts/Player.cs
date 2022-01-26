using Normal.Realtime;
using UnityEngine;
using static Normal.Realtime.Realtime;

public class Player : RealtimeComponent<PlayerModel>
{
	public bool IsToyCar => model.isCar;
	public bool IsAvatar => model.isAvatar;

	GameObject _instance;
	ToyCar _toyCar;
	Avatar _avatar;

	public void JoinAsToyCar()
	{
		if (!isOwnedLocallyInHierarchy) {
			Debug.LogError("Cannot join as toy car when not owned locally");
			return;
		}

		model.isCar = true;
		model.isAvatar = false;

		Debug.Log("Instantiating toy car");

		_instance = Realtime.Instantiate("ToyCar", new InstantiateOptions {
			ownedByClient = true,
			preventOwnershipTakeover = true,
			destroyWhenOwnerLeaves = true,
			destroyWhenLastClientLeaves = true,
			useInstance = realtime
		});

		_toyCar = _instance.GetComponent<ToyCar>();
	}

	public void JoinAsAvatar()
	{
		if (!isOwnedLocallyInHierarchy) {
			Debug.LogError("Cannot join as avatar when not owned locally");
			return;
		}

		model.isCar = false;
		model.isAvatar = true;

		Debug.Log("Instantiating avatar");

		_instance = Realtime.Instantiate("Avatar", new InstantiateOptions {
			ownedByClient = true,
			preventOwnershipTakeover = true,
			destroyWhenOwnerLeaves = true,
			destroyWhenLastClientLeaves = true,
			useInstance = realtime
		});

		_avatar = _instance.GetComponent<Avatar>();
	}

	public void Leave()
	{
		if (_toyCar != null) {
			Realtime.Destroy(_toyCar.gameObject);
		}

		if (_avatar != null) {
			Realtime.Destroy(_avatar.gameObject);
		}
	}
}