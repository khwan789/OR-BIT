using UnityEngine;

public abstract class SocialObserverBase
{
	public static SocialObserverBase Get => GameManager.Instance.socialObserver;

	protected virtual void SignIn()
	{
	}
	
	public virtual void SetLeaderboardScore(int score)
	{
	}

	public virtual void Update()
	{
	}
	
	public virtual void ShowLeaderboard()
	{
	}

	private void Start()
	{
#if UNITY_EDITOR
		return;
#endif

		SignIn();
	}
}