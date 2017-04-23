using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PersonAnimator : MonoBehaviour
{
	private Person mPerson;
	private Rigidbody2D mBody;

	[SerializeField]
	private SpriteRenderer mCharacterHead;
	[SerializeField]
	private SpriteRenderer mCharacterTorso;
	[SerializeField]
	private SpriteRenderer mCharacterRightArm;
	[SerializeField]
	private SpriteRenderer mCharacterLeftArm;
	[SerializeField]
	private SpriteRenderer mCharacterRightLeg;
	[SerializeField]
	private SpriteRenderer mCharacterLeftLeg;
	[SerializeField]
	private SpriteRenderer mTool;

	private float animTrack;


	class Frame
	{
		public Quaternion Rotation;
		public float Duration;
	}

	class Anim
	{
		public List<Frame> Frames { get; private set; }
		private float TotalDuration;

		public Anim(List<Frame> Frames)
		{
			this.Frames = Frames;

			TotalDuration = 0.0f;
			foreach (Frame frame in Frames)
				TotalDuration += frame.Duration;
        }

		public Quaternion Get(float time)
		{
			float animTime = time % TotalDuration;
			
			Frame f0 = null, f1 = null;

			foreach (Frame frame in Frames)
			{
				if (f0 != null)
				{
					f1 = frame;
					break;
				}

				if (animTime < frame.Duration)
				{
					if (f0 == null)
						f0 = frame;
				}

				animTime -= frame.Duration;
            }

			//Looping
			if (f1 == null)
				f1 = Frames[0];

			float v = 1.0f + (animTime / f0.Duration);
			return Quaternion.Slerp(f0.Rotation, f1.Rotation, v);
		}
	}

	class FullAnim
	{
		private Dictionary<uint, Anim> Animation;
		private SpriteRenderer[] Joints;

		public FullAnim(Dictionary<uint, Anim> Animation, SpriteRenderer[] Joints)
		{
			this.Animation = Animation;
			this.Joints = Joints;
        }

		public void Animate(float time, bool smooth, bool flip)
		{
			for (uint i = 0; i < Joints.Length; ++i)
			{
				if (Animation.ContainsKey(i))
				{
					Quaternion desiredState = Animation[i].Get(time);

					if (flip)
					{
						Vector3 eular = desiredState.eulerAngles;
                        desiredState = Quaternion.Euler(eular.x, eular.y, -eular.z);
					}

					if (smooth)
						Joints[i].transform.rotation = Quaternion.Slerp(Joints[i].transform.rotation, desiredState, 0.1f);
					else
						Joints[i].transform.rotation = desiredState;
					
                }
			}
		}
	}


	private FullAnim IdleAnim;
	private FullAnim WalkingAnim;
	private FullAnim AirAnim;
	private FullAnim SwingAnim;

	public delegate void FinishedSwing();
	private float SwingTimer;
	private float SwingTotalTime;
	private FinishedSwing OnFinishSwing;


	void Start ()
	{
		mPerson = GetComponentInParent<Person>();
		mBody = GetComponentInParent<Rigidbody2D>();

		SpriteRenderer[] Joints = new SpriteRenderer[7];
		Joints[0] = mCharacterHead;
		Joints[1] = mCharacterTorso;
		Joints[2] = mCharacterRightArm;
		Joints[3] = mCharacterLeftArm;
		Joints[4] = mCharacterRightLeg;
		Joints[5] = mCharacterLeftLeg;
		Joints[6] = mTool;

		BuildIdleAnimation(Joints);
		BuildWalkingAnimation(Joints);
		BuildAirAnimation(Joints);
		BuildSwingAnimation(Joints);
	}

	const uint CHAR_HEAD = 0;
	const uint CHAR_TORSO = 1;
	const uint CHAR_RIGHT_ARM = 2;
	const uint CHAR_LEFT_ARM = 3;
	const uint CHAR_RIGHT_LEG = 4;
	const uint CHAR_LEFT_LEG = 5;
	const uint CHAR_TOOL = 6;


	void BuildIdleAnimation(SpriteRenderer[] Joints)
	{
        Dictionary<uint, Anim> Animation = new Dictionary<uint, Anim>();

		//Head bob
		{
			const uint index = CHAR_HEAD;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.AngleAxis(5, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.AngleAxis(-5, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Torso bob
		{
			const uint index = CHAR_TORSO;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 3;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.AngleAxis(5, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 3;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 5;
				frame.Rotation = Quaternion.AngleAxis(-5, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//LArm bob
		{
			const uint index = CHAR_LEFT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 2;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.AngleAxis(-5, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 2;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.AngleAxis(5, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//RArm bob
		{
			const uint index = CHAR_RIGHT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 2;
				frame.Rotation = Quaternion.AngleAxis(-5, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 2;
				frame.Rotation = Quaternion.AngleAxis(5, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//LLeg Idle
		{
			const uint index = CHAR_LEFT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 100;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//RLeg Idle
		{
			const uint index = CHAR_RIGHT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 100;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}


		IdleAnim = new FullAnim(Animation, Joints);
    }

	void BuildWalkingAnimation(SpriteRenderer[] Joints)
	{
		Dictionary<uint, Anim> Animation = new Dictionary<uint, Anim>();


		//Torso bob
		{
			const uint index = CHAR_TORSO;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.AngleAxis(5, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 1;
				frame.Rotation = Quaternion.AngleAxis(-5, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Leg move swing
		{
			const uint index = CHAR_LEFT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(75.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(80.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-65.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-70.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Leg move swing
		{
			const uint index = CHAR_RIGHT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-65.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-70.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(75.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(80.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Arm move swing
		{
			const uint index = CHAR_LEFT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-45.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-60.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(45.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(50.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Arm move swing
		{
			const uint index = CHAR_RIGHT_ARM;
			List<Frame> frames = new List<Frame>();
			
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(45.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(50.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.identity;
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-45.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(-60.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}


		WalkingAnim = new FullAnim(Animation, Joints);
	}

	void BuildAirAnimation(SpriteRenderer[] Joints)
	{
		Dictionary<uint, Anim> Animation = new Dictionary<uint, Anim>();

		//Torso Wiggle
		{
			const uint index = CHAR_TORSO;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(5.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(-5.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}
		
		//Arm Wiggle
		{
			const uint index = CHAR_LEFT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(20.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(-20.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Arm Wiggle
		{
			const uint index = CHAR_RIGHT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(-20.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(20.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Leg Wiggle
		{
			const uint index = CHAR_RIGHT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(20.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(-20.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		//Leg Wiggle
		{
			const uint index = CHAR_LEFT_LEG;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(-20.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(20.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}


		AirAnim = new FullAnim(Animation, Joints);
	}

	void BuildSwingAnimation(SpriteRenderer[] Joints)
	{
		Dictionary<uint, Anim> Animation = new Dictionary<uint, Anim>();

		//Swing
		{
			const uint index = CHAR_RIGHT_ARM;
			List<Frame> frames = new List<Frame>();

			{
				Frame frame = new Frame();
				frame.Duration = 0.6f;
				frame.Rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.1f;
				frame.Rotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.3f;
				frame.Rotation = Quaternion.AngleAxis(10.0f, Vector3.forward);
				frames.Add(frame);
			}
			{
				Frame frame = new Frame();
				frame.Duration = 0.0f;
				frame.Rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
				frames.Add(frame);
			}

			Anim anim = new Anim(frames);
			Animation[index] = anim;
		}

		SwingAnim = new FullAnim(Animation, Joints);
	}

	void Update ()
	{
		animTrack += Time.deltaTime;

		const float movementThreshold = 1.0f;


		//Moving
		if (!mPerson.TouchingGround)
			AirAnim.Animate(animTrack, true, transform.localScale.x != 1.0f);

        else if (Mathf.Abs(mBody.velocity.x) >= movementThreshold)
		{
			transform.localScale = new Vector2(Mathf.Sign(mBody.velocity.x), 1.0f);
			WalkingAnim.Animate(animTrack, true, transform.localScale.x != 1.0f);
        }

		//Idle
		else
			IdleAnim.Animate(animTrack, true, transform.localScale.x != 1.0f);


		if (SwingTotalTime != 0.0f)
		{
			SwingTimer += Time.deltaTime;
			SwingAnim.Animate(Mathf.Clamp(SwingTimer / SwingTotalTime, 0.0f, 1.0f), false, transform.localScale.x != 1.0f);

			if (SwingTimer >= SwingTotalTime)
			{
				SwingTotalTime = 0.0f;
				OnFinishSwing();
			}
        }
		

		SetToolSprite();
    }

	void SetToolSprite()
	{
		ItemSlot slot = mPerson.CurrentlyEquiped;

		if (slot == null || slot.ID == ItemID.None)
			mTool.enabled = false;
		else
		{
			ItemMeta meta = ItemController.Library[slot.ID];

			if (meta.TextureID != -1 && meta.Tool)
			{
				mTool.sprite = ItemController.Main.ItemSheet[meta.TextureID];
				mTool.enabled = true;
			}
			else
				mTool.enabled = false;
		}
	}
	

	public bool Swing(float time, FinishedSwing OnFinished)
	{
		if (SwingTotalTime != 0.0f)
			return false;

		if (time < 0.0f)
			time = 0.01f;

		SwingTotalTime = time;
		SwingTimer = 0.0f;
		OnFinishSwing = OnFinished;
		return true;
	}
}
