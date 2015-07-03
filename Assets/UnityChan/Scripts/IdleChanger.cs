using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace UnityChan
{
//
// ↑↓キーでループアニメーションを切り替えるスクリプト（ランダム切り替え付き）Ver.3
// 2014/04/03 N.Kobayashi
//
// Require these components when using this script
	[RequireComponent(typeof(Animator))]
	public class IdleChanger : MonoBehaviour
	{
		[SerializeField]
		VerticalLayoutGroup group	= null;
		[SerializeField]
		GameObject TogglePrefab	= null;
		[SerializeField]
		GameObject ButtonPrefab	= null;
		[SerializeField]
		GameObject SliderPrefab	= null;

		// Use this for initialization
		void Start ()
		{
			// 各参照の初期化
			var anim = gameObject.GetComponent<Animator>();

			if(group == null)
				return;

			foreach(var param in anim.parameters)
			{
				switch(param.type)
				{
					case AnimatorControllerParameterType.Bool:
						var toggleObj	= GameObject.Instantiate(TogglePrefab);
						toggleObj.transform.SetParent(group.transform,false);
						var toggle	= toggleObj.GetComponentInChildren<Toggle>();

						toggle.isOn	= param.defaultBool;
						toggle.GetComponentInChildren<Text>().text	= param.name;
						toggle.onValueChanged.AddListener((b) => anim.SetBool(toggle.GetComponentInChildren<Text>().text,b));
						break;
					case AnimatorControllerParameterType.Trigger:
						var buttonObj	= GameObject.Instantiate(ButtonPrefab);
						buttonObj.transform.SetParent(group.transform,false);
						var button	= buttonObj.GetComponentInChildren<Button>();

						button.GetComponentInChildren<Text>().text	= param.name;
						button.onClick.AddListener(() => anim.SetBool(button.GetComponentInChildren<Text>().text,true));
						break;
				}
			}

			var sliderObj	= GameObject.Instantiate(SliderPrefab);
			sliderObj.transform.SetParent(group.transform,false);
			var slider	= sliderObj.GetComponentInChildren<Slider>();
			slider.onValueChanged.AddListener((f) => anim.SetFloat("Speed",f));
		}
	}
}
