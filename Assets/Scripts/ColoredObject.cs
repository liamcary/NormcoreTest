using Normal.Realtime;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ColoredObject : RealtimeComponent<ColoredObjectModel>
{
	[SerializeField] protected MeshRenderer[] _primaryRenderers;
	[SerializeField] protected MeshRenderer[] _secondaryRenderers;

	protected virtual void Update()
	{
		if (!isOwnedLocallyInHierarchy) {
			return;
		}

		if (Keyboard.current[Key.P].wasPressedThisFrame) {
			model.primaryColor = GetRandomColor();
		} else if (Keyboard.current[Key.O].wasPressedThisFrame) {
			model.secondaryColor = GetRandomColor();
		}
	}

	protected PlayerColor GetRandomColor()
	{
		return (PlayerColor) Random.Range(0, Enum.GetValues(typeof(PlayerColor)).Length);
	}

	protected override void OnRealtimeModelReplaced(ColoredObjectModel previousModel, ColoredObjectModel currentModel)
	{
		if (previousModel != null) {
			previousModel.primaryColorDidChange -= HandlePrimaryColorChanged;
			previousModel.secondaryColorDidChange -= HandleSecondaryColorChanged;
		}

		if (currentModel != null) {
			if (currentModel.isFreshModel) {
				currentModel.primaryColor = PlayerColor.Red;
				currentModel.secondaryColor = PlayerColor.Black;
			}

			HandlePrimaryColorChanged(currentModel, currentModel.primaryColor);
			HandleSecondaryColorChanged(currentModel, currentModel.secondaryColor);

			currentModel.primaryColorDidChange += HandlePrimaryColorChanged;
			currentModel.secondaryColorDidChange += HandleSecondaryColorChanged;
		}
	}

	protected virtual void HandlePrimaryColorChanged(ColoredObjectModel model, PlayerColor newValue)
	{
		var color = newValue.ToColor();

		foreach (var renderer in _primaryRenderers) {
			renderer.material.color = color;
		}
	}

	protected virtual void HandleSecondaryColorChanged(ColoredObjectModel model, PlayerColor newValue)
	{
		var color = newValue.ToColor();

		foreach (var renderer in _secondaryRenderers) {
			renderer.material.color = color;
		}
	}
}