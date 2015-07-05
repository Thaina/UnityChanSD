using UnityEngine;
using UnityEngine.UI;

using System.Linq;

namespace UnityChan
{
	public class FaceUpdate : MonoBehaviour
	{
		public AnimationClip[] Animations;
		public float DelayWeight;
		public string KeepFace	= null;

		Animator anim;
		ToggleGroup toggles;
		void Start()
		{
			anim	= gameObject.GetComponent<Animator>();

			Animations	= anim.runtimeAnimatorController.animationClips;
			Animations	= Animations.Where((animation) => animation.name.Contains("@sd_")).OrderBy((animation) => animation.name).ToArray();
		}

		public AnimatorClipInfo[] ClipInfos
		{
			get { return anim.GetCurrentAnimatorClipInfo(1); }
		}
		
		float current = 0;
		void Update()
		{
			if(Input.GetMouseButton(0))
				current = 1;
			else if(string.IsNullOrEmpty(KeepFace))
				current = Mathf.Lerp(current,0,DelayWeight);

			anim.SetLayerWeight(1,current);
		}

		//アニメーションEvents側につける表情切り替え用イベントコール
		public void OnCallChangeFace(string str)
		{
			if(string.IsNullOrEmpty(str) || Animations.FirstOrDefault((animation) => animation.name == str) == null)
				str	= KeepFace;

			current	= 1;

			anim.CrossFade(str,0.1f,1);
		}
	}
}
