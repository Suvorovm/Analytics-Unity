/*using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FreeplaySDK;
using Game.Analytics;
using Game.Level.Model;
using UnityEngine;

namespace Core.Analytics.Adapter
{
    public class FreePlayAnalyticsAdapter : IAnalyticsAdapter
    {
        private bool _freePlaySdkInited;
        private static GameObject _freePlayObject;

        public async UniTask Init()
        {
            if (_freePlayObject != null)
            {
                return;
            }

            GameObject freePlayPrefab = Resources.Load<GameObject>("Prefab/FreePaySDK");
            Freeplay.OnInitialized += OnFreeplaySdkInitialized;
            _freePlayObject = GameObject.Instantiate(freePlayPrefab);
            GameObject.DontDestroyOnLoad(_freePlayObject);
            await UniTask.WaitUntil(() => _freePlaySdkInited);
        }

        private void OnFreeplaySdkInitialized()
        {
            _freePlaySdkInited = true;
        }

        public void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            switch (eventName)
            {
                case AnalyticsEventName.LEVEL_START:
                    MapLevelStart(parameters);
                    break;
                case AnalyticsEventName.LEVEL_FINISH:
                    MapLevelFinish(parameters);
                    break;
            }
        }

        private void MapLevelFinish(Dictionary<string,object> parameters)
        {
            int levelParam = int.Parse(parameters[AnalyticsParametersName.LEVEL_ID].ToString());
            string levelFullId= parameters[AnalyticsParametersName.LEVEL_FULL_ID].ToString();
            LevelResult levelResult = Enum.Parse<LevelResult>(parameters[AnalyticsParametersName.LEVEL_FINISH_RESULT].ToString());

            if (levelResult == LevelResult.Exit)
            {
                return;
            }
            
            CommonEvents.NotifyLevelCompleted(levelParam, levelFullId, levelResult == LevelResult.Win);
        }

        private void MapLevelStart(Dictionary<string, object> parameters)
        {
            int levelParam = int.Parse(parameters[AnalyticsParametersName.LEVEL_ID].ToString());
            string levelFullId= parameters[AnalyticsParametersName.LEVEL_FULL_ID].ToString();
            
            CommonEvents.NotifyLevelStart(levelParam, levelFullId);
        }
    }
}*/