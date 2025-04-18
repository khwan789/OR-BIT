using GooglePlayGames;
using System.Collections;
using Google.Play.AppUpdate;
using UnityEngine;

public class GoogleObserver : MonoBehaviour
{
	public static GoogleObserver Get => GameManager.Instance.googleObserver;
	private AppUpdateManager _appUpdateManager;

    private void Start()
    {
	    if (Application.platform != RuntimePlatform.Android)
		    return;
	    
        PlayGamesPlatform.Activate();
	    _appUpdateManager = new();
	    
        SignInWithGoogle();
        CheckForUpdate();
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
		Social.ReportScore(score, GPGSIds.leaderboard_highscore, success =>
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

	public void ShowLeaderboard()
	{
		if (Social.localUser.authenticated == false)
		{
			Debug.Log("User not authenticated");
			SignInWithGoogle();
			return;
		}
		
		Social.ShowLeaderboardUI();
	}

	private void CheckForUpdate() => StartCoroutine(CheckForUpdateRoutine());
	
	private IEnumerator CheckForUpdateRoutine()
     {
         var appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();

         yield return appUpdateInfoOperation;

         if (appUpdateInfoOperation.IsSuccessful)
         {
             var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
         }
     }

}