using System;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.Events;

public class AdObserver : MonoBehaviour
{
	public static AdObserver Get => GameManager.Instance.adObserver;

#if UNITY_ANDROID
	private const string _appKey = "218e22805";
#elif UNITY_IPHONE
	private const string _appKey = "21c1fa3dd";
#endif

	private bool _isEligibleForReward;
	private UnityAction _onAdSuccess;

	private void Start()
	{
		IronSource.Agent.setConsent(true);
		IronSource.Agent.setMetaData("do_not_sell", "false");
		IronSource.Agent.setMetaData("is_child_directed", "false");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
		IronSource.Agent.setMetaData("is_test_suite", "enable");
#endif
		IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
		IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
		IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
		IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
		IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
		IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
		IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

		IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

		IronSource.Agent.init(_appKey, IronSourceAdUnits.INTERSTITIAL);
	}

	private void SdkInitializationCompletedEvent()
	{
		Debug.Log("IronSource SDK initialized successfully.");
	}

	public void ShowAd()
	{
		var available = IronSource.Agent.isInterstitialReady();
		Debug.Log("ShowAdStart");

		if (available)
		{
			Debug.Log("ShowAd");
			IronSource.Agent.showInterstitial("Game_Over");
		}
		else
		{
			Debug.Log("ShowAdFailed");
			IronSource.Agent.loadInterstitial();
		}
	}

	public void ShowRewardAd(UnityAction onAdSuccess)
	{
		var available = IronSource.Agent.isRewardedVideoAvailable();
		Debug.Log(available);

		if (available)
		{
			_onAdSuccess = onAdSuccess;
			IronSource.Agent.showRewardedVideo("BonusReward");
		}
		else
		{
			IronSource.Agent.loadRewardedVideo();
		}
	}

	void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}

	void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
	{
		Debug.Log("RewardedVideoOnAdAvailable");
	}

	void RewardedVideoOnAdUnavailable()
	{
		Debug.Log("RewardedVideoOnAdUnavailable");
	}

	void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
	{
		Debug.Log("RewardedVideoOnAdOpenedEvent");
	}

	void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
	{
		if (_isEligibleForReward == false)
		{
			_onAdSuccess = null;
			return;
		}

		_isEligibleForReward = false;

		_onAdSuccess?.Invoke();
		_onAdSuccess = null;

		Debug.Log("RewardedVideoOnAdClosedEvent");
	}

	void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
	{
		_isEligibleForReward = true;
		if (placement != null)
			Debug.Log($"{placement.getRewardName()} {placement.getRewardAmount()}");

		Debug.Log("RewardedVideoOnAdRewardedEvent");
	}

	void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
	{
		Debug.LogError(error.getErrorCode());
	}

	void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
	{
		Debug.Log("RewardedVideoOnAdClickedEvent");
	}
}