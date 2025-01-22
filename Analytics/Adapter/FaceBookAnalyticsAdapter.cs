using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;

namespace Analytics.Adapter
{
    public class FaceBookAnalyticsAdapter : IAnalyticsAdapter
    {
        private bool _inited;
        private bool _isFaceBookGetAnyAnswer;

        public async UniTask Init()
        {
#if !UNITY_EDITOR
            if (!FB.IsInitialized)
            {
                Debug.Log("!FB.IsInitialized");

                FB.Init(InitCallback, OnHideUnity);
                await UniTask.WaitUntil(() => _isFaceBookGetAnyAnswer);  
            }
            else
            {
                FB.ActivateApp();
                Debug.LogWarning("FaceBookAnalyticsAdapter");
                _inited = true;
                await UniTask.CompletedTask;
            }
#endif
        }

        private void OnHideUnity(bool isunityshown)
        {
        }

        private void InitCallback()
        {
            _isFaceBookGetAnyAnswer = true;
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                _inited = true;
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!_inited)
            {
#if !UNITY_EDITOR
                Debug.LogWarning("Face book Not Inited");
#endif
                return;
            }

            FB.LogAppEvent(
                eventName,
                null, parameters);
        }

        public void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue)
        {
        }

        public void SendPurchaseEvent(decimal localizedPrice, string icoCurrency, string productType, string productId,
            string receipt)
        {
            if (!_inited)
            {
#if !UNITY_EDITOR
                Debug.LogWarning("Face book Not Inited");
#endif
                return;
            }
            FB.LogPurchase(localizedPrice, icoCurrency);
        }
    }
}