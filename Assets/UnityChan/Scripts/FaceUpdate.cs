using UnityEngine;
using UnityEngine.UI;

using System.Linq;

namespace UnityChan
{
	public class FaceUpdate : MonoBehaviour
	{
		[SerializeField]
		GameObject TogglePrefab	= null;

		public AnimationClip[] animations;
		Animator anim;
		public float delayWeight;
		public bool isKeepFace = false;

		ToggleGroup toggles;
		void Start ()
		{
			anim	= gameObject.GetComponent<Animator>();

			toggles	= GameObject.FindObjectsOfType<ToggleGroup>().FirstOrDefault((obj) => obj.name == "FaceSelector");
			toggles.allowSwitchOff	= false;

			foreach(var animation in animations)
			{
				var toggle	= GameObject.Instantiate(TogglePrefab).GetComponent<Toggle>();
				toggle.transform.SetParent(toggles.transform,false);
				toggle.group	= toggles;

				toggle.isOn	= animation.name == lastSelected;
				toggle.GetComponentInChildren<Text>().text	= animation.name;
				toggle.onValueChanged.AddListener((b) => {
					lastSelected	= toggle.GetComponentInChildren<Text>().text;
					anim.CrossFade(lastSelected,0.1f);
				});
			}

			var keep	= GameObject.Instantiate(TogglePrefab).GetComponent<Toggle>();
			keep.transform.SetParent(toggles.transform,false);
			keep.GetComponentInChildren<Text>().text	= " Keep Face";
			keep.isOn	= isKeepFace;
			keep.onValueChanged.AddListener((b) => isKeepFace = b);

			toggles.gameObject.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
			toggles.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
		}

		float current = 0;
		string lastSelected	= "default@sd_generic";
		void Update ()
		{
			if(Input.GetMouseButton(0))
				current = 1;
			else if(!isKeepFace)
				current = Mathf.Lerp(current,0,delayWeight);

			anim.SetLayerWeight(1,current);
		}
	 

		//アニメーションEvents側につける表情切り替え用イベントコール
		public void OnCallChangeFace(string str)
		{
			isKeepFace	= true;
			current	= 1;

			var exist	= animations.FirstOrDefault((animation) => animation.name == str);

			anim.CrossFade(exist != null ? str : lastSelected,0.1f,1);
		}
	}
}
