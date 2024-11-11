﻿using System.Collections.Generic;
using Analytics.Adapter;
using Cysharp.Threading.Tasks;

namespace Analytics.Adapter
{
    public interface IAnalyticsAdapter
    {
        UniTask Init();
        void SendEvent(string eventName, Dictionary<string, object> parameters);
        void AdRevenue(AnalyticsAdRevenue analyticsAdRevenue);

    }
}