using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Io.AppMetrica;
using UnityEngine;
using System;

namespace Analytics.Adapter
{
    public class AppMetricAdapter : IAnalyticsAdapter
    {
        private const string APP_METRIC_PLAYER_PREFS_KEY = "appMetrickFirstTime";
        private const string PURCHASE_CUSTOM_EVENT = "purchase_in_app";

        private readonly string _apiKey;

        public AppMetricAdapter(string apiKey)
        {
            _apiKey = apiKey;
        }

        private static bool IsFirstLaunch()
        {
            if (!PlayerPrefs.HasKey(APP_METRIC_PLAYER_PREFS_KEY))
            {
                return true;
            }

            PlayerPrefs.SetString(APP_METRIC_PLAYER_PREFS_KEY, "app");
            return false;
        }

        public async UniTask Init()
        {
            AppMetricaConfig appMetricaConfig = new AppMetricaConfig(_apiKey)
            {
                FirstActivationAsUpdate = !IsFirstLaunch(),
            };
            AppMetrica.Activate(appMetricaConfig);
            await UniTask.CompletedTask;
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            string json = ConvertDictionaryToJson(parameters);
            AppMetrica.ReportEvent(eventName, json);
            AppMetrica.SendEventsBuffer();
        }

        public void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue)
        {
            AdRevenue adRevenue = new AdRevenue(analyticsAdRevenue.AdRevenueValue, "USD");
            switch (analyticsAdRevenue.AdUnitFormat)
            {
                case "interstitial":
                    adRevenue.AdType = AdType.Interstitial;
                    break;
                case "rewarded_video":
                    adRevenue.AdType = AdType.Rewarded;
                    break;
                case "banner":
                    adRevenue.AdType = AdType.Banner;
                    break;
                default:
                    adRevenue.AdType = AdType.Other;
                    break;
            }

            adRevenue.AdNetwork = analyticsAdRevenue.AdSource;
            adRevenue.AdPlacementId = analyticsAdRevenue.AdPlacement;
            adRevenue.AdUnitId = analyticsAdRevenue.AdUnitInstance;
            adRevenue.AdPlacementName = analyticsAdRevenue.AdPlacement;
            adRevenue.Precision = analyticsAdRevenue.AdPrecision;
            Dictionary<string, string> payload = new Dictionary<string, string>()
            {
                { "ad_lifetime_revenue", analyticsAdRevenue.LifetimeRevenue.ToString() },
                { "ad_country", analyticsAdRevenue.AdCountry.ToString() }
            };
            adRevenue.Payload = payload;
            AppMetrica.ReportAdRevenue(adRevenue);
        }

        public void SendPurchaseEvent(decimal localizedPrice, string icoCurrency, string productType, string productId, string receiptRaw)
        {
            Revenue revenue = new Revenue((long)(localizedPrice * 1000000), icoCurrency)
            {
                ProductID = productId,
            };

            if (!string.IsNullOrEmpty(receiptRaw))
            {
                try
                {
                    Receipt parsedReceipt = JsonUtility.FromJson<Receipt>(receiptRaw);
                    Revenue.Receipt receiptObject = new Revenue.Receipt();

#if UNITY_ANDROID
                    PayloadAndroid payloadAndroid = JsonUtility.FromJson<PayloadAndroid>(parsedReceipt.Payload);
                    receiptObject.Data = payloadAndroid.Json;
                    receiptObject.Signature = payloadAndroid.Signature;
#elif UNITY_IPHONE
            receiptObject.Data = parsedReceipt.Payload;
            receiptObject.TransactionID = parsedReceipt.TransactionID;
#endif

                    revenue.ReceiptValue = receiptObject;

                    // Добавим дополнительную инфу в payload
                    revenue.Payload = ConvertDictionaryToJson(new Dictionary<string, object>()
                    {
                        { "productType", productType },
                        { "receipt", receiptRaw }
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AppMetrica] Ошибка парсинга чека: {ex.Message}");
                }
            }

            // Отправка дохода
            AppMetrica.ReportRevenue(revenue);

            // Отправка кастомного ивента
            SendEvent(PURCHASE_CUSTOM_EVENT, new Dictionary<string, object>()
            {
                { "productId",  productId },
                { "productType", productType },
                { "localPrice", localizedPrice },
                { "icoCurrency", icoCurrency }
            });
        }



        private string ConvertDictionaryToJson(Dictionary<string, object> parameters)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\n");

            foreach (var param in parameters)
            {
                jsonBuilder.AppendFormat("    \"{0}\": \"{1}\",\n", param.Key, param.Value);
            }

            // Удаляем последнюю запятую и добавляем закрывающую скобку
            if (parameters.Count > 0)
            {
                jsonBuilder.Length -= 2; // Убираем последнюю запятую и символ новой строки
            }

            jsonBuilder.Append("\n}");

            return jsonBuilder.ToString();
        }
        
        [System.Serializable]
        public struct Receipt {
            public string Store;
            public string TransactionID;
            public string Payload;
        }

// Additional information about the IAP for Android.
        [System.Serializable]
        public struct PayloadAndroid {
            public string Json;
            public string Signature;
        }
    }
}