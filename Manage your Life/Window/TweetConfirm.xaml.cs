using CoreTweet;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Manage_your_Life
{
    /// <summary>
    /// TweetConfirm.xaml の相互作用ロジック
    /// </summary>
    public partial class TweetConfirm : Window
    {

        DataBanker dataBanker = DataBanker.Instance;

        private string ck = "nV7WUMvQV0WNoXaL2jxb47ydC";
        private string cks = "gAje4KL3JL9Y6Sfr2KnMNlrhxdX6Bf2xcgYMjnFyquxZ4z1aGw";



        public TweetConfirm()
        {
            InitializeComponent();

            this.textBox_Confi.Text = (string)dataBanker["TweetConfirm"];
            this.textBox_Format.Text = Properties.Settings.Default.TweetFormat;
        }


        private void button_Tweet_Click(object sender, RoutedEventArgs e)
        {
            string tweetString = this.textBox_Confi.Text;
            Tokens tokens = new Tokens();

            //トークンが取得できていなかったら新規取得
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.AccessToken))
            {
                var session = OAuth.Authorize(ck, cks);
                var url = session.AuthorizeUri;
                Process.Start(url.AbsoluteUri);

                GetTwitterPin window = new GetTwitterPin();
                window.ShowDialog();

                string pin = (string)dataBanker["PIN"];
                tokens = session.GetTokens(pin);

                Properties.Settings.Default.AccessToken = tokens.AccessToken;
                Properties.Settings.Default.AccessTokenSecret = tokens.AccessTokenSecret;
                Properties.Settings.Default.Save();
            }
            else
            {
                tokens = Tokens.Create(ck, cks,
                        Properties.Settings.Default.AccessToken,
                        Properties.Settings.Default.AccessTokenSecret);
            }


            string filename = "tweet.png";
            MediaUploadResult result = new MediaUploadResult();

            //画像ツイート機能
            if (Properties.Settings.Default.checkBox_IsTweetImage)
            {
                //画像の投稿
                result = tokens.Media.Upload(media => new FileInfo(filename));
            }


            //文字数オーバー
            if (Properties.Settings.Default.checkBox_IsTweetOver)
            {
                switch (Properties.Settings.Default.checkBox_IsTweetImage)
                {
                    case true:
                        if (tweetString.Length >= 110)
                        {
                            tweetString = tweetString.Substring(0, 110);
                        }
                        tokens.Statuses.Update(status => tweetString + "...", media_ids => new long[] { result.MediaId });
                        break;

                    case false:
                        if (tweetString.Length >= 135)
                        {
                            tweetString = tweetString.Substring(0, 135);
                        }
                        tokens.Statuses.Update(new { status = tweetString + "..." });
                        break;
                }
            }
            else
            {
                if (tweetString.Length >= 140)
                {
                    MessageBox.Show(String.Format("文字数がオーバーしました。 ({0})", tweetString.Length), "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    tokens.Statuses.Update(new { status = tweetString });
                }
            }

            MessageBox.Show("Tweetしました。\n" + tweetString, "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            tweetString = "";
            this.Close();
        }




        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.TweetFormat = this.textBox_Format.Text;
            Properties.Settings.Default.Save();
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ウィンドウをドラッグで動かせるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            PostMessage(hwnd, 0xA1, (IntPtr)2, IntPtr.Zero);
        }

        private void textBox_Confi_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.label_TextBoxLength.Content = this.textBox_Confi.Text.Length;
        }
                
    }
}
