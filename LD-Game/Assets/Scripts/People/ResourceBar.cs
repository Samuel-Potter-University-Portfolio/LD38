using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ResourceBar
{
	private RectTransform Bar;
	private float BarFullWidth;

	public float Value { get { return ActualValue > max ? max : ActualValue < min ? min : ActualValue; } }
	public float NormalizedValue { get { return Value / (max - min); } }
	public float min;
	public float max;
	public float decay;

	private float ActualValue;


	public ResourceBar(float min, float max, float decay)
	{
		this.min = min;
		this.max = max;
		this.decay = decay;
		Restore();
    }

	public void SetAnimBar(RectTransform Bar)
	{
		this.Bar = Bar;
		BarFullWidth = Bar.sizeDelta.x;
    }

	public void Restore()
	{
		ActualValue = max + decay * 10.0f;
    }

	public void Update(float deltaTime)
	{
		ActualValue -= decay * deltaTime;

		//Animate bar
		if (Bar != null)
			Bar.sizeDelta = new Vector2(BarFullWidth * NormalizedValue, Bar.sizeDelta.y);
    }
}
