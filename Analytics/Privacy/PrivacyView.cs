using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Analytics.Privacy
{
    public class PrivacyView : MonoBehaviour, IPrivacyView
    {
        [SerializeField]
        private Button _continueButton;

        [SerializeField]
        private Button _privacyButton;

        private string _link;

        private UniTaskCompletionSource _buttonPressedTask;

        private void Awake()
        {
            _continueButton.onClick.AddListener(OnContinueButtonClick);
            _privacyButton.onClick.AddListener(OnPrivacyButtonClick);
        }

        private void OnPrivacyButtonClick()
        {
            Application.OpenURL(_link);
        }

        private void OnContinueButtonClick()
        {
            _buttonPressedTask?.TrySetResult();
        }

        public async UniTask WaitContinueClick()
        {
            _buttonPressedTask = new UniTaskCompletionSource();
            await _buttonPressedTask.Task;
        }

        public void SetUpInfo(string privacyLink)
        {
            _link = privacyLink;
        }
    }
}