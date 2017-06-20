using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Exceptions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

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
        public const int formHeight = 200;
        #endregion variables

        public MainWindow()
        {
            InitializeComponent();
            this.request = BoxManagerConst.authurl + "&client_id=" + BoxManagerConst.client_id + "&state=security_token%3DKnhMJatFipTAnM0nHlZA";
        }


        /// <summary>
        /// Writes new tokens into tokens.xml file whenever new tokesn are obtained or refreshed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Auth_SessionAuthenticated(object sender, SessionAuthenticatedEventArgs e) 
        {
            TokenConfig.writeTokens(e.Session.AccessToken, e.Session.RefreshToken);
        }

        /// <summary>
        /// Web Browser redirection in case of first login/logout or corrupted tokens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    lblLoggedInUser.Text = loggedin_user;
                    main.Visibility = Visibility.Visible;
                    txtAccessToken.Text = session.AccessToken;
                    txtRefreshToken.Text = session.RefreshToken;
                    mainWindow.Height = formHeight;
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

        /// <summary>
        /// main window load method that checks the valid session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        mainWindow.Height = formHeight;
                    }
                }

                catch (BoxException)
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
                MessageBox.Show("Tokens Copied. Copied tokens are seperated by {#}.", "Copy Tokens to clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// logout session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
