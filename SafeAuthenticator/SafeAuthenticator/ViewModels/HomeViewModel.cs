﻿using System;
using System.Linq;
using System.Windows.Input;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class HomeViewModel : BaseViewModel {
    private string _accountStorageInfo;
    private bool _isRefreshing;
    public ICommand LogoutCommand { get; }
    public ObservableRangeCollection<RegisteredApp> Apps { get; set; }
    public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }
    public ICommand RefreshAccountsCommand { get; }
    public ICommand AccountSelectedCommand { get; }
    public string AccountStorageInfo { get => _accountStorageInfo; set => SetProperty(ref _accountStorageInfo, value); }

    public HomeViewModel() {
      IsRefreshing = false;
      Apps = new ObservableRangeCollection<RegisteredApp>();
      RefreshAccountsCommand = new Command(OnRefreshAccounts);
      AccountSelectedCommand = new Command<RegisteredApp>(OnAccountSelected);
      LogoutCommand = new Command(OnLogout);

      Device.BeginInvokeOnMainThread(OnRefreshAccounts);
    }

    private void OnAccountSelected(RegisteredApp appInfo) {
      MessagingCenter.Send(this, MessengerConstants.NavAppInfoPage, appInfo);
    }

    private async void OnLogout() {
      await Authenticator.LogoutAsync();
      MessagingCenter.Send(this, MessengerConstants.NavLoginPage);
    }

    private async void OnRefreshAccounts() {
      try {
        IsRefreshing = true;
        var registeredApps = await Authenticator.GetRegisteredAppsAsync();
        Apps.AddRange(registeredApps.Except(Apps));
        Apps.Sort();
        var acctStorageTuple = await Authenticator.GetAccountInfoAsync();
        AccountStorageInfo = $"{acctStorageTuple.Item1} / {acctStorageTuple.Item2}";
        IsRefreshing = false;
      } catch (Exception ex) {
        await Application.Current.MainPage.DisplayAlert("Error", $"Refresh Accounts Failed: {ex.Message}", "OK");
      }
    }
  }
}
