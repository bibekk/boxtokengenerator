﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Exceptions;

namespace BoxTokenGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        private string request;
        public string AuthCode { get; private set; }
        public OAuthSession session = null;
        public static string loggedin_user = string.Empty;
        public IBoxConfig Config { get; set; }
        public static BoxClient Client { get; set; }
        #endregion variables

        public MainWindow()
        {
            InitializeComponent();
            this.request = BoxManagerConst.authurl + "&client_id=" + BoxManagerConst.client_id + "&state=security_token%3DKnhMJatFipTAnM0nHlZA";
        }

        private async void main_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void Auth_SessionAuthenticated(object sender, SessionAuthenticatedEventArgs e) 
        {
            TokenConfig.writeTokens(e.Session.AccessToken, e.Session.RefreshToken);
        }

        private async void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.Host == BoxManagerConst.redirecturi)
            {

                // grab access_token and oauth_verifier
                IDictionary<string, string> keyDictionary = new Dictionary<string, string>();
                var qSplit = e.Uri.Query.Split('?');
                foreach (var kvp in qSplit[qSplit.Length - 1].Split('&'))
                {
                    var kvpSplit = kvp.Split('=');
                    if (kvpSplit.Length == 2)
                    {
                        keyDictionary.Add(kvpSplit[0], kvpSplit[1]);
                    }
                }

                AuthCode = keyDictionary["code"];
                webBrowser.Visibility = Visibility.Hidden;
                try
                {
                    session = await Client.Auth.AuthenticateAsync(AuthCode);
                    TokenConfig.writeTokens(session.AccessToken, session.RefreshToken);
                    Box.V2.Models.BoxUser u = await Client.UsersManager.GetCurrentUserInformationAsync();
                    loggedin_user = u.Login;
                    main.Visibility = Visibility.Visible;
                    txtAccessToken.Text = session.AccessToken;
                    txtRefreshToken.Text = session.RefreshToken;
                    mainWindow.Height = 220;
                }
                catch (BoxException ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCopyAccessToken_Click(object sender, RoutedEventArgs e)
        {
            if (txtAccessToken.Text != "")
            {
                Clipboard.SetText(txtAccessToken.Text.Trim());
                MessageBox.Show("Access Token Copied", "Copy Access Token to clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCopyRefreshToken_Click(object sender, RoutedEventArgs e)
        {
            if (txtRefreshToken.Text != "")
            {
                Clipboard.SetText(txtRefreshToken.Text.Trim());
                MessageBox.Show("Refresh Token Copied", "Copy Refresh Token to clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                session = MyOAuth.getSession();
                Config = new BoxConfig(BoxManagerConst.client_id, BoxManagerConst.client_secret, new Uri("https://" + BoxManagerConst.redirecturi));
                Client = new BoxClient(Config, session);
                Client.Auth.SessionAuthenticated += Auth_SessionAuthenticated;
                try
                {
                    Box.V2.Models.BoxUser u = await Client.UsersManager.GetCurrentUserInformationAsync();
                    if (u != null)
                    {
                        webBrowser.Visibility = Visibility.Hidden;
                        //BoxTokenGenerator.ActiveForm.Text += " - " + u.Login;
                        loggedin_user = u.Login;
                        //boxMenu.Visible = true;
                        main.Visibility = Visibility.Visible;
                        //txtToken.Text = session.AccessToken + "#" + session.RefreshToken;
                        txtAccessToken.Text = session.AccessToken;
                        txtRefreshToken.Text = session.RefreshToken;
                        lblLoggedInUser.Text = loggedin_user;
                        mainWindow.Height = 220;
                    }
                }

                catch (BoxException ex)
                {
                    webBrowser.Visibility = Visibility.Visible;
                    session = null;
                    webBrowser.Source = new Uri(this.request);
                    mainWindow.Height = 600;
                }
                //catch (Exception ex)
                //{
                //    if (ex.Message.Contains("BoxSessionInvalidated"))
                //    {
                //        webBrowser.Visibility = Visibility.Visible;
                //        main.Visibility = Visibility.Hidden;
                //        session = null;
                //        webBrowser.Source = new Uri(this.request);
                //        mainWindow.Height = 600;
                //    }
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void btnCopyBothTokens_Click(object sender, RoutedEventArgs e)
        {
            if (txtAccessToken.Text != "" && txtRefreshToken.Text != "")
            {
                Clipboard.SetText(txtAccessToken.Text.Trim() + "{#}" + txtRefreshToken.Text.Trim());
                MessageBox.Show("Tokens Copied. Copied tokens are seperated by {#}.", "Tokens to clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var answer = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer == MessageBoxResult.Yes )
            {
                session = null;
                webBrowser.Visibility = Visibility.Visible;
                mainWindow.Height = 600;
                TokenConfig.writeTokens("", "");
                webBrowser.Source = new Uri(this.request);
            }
        }

    }
}
