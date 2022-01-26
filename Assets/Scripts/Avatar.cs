using Normal.Realtime;
using Normal.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

[ExecutionOrder(-95)] // Make sure our Update() runs before the default to so that the avatar positions are as up to date as possible when everyone else's Update() runs.
public class Avatar : RealtimeComponent<AvatarModel>
{
	public bool IsLocal { get; private set; }

	/// <summary>
	/// The XR device type of the client that owns this avatar. See RealtimeAvatar#DeviceType for values.
	/// </summary>
	public RealtimeAvatar.DeviceType DeviceType
	{
		get => model.deviceType;
		set => model.deviceType = value;
	}

	/// <summary>
	/// The XRDevice.model of the client that owns this avatar.
	/// </summary>
	public string DeviceModel
	{
		get => model.deviceModel;
		set => model.deviceModel = value;
	}

	[SerializeField] Transform _head;
	[SerializeField] Transform _leftHand;
	[SerializeField] Transform _rightHand;

	[SerializeField] RealtimeView _realtimeView;
	[SerializeField] Camera _camera;

	static readonly List<XRNodeState> _nodeStates = new List<XRNodeState>();

	void Start()
	{
		_camera.gameObject.SetActive(_realtimeView.isOwnedLocallyInHierarchy);

		if (_realtimeView.isOwnedLocallyInHierarchy) {
			InitializeXR();

			_realtimeView.RequestOwnership();

			var realtimeComponents = GetComponentsInChildren<IRealtimeComponent>();
			foreach (var component in realtimeComponents) {
				component.RequestOwnership();
			}
		}
	}

	void OnDestroy()
	{
		DeinitializeXR();
	}

	void InitializeXR()
	{
		if (XRGeneralSettings.Instance.Manager.activeLoader != null) {
			DeinitializeXR();
		}

		XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
		XRGeneralSettings.Instance.Manager.StartSubsystems();
	}

	void DeinitializeXR()
	{
		XRGeneralSettings.Instance.Manager.StopSubsystems();
		XRGeneralSettings.Instance.Manager.DeinitializeLoader();
	}

	void FixedUpdate()
	{
		UpdateAvatarTransformsForLocalPlayer();
	}

	void Update()
	{
		UpdateAvatarTransformsForLocalPlayer();
	}

	void LateUpdate()
	{
		UpdateAvatarTransformsForLocalPlayer();
	}

	protected override void OnRealtimeModelReplaced(AvatarModel previousModel, AvatarModel currentModel)
	{
		if (previousModel != null) {
			previousModel.headActiveDidChange -= ActiveStateChanged;
			previousModel.leftHandActiveDidChange -= ActiveStateChanged;
			previousModel.rightHandActiveDidChange -= ActiveStateChanged;
		}

		if (currentModel != null) {
			currentModel.headActiveDidChange += ActiveStateChanged;
			currentModel.leftHandActiveDidChange += ActiveStateChanged;
			currentModel.rightHandActiveDidChange += ActiveStateChanged;
		}
	}

	public void SetLocalPlayer()
	{
		if (IsLocal) {
			return;
		}

		IsLocal = true;

		if (TryGetComponent<RealtimeTransform>(out var rootRealtimeTransform)) {
			rootRealtimeTransform.RequestOwnership();
		}

		if (_head != null && _head.TryGetComponent<RealtimeTransform>(out var headRealtimeTransform)) {
			headRealtimeTransform.RequestOwnership();
		}

		if (_leftHand != null && _head.TryGetComponent<RealtimeTransform>(out var leftHandRealtimeTransform)) {
			leftHandRealtimeTransform.RequestOwnership();
		}

		if (_head != null && _head.TryGetComponent<RealtimeTransform>(out var rightHandRealtimeTransform)) {
			rightHandRealtimeTransform.RequestOwnership();
		}
	}

	void ActiveStateChanged(AvatarModel model, bool nodeIsActive)
	{
		// Leave the head active so RealtimeAvatarVoice runs even when the head isn't tracking.
		if (_leftHand != null) {
			_leftHand.gameObject.SetActive(model.leftHandActive);
		}

		if (_rightHand != null) {
			_rightHand.gameObject.SetActive(model.rightHandActive);
		}
	}

	void UpdateAvatarTransformsForLocalPlayer()
	{
		// Make sure this avatar is a local player
		if (IsLocal) {
			return;
		}

		InputTracking.GetNodeStates(_nodeStates); // the list is cleared by GetNodeStates

		foreach (var nodeState in _nodeStates) {
			if (nodeState.nodeType == XRNode.Head) {
				model.headActive = nodeState.tracked;
				UpdateTransformWithNodeState(_head, nodeState);
			} else if (nodeState.nodeType == XRNode.LeftHand) {
				model.leftHandActive = nodeState.tracked;
				UpdateTransformWithNodeState(_leftHand, nodeState);
			} else if (nodeState.nodeType == XRNode.RightHand) {
				model.rightHandActive = nodeState.tracked;
				UpdateTransformWithNodeState(_rightHand, nodeState);
			}
		}
	}

	static void UpdateTransformWithNodeState(Transform transform, XRNodeState state)
	{
		if (state.TryGetPosition(out var position)) {
			transform.localPosition = position;
		}

		if (state.TryGetRotation(out var rotation)) {
			transform.localRotation = rotation;
		}
	}
}