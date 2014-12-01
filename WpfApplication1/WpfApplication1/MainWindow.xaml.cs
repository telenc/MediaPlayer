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
using System.Speech.Recognition;

namespace WpfApplication1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>


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
        delegate void FuncPtr();
        Dictionary<string, FuncPtr> funcTab = new Dictionary<string, FuncPtr>();

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
            funcTab["play"] = playFunc;
            funcTab["stop"] = stopFunc;
            funcTab["pause"] = mediaElement1.Pause;
            funcTab["plainécran"] = fullScreen;
            funcTab["avancerapide"] = faster;
            funcTab["suivant"] = nextInPlaylist;
            funcTab["précédent"] = prevInPlaylist;
        }

        void mediaElement1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            fullScreen();
        }

        void fullScreen()
        {
            LayoutRoot.Children.Remove(mediaElement1);
            this.Background = new SolidColorBrush(Colors.Black);
            mediaElement1.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            mediaElement1.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Content = mediaElement1;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }

        void reduceScreen()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;
        }

        void playFunc()
        {
            mediaElement1.Play();
            mediaElement1.SpeedRatio = 1;
        }

        void stopFunc()
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
            funcTab[e.Result.Text].Invoke();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (mediaElement1 != null)
            {
                mediaElement1.Width = myWindows.Width;
                mediaElement1.Height = myWindows.Height;
                mediaElement1.Play();
                mediaElement1.SpeedRatio = 1;
              /*  
               * 
               *               Update Slider based on mediaElement position doesn't work; 
                 System.Timers.Timer timer = new System.Timers.Timer(1000);
                 timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);

                timer.Interval = 1000;
                timer.Enabled = true;
              */   
            }
            else
                System.Windows.MessageBox.Show("Vous devez d'abord sélectionner un media a lire");
               
        }

        void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            PositionControlSlider.Maximum = mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Media terminé");
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
            mediaElement1.Source = new Uri(ofd.FileName);
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
            mediaElement1.Pause();
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

 //            Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
 //            Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            mediaElement1.Position = ts;
        }

        private void mediaElement1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
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
    }
}