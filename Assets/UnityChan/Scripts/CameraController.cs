//CameraController.cs for UnityChan
//Original Script is here:
//TAK-EMI / CameraController.cs
//https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d
//https://twitter.com/TAK_EMI
//
//Revised by N.Kobayashi 2014/5/15 
//Change : To prevent rotation flips on XY plane, use Quaternion in cameraRotate()
//Change : Add the instrustion window
//Change : Add the operation for Mac
//




using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UnityChan
{
	[System.Serializable]
	public class StringCurvePair
	{
		public string Key;
		public Keyframe[] Value;
		public StringCurvePair(string key,Keyframe[] value)
		{
			Key	= key;
			Value	= value;
		}
	}

	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		SpriteRenderer cursor;

		void Start()
		{
			var perpend	= Vector3.Scale(this.transform.forward,new Vector3(1,0,1)).normalized;
			float dot	= Vector3.Dot(perpend,this.transform.position);

			var focus	= this.transform.position - (dot * this.transform.forward);

			this.transform.LookAt(focus,Vector3.up);
			this.transform.localScale	= new Vector3(1,1,(this.transform.position - focus).magnitude);

			var anim	= gameObject.GetComponent<Animation>();
		}

		void Update ()
		{
			cursor.enabled	= false;
			if(!Input.touchSupported)
			{
				float delta	= Input.mouseScrollDelta.y / 3;
				if(delta != 0 && !EventSystem.current.IsPointerOverGameObject())
					this.mouseWheelEvent(delta);

				this.mouseDragEvent();

				if(Input.GetKeyDown(KeyCode.Space))
					this.transform.LookAt(Vector3.zero,Vector3.up);
				return;
			}

			if(Input.touchCount == 1)
			{
				var touch	= Input.touches[0];
				if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
					cameraRotate(8 * new Vector3(touch.deltaPosition.y,touch.deltaPosition.x));

				if(touch.tapCount > 1)
					this.transform.LookAt(Vector3.zero,Vector3.up);
			}
			else if(Input.touchCount == 2)
			{
				var touch0	= Input.touches[0];
				var touch1	= Input.touches[1];
				float dot	= Vector2.Dot(touch0.deltaPosition.normalized,touch1.deltaPosition.normalized);
				if(dot > 0.75f)
					this.cameraTranslate(-touch0.deltaPosition / 100.0f);

				if(dot < -0.66f)
				{
					var to	= (touch0.position - touch1.position).magnitude;
					var from	= ((touch0.position - touch0.deltaPosition) - (touch1.position - touch1.deltaPosition)).magnitude;
					this.mouseWheelEvent((from - to) / 10);
				}
			}
		}

		Vector3 oldPos;
		void mouseDragEvent()
		{
			var mousePos	= Input.mousePosition;
			if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
				this.oldPos = mousePos;

			Vector3 diff = mousePos - oldPos;
			this.oldPos = mousePos;

			bool translate	= Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftCommand);
			if(translate || Input.GetMouseButton(2))
			{
				this.cameraTranslate(-diff / 100.0f);
				return;
			}

			bool rotate	= Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt);
			if(rotate || Input.GetMouseButton(1))
				this.cameraRotate(new Vector3(diff.y,diff.x,0.0f));
		}

		//Dolly
		public void mouseWheelEvent(float delta)
		{
			var scale	= this.transform.localScale;
			var focus	= this.transform.position + (this.transform.forward * scale.z);

			scale.z	= Mathf.Clamp(scale.z - delta,0.2f,10);
						
			this.transform.position	= focus - (this.transform.forward * scale.z);
			this.transform.localScale	= scale;
		}

		void cameraTranslate(Vector3 vec)
		{
			if(vec.magnitude <= Vector3.kEpsilon)
				return;
			
			var pos	= this.transform.position;
			pos	+= this.transform.right * vec.x;
			pos	+= this.transform.up * vec.y;
			this.transform.position	= pos;
		}

		public void cameraRotate(Vector3 eulerAngle)
		{
			cursor.enabled	= true;
			var ray	= Camera.main.ScreenPointToRay(Input.mousePosition);
			cursor.transform.position	= ray.origin + (ray.direction * cursor.gameObject.GetComponentInParent<Canvas>().planeDistance);

			if(eulerAngle.magnitude <= Vector3.kEpsilon)
				return;
			
			var zoom	= this.transform.localScale.z;
			var focus	= this.transform.position + (this.transform.forward * zoom);

			var value	= new Vector2(eulerAngle.x * 180f / Screen.width,eulerAngle.y * 90f / Screen.height);

			var angleOffset	= 1 - (this.transform.localEulerAngles.x / 90);
			var yOffset	= 1 - (2 * Input.mousePosition.y / Screen.height) + (angleOffset * angleOffset);
			value.y	= value.y * Mathf.Clamp(Mathf.Sign(yOffset) * Mathf.Sqrt(Mathf.Abs(yOffset)),-1,1);

			if(Mathf.Sign(yOffset) < 0 ^ cursor.transform.localScale.y < 0)
				cursor.transform.localScale	= Vector3.Scale(cursor.transform.localScale,new Vector3(1,-1,1));

			eulerAngle	= this.transform.localEulerAngles + new Vector3(-value.x,value.y,0);
			eulerAngle.x	= Mathf.Clamp(eulerAngle.x,2,88);
			this.transform.localEulerAngles = eulerAngle;

			this.transform.position	= focus - (this.transform.forward * zoom);
		}
	}
}