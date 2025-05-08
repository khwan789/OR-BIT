#if UNITY_ANDROID
using GooglePlayGames;
using Google.Play.AppUpdate;
using System.Collections;
#endif
using UnityEngine;

public class GoogleObserver : SocialObserverBase
{
#if UNITY_ANDROID
	private AppUpdateManager _appUpdateManager;

	public GoogleObserver()
	{
		SignIn();
	}

	protected sealed override void SignIn()
	{
		base.SignIn();

		if (Application.platform != RuntimePlatform.Android)
			return;

		PlayGamesPlatform.Activate();
		_appUpdateManager = new();

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

	public override void SetLeaderboardScore(int score)
	{
		base.SetLeaderboardScore(score);

		if (Application.platform != RuntimePlatform.Android)
			return;

		Social.ReportScore(score, GPGSIds.leaderboard_highscore, success => { Debug.Log(success ? "Score reported successfully" : "Unable to report score"); });
	}

	public override void ShowLeaderboard()
	{
		base.ShowLeaderboard();

		if (Application.platform != RuntimePlatform.Android)
			return;

		if (Social.localUser.authenticated == false)
		{
			Debug.Log("User not authenticated");
			SignInWithGoogle();
			return;
		}

		Social.ShowLeaderboardUI();
	}

	public override IEnumerator CheckUpdateCoroutine()
	{
		yield return base.CheckUpdateCoroutine();

		if (Application.platform != RuntimePlatform.Android)
			yield break;

		var appUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();

		yield return appUpdateInfoOperation;

		if (appUpdateInfoOperation.IsSuccessful)
		{
			var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

			if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
			{
				var appUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfoResult, AppUpdateOptions.ImmediateAppUpdateOptions());
			}
		}
	}
#endif
}