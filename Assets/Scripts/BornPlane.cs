using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BornPlane : MonoBehaviour
{
		public static BornPlane Instance;

		public Transform[] Positions;

		void Awake()
		{
			Instance = this;
		}
}
