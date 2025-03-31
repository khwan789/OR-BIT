using GooglePlayGames;
using UnityEngine;

public class GoogleObserver : MonoBehaviour
{
	public static GoogleObserver Get => GameManager.Instance.googleObserver;
	
    private void Start()
    {
        PlayGamesPlatform.Activate();
        SignInWithGoogle();
    }

    private void SignInWithGoogle()
    {
        Social.localUser.Authenticate(isSuccess =>
        {
	        if (isSuccess)
	        {
		        Debug.Log("Google Play Games sign-in successful");
	        }
	        else
	        {
		        Debug.Log("Google Play Games sign-in failed");
	        }
        });
    }
    
    public void SetLeaderboardScore(int score)
	{
		Social.ReportScore(score, GPGSIds.leaderboard_high_score, success =>
		{
			if (success)
			{
				Debug.Log("Score reported successfully");
			}
			else
			{
				Debug.Log("Unable to report score");
			}
		});
	}
}