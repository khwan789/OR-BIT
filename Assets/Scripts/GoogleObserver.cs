using GooglePlayGames;
using UnityEngine;

public class GoogleObserver : MonoBehaviour
{
    private void Start()
    {
        PlayGamesPlatform.Activate();
        SignInWithGoogle();
    }

    private void SignInWithGoogle()
    {
        Social.localUser.Authenticate(success =>
        {
	        
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