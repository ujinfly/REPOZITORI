using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	public float lifeTime;

	void Start () {
		Destroy (gameObject ,lifeTime );
	}
}
