using UnityEngine;

namespace WereHorse.Runtime.Utility.Extensions {
	public static class VectorExtensions {
		public static float YRotation(this Vector3 vector) {
			return Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;
		}

		public static Vector3 ProjectOnGround(this Vector2 vector) {
			return new Vector3(vector.x, 0, vector.y);
		}
		
		public static Vector3 Flatten(this Vector3 vector) {
			return new Vector3(vector.x, 0, vector.z);
		}

		public static Vector2 CScale(this Vector2 vector, float scaleX, float scaleY) {
			return new Vector2(vector.x * scaleX, vector.y * scaleY);
		}
		
		public static Vector3 CScale(this Vector3 vector, float scaleX, float scaleY, float scaleZ) {
			return new Vector3(vector.x * scaleX, vector.y * scaleY, vector.z * scaleZ);
		}
        
        public static Vector3 CScale(this Vector3 vector, Vector3 scale) {
            vector.Scale(scale);
            return vector;
        }
	}
}
