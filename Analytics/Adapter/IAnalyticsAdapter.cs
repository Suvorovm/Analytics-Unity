using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Analytics.Adapter
{
    public interface IAnalyticsAdapter : IDisposable
    {
        UniTask Init();
        void SendEvent(string eventName, Dictionary<string, object> parameters);
        void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue);

        void SendPurchaseEvent(decimal localizedPrice, string icoCurrency, string productType, string productId, string receipt);

    }
}