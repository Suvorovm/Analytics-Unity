using Cysharp.Threading.Tasks;

namespace Analytics.Privacy
{
    public interface IPrivacyView
    {
        UniTask WaitContinueClick();
        void SetUpInfo(string privacyLink);
    }
}