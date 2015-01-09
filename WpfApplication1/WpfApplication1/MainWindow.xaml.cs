using System;
using System.IO;
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
using System.Windows.Forms;
using System.Timers;
using System.Windows.Threading;
using System.Speech.Recognition;
using MyWindowsMediaPlayerV2;


namespace WpfApplication1
{
    delegate void FuncPtr();
  
    public partial class MainWindow : Window
    {
        public class MediaCollection
        {
            string SongName { set; get; }
            string AlbumName { set; get; }
            string StyleName { set; get; }
            string ArtistName { set; get; }
        }
        string pathOfPlaylist;
        SpeechRecognizer recognizer = new SpeechRecognizer();
        Dictionary<string, FuncPtr> totoTab = new Dictionary<string, FuncPtr>();
        MyRemote myServerRemote;
        
        public MainWindow()
        {
           
            InitializeComponent();

            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);

            mediaElement1.LoadedBehavior = MediaState.Manual;
            volumeLabel.Text = "100";
            volumeSlider.Value = 100.0;
            mediaElement1.MouseDown += mediaElement1_MouseDown;
            
            Choices color = new Choices();
            color.Add(new string[] { "play", "stop", "pause", "plainécran", "avancerapide", "suivant", "précédent" });

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(color);
            Grammar g = new Grammar(gb);
            recognizer.LoadGrammar(g);
            totoTab["play"] = playFunc;
            totoTab["stop"] = stopFunc;
            /*Playfunc change */
            totoTab["pause"] = playFunc;
            totoTab["plainécran"] = fullScreen;
            totoTab["avancerapide"] = faster;
            totoTab["suivant"] = nextInPlaylist;
            totoTab["précédent"] = prevInPlaylist;
            myServerRemote = new MyRemote(ref mediaElement1);
         
            mediaElement1.MediaOpened += this.mediaElement1_MediaOpened;
        }

        /* Function pour reduire panel */
        private void reducePanel(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(mediaElement1, 2);
            GridPanel.Visibility = System.Windows.Visibility.Hidden;
            btnShowPanel.IsEnabled = true;
        }
        /* Function pour show panel */
        private void showPanel(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(mediaElement1, 1);
            GridPanel.Visibility = System.Windows.Visibility.Visible;
            btnShowPanel.IsEnabled = false;
        }

        void mediaElement1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Coucoucoucoucocuou");
            fullScreen();
        }
        /* Ici ca a changé full screen */
        bool isFullScreen = false;
        Object sauvContent;
        void fullScreen()
        {
            if (isFullScreen == false)
            {
                sauvContent = this.Content;
                this.Content = mediaElement1;
                LayoutRoot.Children.Remove(mediaElement1);
                this.Background = new SolidColorBrush(Colors.Black);
                
                mediaElement1.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                mediaElement1.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
               
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                isFullScreen = true;
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.White);
              
                this.Content = sauvContent;
                LayoutRoot.Children.Add(mediaElement1);
                
                mediaElement1.Height = Double.NaN;
                mediaElement1.Width = Double.NaN;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                isFullScreen = false;
            }
        }
        /* sert plus a rien */
        void reduceScreen()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;
        }
        /* Sert plus a rien */
        public void pauseFunc()
        {
            mediaElement1.Pause();
        }
        /* Ici ca change */
        public void playFunc()
        {
            Image myImage3 = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            if (isPlay == false)
            {
                if (mediaElement1 != null && mediaElement1.Source != null)
                {

                    mediaElement1.Play();
                    mediaElement1.SpeedRatio = 1;
                    isPlay = true;

                    bi3.UriSource = new Uri("Pause.png", UriKind.Relative);
                    bi3.EndInit();
                    imageButtonPlay.Source = bi3;
                }
                else
                    System.Windows.MessageBox.Show("Vous devez d'abord sélectionner un media a lire");
            }
            else
            {
                mediaElement1.Pause();
                bi3.UriSource = new Uri("Play-Normal-icon.png", UriKind.Relative);
                bi3.EndInit();

                imageButtonPlay.Source = bi3;
                isPlay = false;
            }
        }

        public void stopFunc()
        {
            mediaElement1.Stop();
            mediaElement1.Close();
        }

        void faster()
        {
            mediaElement1.SpeedRatio += 1.5;
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            totoTab[e.Result.Text].Invoke();
        }

        /* Ici ca change */
        bool isPlay = false;
        public void playClick(object sender, RoutedEventArgs e)
        {
            playFunc();
        }

        /* Ici ca change */
        private TimeSpan TotalTime;
        private DispatcherTimer _timer;
        void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(200);
            this._timer.Tick += new EventHandler(timerTick);
            if (mediaElement1.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = mediaElement1.NaturalDuration.TimeSpan;
                PositionControlSlider.Maximum = ts.TotalSeconds;
                PositionControlSlider.SmallChange = 1;
                PositionControlSlider.LargeChange = Math.Min(10, ts.Seconds / 10);
                _timer.Start();
            }
        }

        private void timerTick(Object sender, EventArgs e)
        {
            PositionControlSlider.Value = this.mediaElement1.Position.Seconds ;
            
        }


 
        void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Media terminé");
            _timer.Stop();
            if (playlist.Items.Count != 0 && playlist.SelectedIndex < playlist.Items.Count)
            {
                playlist.SelectedIndex = playlist.SelectedIndex + 1;
                mediaElement1.Source = new Uri(pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "Media(*.*)|*.*";
            ofd.ShowDialog();
            mediaElement1.MediaOpened += new RoutedEventHandler(mediaElement1_MediaOpened);
            //mediaElement1.Source = new Uri(ofd.FileName);
            fileName = System.IO.Path.GetFileName(ofd.FileName);
            label1.Text = fileName;
            mediaElement1.Play();
            mediaElement1.Volume = 100;

        }

        private void label1_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            playFunc();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
            mediaElement1.Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeLabel.Text = ((int)(volumeSlider.Value * 10)).ToString();
            mediaElement1.Volume = volumeSlider.Value / 100;

        }

        private void volumeLabel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlist.SelectedIndex != -1 && playlist.SelectedItems != null)
            {
                mediaElement1.Source = new Uri(pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                label1.Text = playlist.SelectedItem.ToString();
                mediaElement1.Play();
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result != null)
            {
                string[] file = Directory.GetFiles(fbd.SelectedPath);
                pathOfPlaylist = fbd.SelectedPath;
                List<string> fileList = new List<string>();
                for (int i = 0; i < file.Length; i++)
                    fileList.Add(System.IO.Path.GetFileName(file[i]));
                playlist.ItemsSource = fileList;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            prevInPlaylist();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            nextInPlaylist();
        }

        void nextInPlaylist()
        {
            if (playlist.Items.Count > 0)
            {
                if (playlist.SelectedIndex < playlist.Items.Count)
                    playlist.SelectedIndex = playlist.SelectedIndex + 1;
                else
                    playlist.SelectedIndex = 0;
                mediaElement1.Source = new Uri(pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
            }
        }

        void prevInPlaylist()
        {
            if (playlist.Items.Count > 0)
            {
                if (playlist.SelectedIndex > 0)
                    playlist.SelectedIndex = playlist.SelectedIndex - 1;
                else
                    playlist.SelectedIndex = playlist.Items.Count;
                mediaElement1.Source = new Uri(pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
            }
        }

        private void mediaElement1_Ended(object sender, MediaScriptCommandRoutedEventArgs e)
        {
            Console.WriteLine("MEDIA ENDED");
            if (playlist.SelectedIndex < playlist.Items.Count)
                playlist.SelectedIndex = playlist.SelectedIndex + 1;
            mediaElement1.Source = new Uri(pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
            mediaElement1.Play();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            mediaElement1.SpeedRatio += 2;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
        }

        private void PositionControlSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int SliderValue = (int)PositionControlSlider.Value;

            if (mediaElement1.Position.Seconds != SliderValue)
            {
                //            Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
                //            Create a TimeSpan with miliseconds equal to the slider value.
                TimeSpan ts = new TimeSpan(0, 0, 0, SliderValue, 0);

                mediaElement1.Position = ts;
            }
        }

        private void mediaElement1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("Coucoucocuou");
            PositionControlSlider.Value = mediaElement1.Position.Seconds;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            /*
            if (fullscreen == false)
            {
                LayoutRoot.Children.Remove(mediaElement1);
                this.Background = new SolidColorBrush(Colors.Black);
                this.Content = mediaElement1;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                mediaElement1.Position = TimeSpan.FromSeconds(currentposition);
                fullscreen = true;
            }
            else if (fullscreen == true)
            {
                this.Content = LayoutRoot;
                LayoutRoot.Children.Add(mediaElement1);
                this.Background = new SolidColorBrush(Colors.White);
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                mediaElement1.Position = TimeSpan.FromSeconds(currentposition);
                fullscreen = false;
            }*/
        }

        private void Button_Remote(object sender, RoutedEventArgs e)
        {
            myServerRemote._actived = (myServerRemote._actived == true) ? false : true;
            if (myServerRemote._actived == true)
            {
                myServerRemote.startRemote();
                System.Windows.MessageBox.Show("Thread .Net launched");
            }
            else
            {
                myServerRemote.stopRemote();
                System.Windows.MessageBox.Show("Thread .Net Stoped");
            }
        }
    }
}