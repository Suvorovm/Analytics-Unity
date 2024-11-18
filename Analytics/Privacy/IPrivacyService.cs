using Cysharp.Threading.Tasks;

namespace Analytics.Privacy
{
    public interface IPrivacyService
    {
        bool NeedShowPrivacyDialog();
        UniTask ShowPrivacyPolicy();
    }
}