using UnityEngine;
using System.Collections;


public class FollowCamera : MonoBehaviour {



	public CharacterController target;
	public Vector3 focusAreaSize;

	public float verticalOffset;

	FocusArea focusArea;

	void Start(){
		focusArea = new FocusArea (target.bounds, focusAreaSize);


	}

	void LateUpdate(){
		focusArea.Update (target.bounds);

		Vector3 focusPosition = focusArea.center + Vector3.up * verticalOffset;

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	
	}

	/*void OnDrawGizmos(){
		Gizmos.color = new Color (1, 0, 0, 0.5f);
		Gizmos.DrawCube (focusArea.center, focusAreaSize);
	}*/

	struct FocusArea{
		public Vector3 center;
		public Vector3 velocity;
		float left,right;
		float top,bottom;

		public FocusArea(Bounds targetBounds, Vector3 size){
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			top = targetBounds.min.y+size.y;
			bottom = targetBounds.min.y;

			velocity = Vector3.zero;
			center = new Vector3((left+right)/2,(top+bottom)/2,0);
		}

		public void Update(Bounds targetBounds){
			float shiftx = 0;
			if (targetBounds.min.x < left) {
				shiftx = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftx = targetBounds.max.x - right;
			}
			left += shiftx;
			right += shiftx;

			float shifty = 0;
			if (targetBounds.min.y < bottom) {
				shifty = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shifty = targetBounds.max.y - top;
			}
			bottom += shifty;
			top += shifty;
			center = new Vector3 ((left + right) / 2, (top + bottom) / 2, 0);
			velocity = new Vector3 (shiftx, shifty, 0);
		}
	}
}
