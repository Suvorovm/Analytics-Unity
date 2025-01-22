using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;

#if !UNITY_EDITOR
using Firebase;
using Firebase.Analytics;
using UnityEngine;
#endif

namespace Analytics.Adapter
{
    public class FireBaseAnalyticsAdapter : IAnalyticsAdapter
    {
        private const string AD_ADDITIONAL_INFO_EVENT_NAME = "ad_iron_source_additional_info";
        private const string AD_SOURCE = "ad_source";
        private const string AD_UNIT_NAME = "ad_unit_name";
        private const string AD_FORMAT = "ad_unit_format";
        private const string AD_PLACEMENT = "ad_placement";
        private const string AD_REVENUE = "revenue";
        private const string AD_COUNTRY = "ad_country";
        private const string AD_PRECISION = "ad_precision";
        private const string AD_LIFETIME_REVENUE = "ad_liftime_revenue";
        private const string AD_VALUE = "value";
        
        
        private bool _inited;
        private bool _anyAnswerFromFireBase;

        public async UniTask Init()
        {
#if !UNITY_EDITOR
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;
                _inited = true;
                Debug.Log(System.String.Format(
                    "All ok", dependencyStatus));
                // Set a flag here to indicate that your project is ready to use Firebase.
            }
            else
            {
                Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
#endif
            await UniTask.CompletedTask;
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (!_inited)
            {
#if !UNITY_EDITOR
                Debug.LogWarning("Firebase Not Inited");
#endif
                return;
            }

            List<Parameter> fireBaseParams = new List<Parameter>();
            foreach (var (key, value) in parameters)
            {
                var parameter = CreateParam(key, value);
                fireBaseParams.Add(parameter);
            }

            FirebaseAnalytics.LogEvent(eventName, fireBaseParams.ToArray());
        }

        public void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue)
        {
            List<Parameter> fireBaseParams = new List<Parameter>()
            {
                new Parameter(AD_SOURCE, analyticsAdRevenue.AdSource),
                new Parameter(AD_UNIT_NAME, analyticsAdRevenue.AdUnitFormat),
                new Parameter(AD_FORMAT, analyticsAdRevenue.AdUnitInstance),
                new Parameter(AD_PLACEMENT, analyticsAdRevenue.AdPlacement),
                new Parameter(AD_COUNTRY, analyticsAdRevenue.AdCountry),
                new Parameter(AD_PRECISION, analyticsAdRevenue.AdPrecision),
                new Parameter(AD_LIFETIME_REVENUE, analyticsAdRevenue.LifetimeRevenue),
                new Parameter(AD_VALUE, analyticsAdRevenue.AdRevenueValue),
                new Parameter(AD_REVENUE, analyticsAdRevenue.AdRevenueValue),
                new Parameter("currency", "USD"), //required
                
            };
            FirebaseAnalytics.LogEvent(AD_ADDITIONAL_INFO_EVENT_NAME, fireBaseParams.ToArray());
        }

        public void SendPurchaseEvent(decimal localizedPrice, string icoCurrency, string productType, string productId,
            string receipt)
        {
            FirebaseAnalytics.LogEvent(
                FirebaseAnalytics.EventPurchase,
                new Parameter[]
                {
                    new Parameter(FirebaseAnalytics.ParameterItemId, productId), 
                    new Parameter(FirebaseAnalytics.ParameterItemName, productId), 
                    new Parameter(FirebaseAnalytics.ParameterCurrency, icoCurrency), 
                    new Parameter(FirebaseAnalytics.ParameterValue, (double) localizedPrice), 
                });
        }

        private Parameter CreateParam(string paramName, object param)
        {
            if (param == null)
            {
                return new Parameter(paramName, "");
            }

            if (param is double digit)
            {
                return new Parameter(paramName, digit);
            }

            if (param is int number)
            {
                return new Parameter(paramName, number);
            }

            return new Parameter(paramName, param.ToString());
        }
    }
}