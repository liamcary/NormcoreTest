using UnityEngine;

public class ToyCarCamera : MonoBehaviour
{
	public Vector3 Offset;
	public float OffsetScalarStopped;
	public float OffsetScalarFast;
	public float SpeedAtFastOffset;

	ToyCar _toyCar;
	Vector3 currentOffset;
	Vector3 currentVelocity;

	void Update()
	{
		var horizontalForward = Vector3.Scale(_toyCar.transform.forward, new Vector3(1f, 0f, 1f));
		float horizontalAngle = Vector3.SignedAngle(Vector3.forward, horizontalForward, Vector3.up);
		var targetOffset = Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Offset;

		var velocity = _toyCar.Rigidbody.velocity;
		float t = Mathf.InverseLerp(0f, SpeedAtFastOffset, velocity.magnitude);
		float scalar = Mathf.Lerp(OffsetScalarStopped, OffsetScalarFast, t);

		targetOffset *= scalar;

		currentOffset = Vector3.SmoothDamp(currentOffset, targetOffset, ref currentVelocity, 0.5f);

		transform.position = _toyCar.transform.position + currentOffset;
		transform.LookAt(_toyCar.transform.position);
	}

	public void Initialise(ToyCar car)
	{
		_toyCar = car;
	}
}