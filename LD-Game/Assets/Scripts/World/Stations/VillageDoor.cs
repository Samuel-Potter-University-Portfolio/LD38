using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDoor : MonoBehaviour
{
	public static VillageDoor Main { get; private set; }


	void Start ()
	{
		Main = this;	
	}
	
}
