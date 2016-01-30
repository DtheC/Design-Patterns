using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

	public int widthInParticles;
	public int heightInParticles;

	public GameObject particleObject;

	private GameObject[] platformParticles;

	void Awake(){
		platformParticles = new GameObject[widthInParticles*heightInParticles];

		for (int x = 0; x < widthInParticles; x++){
			for (int z = 0; z < heightInParticles; z++){
				platformParticles[x+z] = Instantiate(particleObject, new Vector3(x*1.0f,0,z*1.0f), transform.rotation) as GameObject;
				platformParticles[x+z].GetComponent<UserParticle>().OriginalPosition(x, z);
				platformParticles[x+z].GetComponent<UserParticle>().originPlatform = this;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 ParticleOriginalLocation(int _x, int _z){
		return gameObject.transform.position + new Vector3 (_x * 1.0f, 0, _z * 1.0f);
	}

}
