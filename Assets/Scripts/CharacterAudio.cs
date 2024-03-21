using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	[SerializeField]
	private AudioClipInfo DodgeLeft;

	[SerializeField]
	public AudioClipInfo DodgeRight;

	[SerializeField]
	private AudioClipInfo H_Left;

	[SerializeField]
	private AudioClipInfo H_Right;

	[SerializeField]
	private AudioClipInfo StumbleSound;

	[SerializeField]
	private AudioClipInfo StumbleBushSound;

	[SerializeField]
	private AudioClipInfo StumbleSideSound;

	[SerializeField]
	private AudioClipInfo PowerupSound;

	[SerializeField]
	private AudioClipInfo PowerdownSound;

	[SerializeField]
	private AudioClipInfo NametagAttackSuccess;

	[SerializeField]
	private AudioClipInfo NametagAttackMiss;

	private Game game;

	private Character character;

	private AudioStateLoop audioStateLoop;

	private Dictionary<Character.StumbleType, AudioClipInfo> stumbleClips;

	public void Awake()
	{
		game = Game.Instance;
		game.OnNametag += HandleOnNametag;
		character = GetComponent<Character>();
		character.OnChangeTrack += HandleOnChangeTrack;
		character.OnStumble += HandleOnStumble;
		character.OnNametag += HandleOnNametag;
		stumbleClips = new Dictionary<Character.StumbleType, AudioClipInfo>();
		stumbleClips.Add(Character.StumbleType.Normal, StumbleSound);
		stumbleClips.Add(Character.StumbleType.Bush, StumbleBushSound);
		stumbleClips.Add(Character.StumbleType.Side, StumbleSideSound);
		audioStateLoop = UtilRMan.FindObject<AudioStateLoop>();
	}

	private void HandleOnNametag(Character.OnNametagAction action)
	{
		if (action == Character.OnNametagAction.Success)
		{
			So.Instance.playSound(NametagAttackSuccess);
		}
		else
		{
			So.Instance.playSound(NametagAttackMiss);
		}
	}

	private void HandleOnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (direction == Character.OnChangeTrackDirection.Left)
		{
			if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
			{
				So.Instance.playSound(H_Left);
			}
			else
			{
				So.Instance.playSound(DodgeLeft);
			}
		}
		else if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
		{
			So.Instance.playSound(H_Right);
		}
		else
		{
			So.Instance.playSound(DodgeRight);
		}
	}

	private void HandleOnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		So.Instance.playSound(stumbleClips[stumbleType]);
	}

	private void JetpackOnStart(bool isHeadStart)
	{
		if (!isHeadStart)
		{
			So.Instance.playSound(PowerupSound);
		}
		audioStateLoop.ChangeLoop(AudioState.Jetpack);
	}

	private void JetpackOnStop()
	{
		audioStateLoop.ChangeLoop(AudioState.JetpackStop);
		So.Instance.playSound(PowerdownSound);
	}
}
