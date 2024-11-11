using System;
using System.Collections.Generic;
using Analytics.Adapter;
using Analytics.Privacy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Analytics
{
    public class AnalyticsFacade
    {
        private readonly List<IAnalyticsAdapter> _analyticsAdapters;
        private readonly PrivacyService _privacyService;

        public AnalyticsFacade(List<IAnalyticsAdapter> analyticsAdapters, PrivacyService privacyService)
        {
            _privacyService = privacyService;
            _analyticsAdapters = analyticsAdapters;
        }

        public async UniTask Init()
        {
            if (_privacyService.NeedShowPrivacyDialog())
            {
                await _privacyService.ShowPrivacyPolicy();
            }

            List<UniTask> analyticsAwait = new List<UniTask>();
            foreach (var analyticsAdapter in _analyticsAdapters)
            {
                try
                {
                    UniTask uniTask = analyticsAdapter.Init();
                    analyticsAwait.Add(uniTask);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Analytics init failed {e}");
                }
            }

            await UniTask.WhenAll(analyticsAwait);
        }

        public void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue)
        {
            foreach (IAnalyticsAdapter analyticsAdapter in _analyticsAdapters)
            {
                analyticsAdapter.AdRevenue(analyticsAdRevenue);
            }
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            foreach (IAnalyticsAdapter analyticsAdapter in _analyticsAdapters)
            {
                analyticsAdapter.SendEvent(eventName, parameters);
            }
#if UNITY_EDITOR
            PrintEvent(eventName, parameters);
#endif
        }

        private void PrintEvent(string eventName, Dictionary<string, object> parameters)
        {
            string collectedParamsToString = $"event Name {eventName}\n";
            foreach ((string key, object value) in parameters)
            {
                collectedParamsToString += $"param name : {key} value {value}\n";
            }

            Debug.Log($"Params : {collectedParamsToString}");
        }
    }
}