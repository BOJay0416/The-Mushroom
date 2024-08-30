using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieCamera : MonoBehaviour
{
		public static DieCamera Instance;

		public Transform[] Camera;

		void Awake()
		{
			Instance = this;
		}
}
