using UnityEngine;
#if UNITY_IOS
using System;
using Apple.GameKit;
#endif

public class IOSObserver : SocialObserverBase
{
#if UNITY_IOS
	protected override async void SignIn()
	{
		if (GKLocalPlayer.Local.IsAuthenticated == false)
		{
			// Perform the authentication.
			var player = await GKLocalPlayer.Authenticate();
			Debug.Log($"GameKit Authentication: player {player}");

			// Grab the display name.
			var localPlayer = GKLocalPlayer.Local;
			Debug.Log($"Local Player: {localPlayer.DisplayName}");

			// Fetch the items.
			var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();

			var signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
			var teamPlayerID = localPlayer.TeamPlayerId;
			Debug.Log($"Team Player ID: {teamPlayerID}");

			Debug.Log($"GameKit Authentication: signature => {signature}");
			Debug.Log($"GameKit Authentication: publickeyurl => {fetchItemsResponse.PublicKeyUrl}");
			Debug.Log($"GameKit Authentication: salt => {Convert.ToBase64String(fetchItemsResponse.GetSalt())}");
			Debug.Log($"GameKit Authentication: Timestamp => {fetchItemsResponse.Timestamp.ToString()}");
		}
		else
		{
			Debug.Log("AppleGameCenter player already logged in.");
		}
	}

	public override void SetLeaderboardScore(int score)
	{
		base.SetLeaderboardScore(score);
		
		
	}

	public override async void ShowLeaderboard()
	{
		base.ShowLeaderboard();
		
		try
		{
			var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Leaderboards);
			await gameCenter.Present();
		}
		catch (GameKitException ex)
		{
			Debug.LogError($"GameKitException while presenting leaderboard: {ex.Message}");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Unexpected error while presenting leaderboard: {ex.Message}");
		}
	}
#endif
}