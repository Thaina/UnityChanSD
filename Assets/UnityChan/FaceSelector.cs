using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;
using System.Collections.Generic;

namespace UnityChan
{
	public class FaceSelector : MonoBehaviour
	{
		[SerializeField]
		GameObject TogglePrefab	= null;

		[SerializeField]
		FaceUpdate faceUpdate	= null;

		readonly List<Toggle> allToggle	= new List<Toggle>();
		Toggle keep;
		void Start()
		{
			var toggles	= gameObject.GetComponent<ToggleGroup>();
			toggles.allowSwitchOff	= false;

			keep	= GameObject.Instantiate(TogglePrefab).GetComponent<Toggle>();
			keep.transform.SetParent(toggles.transform,false);
			keep.GetComponentInChildren<Text>().text	= " Keep Face";
			keep.onValueChanged.AddListener((b) => faceUpdate.KeepFace = null);
			keep.isOn	= true;

			foreach(var animation in faceUpdate.Animations)
			{
				var toggle	= GameObject.Instantiate(TogglePrefab).GetComponent<Toggle>();
				toggle.transform.SetParent(toggles.transform,false);
				toggle.group	= toggles;

				toggle.name	= animation.name;
				toggle.isOn	= toggle.name == faceUpdate.KeepFace;
				toggle.GetComponentInChildren<Text>().text	= toggle.name;
				toggle.onValueChanged.AddListener((value) => {
					if(value)
					{
						if(keep.isOn)
							faceUpdate.KeepFace	= toggle.name;
						faceUpdate.OnCallChangeFace(toggle.name);
					}
				});

				allToggle.Add(toggle);
			}

			toggles.gameObject.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
			toggles.gameObject.GetComponent<ContentSizeFitter>().SetLayoutVertical();
		}
	}
}