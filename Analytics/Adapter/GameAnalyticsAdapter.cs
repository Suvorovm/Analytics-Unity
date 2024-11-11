using System.Collections.Generic;
using Cysharp.Threading.Tasks;
#if !UNITY_EDITOR
using GameAnalyticsSDK;
using UnityEngine;
#endif


namespace Analytics.Adapter
{
    public class GameAnalyticsAdapter : IAnalyticsAdapter
    {
        public UniTask Init()
        {
#if !UNITY_EDITOR
            GameAnalytics gameAnalytics = GameObject.FindObjectOfType<GameAnalytics>();
            if (gameAnalytics == null)
            {
                GameObject load = Resources.Load<GameObject>("Prefab/Analytics/pfGameAnalytics");
                GameObject.Instantiate(load);
            }
            GameAnalytics.Initialize();
#endif
            return UniTask.CompletedTask;
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
#if !UNITY_EDITOR

            string eventId = eventName;

            foreach (var param in parameters)
            {
                eventId += $":{param.Key}_{param.Value ?? ""}";
            }
            GameAnalytics.NewDesignEvent(eventId);
#endif
        }

        public void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue)
        {
#if !UNITY_EDITOR
            GAAdType adType = GAAdType.Undefined;
            switch (analyticsAdRevenue.AdUnitFormat)
            {
                case "interstitial":
                    adType = GAAdType.Interstitial;
                    break;
                case "rewarded_video":
                    adType = GAAdType.RewardedVideo;
                    break;
                case "banner":
                    adType = GAAdType.Banner;
                    break;
            }

            GameAnalytics.NewAdEvent(GAAdAction.Show, adType, analyticsAdRevenue.AdSource, analyticsAdRevenue.AdPlacement);
#endif
        }
    }
}