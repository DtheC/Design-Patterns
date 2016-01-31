using UnityEngine;
using System.Collections;

public class UserParticle : MonoBehaviour {

	public Rigidbody rigidbody;
	public ParticleState state_;
	public Vector3 floatingAcceleration;

	public Platform originPlatform;

	public int xPositionInPlatform;
	public int zPositionInPlatform;

	private Vector3 newPosition;
	private Vector3 newRotation;
	private float easingSpring = 0.01f;


	
	void Start () {
		rigidbody = gameObject.GetComponent<Rigidbody> ();

		state_ = new StaticState ();
		
		floatingAcceleration = new Vector3 (0, Random.Range(0.05f, 0.50f), 0);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.anyKeyDown) {
			Debug.Log("key down");
			ParticleState state = state_.changeState(this);
			if (state != null){
				state_ = state;
				state_.enter(this);
			}
		}
		state_.update (this);
	}

	public void OriginalPosition(int _x, int _z){
		xPositionInPlatform = _x;
		zPositionInPlatform = _z;
	}

	public void SpringTowardPosition(Vector3 _p)
	{
		newPosition += (_p - transform.position) * easingSpring;
		newPosition *= .92f;
		Vector3 t = transform.position + newPosition;
		transform.position = t;
	}

	public void SpringTowardRotation(Vector3 _p)
	{
		newRotation += (_p - transform.rotation.eulerAngles) * easingSpring;
		newRotation *= .92f;
		Vector3 t = transform.rotation.eulerAngles + newRotation;
		transform.rotation = Quaternion.Euler(t);
	}
}

public class ParticleState {
	public virtual void enter(UserParticle particle){}
	public virtual ParticleState changeState(UserParticle particle){
		return null;
	}
	public virtual void update (UserParticle particle){}
}

class StaticState : ParticleState {

	public override void enter (UserParticle particle)
	{
		particle.rigidbody.velocity = Vector3.zero;
		particle.rigidbody.isKinematic = true;
	}

	public override ParticleState changeState(UserParticle particle){
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (Random.Range(0,100) > 98){
				Debug.Log ("State: Floating");
				return new FloatingState();
			}
		}
		return null;
	}
}

class FloatingState : ParticleState {
	public override void enter (UserParticle particle)
	{
		particle.rigidbody.isKinematic = false;
	}

	public override ParticleState changeState(UserParticle particle){
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			Debug.Log ("State: Returning");
			return new ReturningState();
		}
		return null;
	}

	public override void update(UserParticle particle){
		particle.rigidbody.AddForce (particle.floatingAcceleration);
	}
}

class ReturningState : ParticleState {
	public override void enter (UserParticle particle)
	{
		particle.rigidbody.isKinematic = true;
	}

	public override ParticleState changeState(UserParticle particle){
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (Random.Range(0,100) > 98){
				Debug.Log ("State: Floating");
				return new FloatingState();
			}
		}
		return null;
	}

	public override void update(UserParticle particle){
		particle.rigidbody.detectCollisions = false;
		particle.transform.position = Vector3.Lerp (particle.transform.position, particle.originPlatform.ParticleOriginalLocation (particle.xPositionInPlatform, particle.zPositionInPlatform), 0.1f);
		particle.transform.rotation = Quaternion.Lerp (particle.transform.rotation, Quaternion.AngleAxis(0, Vector3.zero), Time.time * 0.1f);

		if (Vector3.Distance(particle.transform.position, particle.originPlatform.ParticleOriginalLocation(particle.xPositionInPlatform, particle.zPositionInPlatform)) < 0.3f){

			particle.transform.position = particle.originPlatform.ParticleOriginalLocation(particle.xPositionInPlatform, particle.zPositionInPlatform);
			particle.transform.rotation = Quaternion.AngleAxis(0, Vector3.zero);

			//This seems dirty- should I be setting states here? I guess really I wouldn't be using input on the 'real' thing
			particle.state_ = new StaticState();
			particle.state_.enter(particle);
			Debug.Log ("State: Static");
		}

	}
}