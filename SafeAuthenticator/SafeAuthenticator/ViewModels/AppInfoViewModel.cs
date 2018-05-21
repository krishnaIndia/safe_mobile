using System;
using System.Windows.Input;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class AppInfoViewModel : BaseViewModel {
    private RegisteredAppModel _appModelInfo;

    [PublicAPI]
    public RegisteredAppModel AppModelInfo { get => _appModelInfo; set => SetProperty(ref _appModelInfo, value); }

    [PublicAPI]
    public ICommand RevokeAppCommand { get; }

    public AppInfoViewModel(RegisteredAppModel appModelInfo) {
      AppModelInfo = appModelInfo;
      RevokeAppCommand = new Command(OnRevokeAppCommand);
    }

    private async void OnRevokeAppCommand() {
      try {
        using (Acr.UserDialogs.UserDialogs.Instance.Loading("Revoking App Access")) {
          await Authenticator.RevokeAppAccessAsync(_appModelInfo.AppId);
                }
        MessagingCenter.Send(this, MessengerConstants.NavHomePage);
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Refresh Accounts Failed: {ex.Message}", "OK");
      }
    }
  }
}
