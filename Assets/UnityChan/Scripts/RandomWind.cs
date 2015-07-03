//
//RandomWind.cs for unity-chan!
//
//Original Script is here:
//ricopin / RandomWind.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//修正2014/12/20
//風の方向変化/重力影響を追加.
//

using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class RandomWind : MonoBehaviour
	{
		public bool isWindActive = false;

		public float threshold = 0.5f;				// ランダム判定の閾値.
		public float interval = 5.0f;				// ランダム判定のインターバル.
		public float windPower = 10;				//風の強さ.
		public float gravity = 0.98f;				//重力の強さ.

		void Start ()
		{
			StartCoroutine ("RandomChange");
		}

		Vector3 direction	= new Vector3(0,1,0);
		void Update ()
		{
			var force = 0.001f * direction;
			if(isWindActive)
			{
				float str	= windPower * Mathf.PerlinNoise(Time.time,0.0f);
				force.x	*= str;
				force.z	*= str;
			}
			else
			{
				force.x	= 0;
				force.z	= 0;
			}

			foreach(var spring in gameObject.GetComponent<SpringManager>().springBones)
				spring.springForce	= force;
		}
		
		void OnGUI ()
		{
			Rect rect1 = new Rect (Screen.width * 0.5f, Screen.height - 40, 400, 30);
			isWindActive = GUI.Toggle (rect1, isWindActive, "Random Wind");
		}

		// ランダム判定用関数.
		IEnumerator RandomChange ()
		{
			// 無限ループ開始.
			while (true) {
				//ランダム判定用シード発生.
				direction.x	= Random.Range(0.0f,1.0f);
				direction.z	= 1 - direction.x;

				direction.x	*= Random.Range(0.0f,1.0f) > 0.5f ? -1 : 1;
				direction.z	*= Random.Range(0.0f,1.0f) > 0.5f ? -1 : 1;
				direction.y	= -gravity;

				// 次の判定までインターバルを置く.
				yield return new WaitForSeconds (interval);
			}
		}


	}
}