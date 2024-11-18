using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Analytics.Privacy
{
    public class PrivacyService : IPrivacyService
    {
        private const string PERMISSION_RECEIVED = "privicyPermissionReceived";

        private readonly string _dialogPath;
        private readonly string _privacyLink;

        public PrivacyService(string dialogPath, string privacyLink)
        {
            _dialogPath = dialogPath;
            _privacyLink = privacyLink;
        }

        public bool NeedShowPrivacyDialog()
        {
            if (PlayerPrefs.HasKey(PERMISSION_RECEIVED))
            {
                return false;
            }

            return true;
        }

        public async UniTask ShowPrivacyPolicy()
        {
            GameObject privacyPrefab = Resources.Load<GameObject>(_dialogPath);
            GameObject go = Object.Instantiate(privacyPrefab);
            IPrivacyView privacyView = go.GetComponent<IPrivacyView>(); 
            privacyView.SetUpInfo(_privacyLink);
            await privacyView.WaitContinueClick();
            SaveReceivedPermission();
            Object.Destroy(go);
        }

        private void SaveReceivedPermission()
        {
            PlayerPrefs.SetString(PERMISSION_RECEIVED, PERMISSION_RECEIVED);
            PlayerPrefs.Save();
        }
    }
}