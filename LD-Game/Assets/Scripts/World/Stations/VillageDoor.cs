using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDoor : MonoBehaviour
{
	public static VillageDoor Main { get; private set; }

	public float Productivity { get; private set; }
	private float ProductivityDecay = 0.001f;

	public float Health = 10.0f;
	public float MaxHealth = 10.0f;
	public float NormalizedHealth {	get { return Mathf.Clamp(Health / MaxHealth, 0.0f, 1.0f); } }


	void Start ()
	{
		Main = this;
		Productivity = 1.0f;
		MaxHealth = Health;
    }

	void Update()
	{
		Productivity -= ProductivityDecay * Time.deltaTime;
		Productivity = Mathf.Clamp(Productivity, 0.0f, NormalizedHealth);


		if (Productivity == 0.0f)
			Health -= ProductivityDecay * Time.deltaTime;


		if (Health <= 0)
		{
			Destroy(gameObject);
			SceneAssist.LevelSwitch("GameOver");
        }
    }

	public void OnCompleteQuest(QuestMeta meta)
	{
		Productivity += 0.2f;
		Productivity = Mathf.Clamp(Productivity, 0.0f, NormalizedHealth);
	}

	public bool Attack(ItemID what, Person who)
	{
		if (who.IsPlayer)
			return false;

		float damage = ItemController.Library.ContainsKey(what) ? ItemController.Library[what].Damage : 0.1f;
		

		Health -= damage;
		if (Health <= 0)
			Health = 0;

		return true;
	}
	
}
