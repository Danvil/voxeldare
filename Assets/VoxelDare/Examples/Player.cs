using UnityEngine;

public class Player : MonoBehaviour
{
	public VoxelDare.VoxelRenderer voxelRenderer;

	public bool flying = false;

	public float speed = 1.0f;

	public float sensitivity = 15.0f;
	 
	public float minimumX = -360.0f;
	public float maximumX = 360.0f;
	 
	public float minimumY = -60.0f;
	public float maximumY = 60.0f;
	 
	float rotationX = 0.0f;
	float rotationY = 0.0f;
	 
	Quaternion originalRotation;

	void Start()
	{
		if(rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	void Update()
	{
		// look

		rotationX += Input.GetAxis("Mouse X") * sensitivity;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		 
		rotationX = ClampAngle(rotationX, minimumX, maximumX);
		rotationY = ClampAngle(rotationY, minimumY, maximumY);

		Quaternion rotX = Quaternion.AngleAxis(rotationX, Vector3.up);
		Quaternion rotY = Quaternion.AngleAxis(rotationY, -Vector3.right);
		 
		transform.localRotation = originalRotation * rotX * rotY;

		// move

		float dz = Input.GetAxis("Vertical");
		float dx = Input.GetAxis("Horizontal");
		Vector3 delta = Time.deltaTime*speed*Limit(new Vector3(dx,0,dz), 1.0f);
		if(flying) {
			delta = this.transform.rotation * delta;
		}
		else {
			delta = Quaternion.AngleAxis(this.transform.rotation.eulerAngles.y, Vector3.up) * delta;
		}
		Vector3 posnew = this.transform.position + delta;
		bool hasTopVoxel = voxelRenderer.Voxels.HasTopVoxel(posnew);
		if(flying) {
			this.transform.position = posnew;
		}
		else {
			// check if we can move there
			if(hasTopVoxel) {
				this.transform.position = posnew;
				// move onto to voxel
				Vector3 p = this.transform.position;
				p.y = voxelRenderer.Voxels.GetTopVoxelHeight(p) + 1.0f;
				this.transform.position = p;
			}
		}

		// toogle fly
		if(Input.GetKeyDown(KeyCode.F)) {
			if(flying) {
				// only toggle back to non-flying if we are on supported ground
				if(hasTopVoxel)
					flying = false;
			}
			else {
				flying = true;
			}
		}
		
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	public static Vector3 Limit(Vector3 v, float maxlen)
	{
		if(maxlen == 0.0f) {
			return Vector3.zero;
		}
		float l = v.magnitude;
		if(l > maxlen) {
			return v * (maxlen/l);
		}
		else {
			return v;
		}
	}

}
