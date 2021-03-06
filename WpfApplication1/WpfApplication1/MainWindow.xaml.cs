﻿using System;
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
using System.Threading;

namespace WpfApplication1
{
    public delegate void FuncPtr();//putain de delegate de merde, le c# c nul
  
    public partial class MainWindow : Window
    {
        Dictionary<string, FuncPtr> _funcTab = new Dictionary<string, FuncPtr>();
        string                      _pathOfPlaylist;
        bool                        _isFullScreen = false;
        bool                        _isPlaying = false;
        MyRemote                    _remoteServer;
        DispatcherTimer             _timer;
        Speecher                    _speecher;
        Object                      sauvContent;
        bool                        _isRepeat;
        PlayList                    pl;

        public MainWindow()
        {           
            InitializeComponent();

            mediaElement1.LoadedBehavior = MediaState.Manual;
            volumeLabel.Text = "100";
            volumeSlider.Value = 100.0;
            mediaElement1.MouseDown += mouseDownEventToFullscreen;
            
            _funcTab["play"] = playFunc;
            _funcTab["stop"] = stopFunc;
            _funcTab["pause"] = playFunc;
            _funcTab["plainécran"] = fullScreen;
            _funcTab["avancerapide"] = faster;
            _funcTab["suivant"] = nextInPlaylist;
            _funcTab["precedent"] = prevInPlaylist;
            _remoteServer = new MyRemote();
            _remoteServer._funcTab = _funcTab;
            _speecher = new Speecher(ref _funcTab, new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized));         
            mediaElement1.MediaOpened += this.mediaElement1_MediaOpened;
        }

        void reducePanel(object sender, RoutedEventArgs e)
        {
            GridPanel.Visibility = System.Windows.Visibility.Hidden;
            btnShowPanel.IsEnabled = true;
        }

        void showPanel(object sender, RoutedEventArgs e)
        {
            GridPanel.Visibility = System.Windows.Visibility.Visible;
            btnShowPanel.IsEnabled = false;
        }

        void mouseDownEventToFullscreen(object sender, MouseButtonEventArgs e)
        {
            fullScreen();
        }
 
        void fullScreen()
        {
            if (_isFullScreen == false)
            {
                sauvContent = this.Content;
                this.Content = mediaElement1;
                MediaRoot.Children.Remove(mediaElement1);
                this.Background = new SolidColorBrush(Colors.Black);
                
                mediaElement1.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
                mediaElement1.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
               
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                _isFullScreen = true;
            }
            else
            {
                this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF252E35"));
                this.Content = sauvContent;
               // MediaRoot.Children.Add(mediaElement1);
                MediaRoot.Children.Insert(0, mediaElement1);
                mediaElement1.Height = Double.NaN;
                mediaElement1.Width = Double.NaN;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Maximized;
                _isFullScreen = false;
            }
        }

        void playFunc()
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            if (_isPlaying == false)
            {
                if (mediaElement1 != null && mediaElement1.Source != null)
                {
                    mediaElement1.Play();
                    mediaElement1.SpeedRatio = 1;
                    _isPlaying = true;
                    bi3.UriSource = new Uri("Ressources/Pause.png", UriKind.Relative);
                    bi3.EndInit();
                    imageButtonPlay.Source = bi3;
                }
                else
                    System.Windows.MessageBox.Show("Vous devez d'abord sélectionner un media a lire");
            }
            else
            {
                mediaElement1.Pause();
                bi3.UriSource = new Uri("Ressources/play2.png", UriKind.Relative);
                bi3.EndInit();
                imageButtonPlay.Source = bi3;
                _isPlaying = false;
            }
        }

        void stopFunc()
        {
            mediaElement1.Stop();
            mediaElement1.Close();
            _isPlaying = false;
        }

        void faster()
        {
            mediaElement1.SpeedRatio += 1.5;
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            _funcTab[e.Result.Text].Invoke();
        }

        void playClick(object sender, RoutedEventArgs e)
        {
            playFunc();
        }

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

        void timerTick(Object sender, EventArgs e)
        {
            PositionControlSlider.Value = this.mediaElement1.Position.Seconds;
        }

        void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            if (playlist.Items.Count != 0 && playlist.SelectedIndex < playlist.Items.Count)
            {
                playlist.SelectedIndex = playlist.SelectedIndex + 1;
                mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
                _isPlaying = true;
            }
            if (_isRepeat == true)
            {
                mediaElement1.Stop();
                mediaElement1.Play();
            }
        }

        void addFileButtonClick(object sender, RoutedEventArgs e)
        {
            string fileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "Media(*.*)|*.*";
            ofd.ShowDialog();
            mediaElement1.MediaOpened += new RoutedEventHandler(mediaElement1_MediaOpened);
            if (ofd.FileName != "")
            {
                mediaElement1.Source = new Uri(ofd.FileName);
                fileName = System.IO.Path.GetFileName(ofd.FileName);
                label1.Text = fileName;
                mediaElement1.Play();
                mediaElement1.Volume = 100;
                _isPlaying = true;
            }
        }

        void label1_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        void playButtonClick(object sender, RoutedEventArgs e)
        {
            playFunc();
        }

        void stopButtonClick(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
            mediaElement1.Close();
            _isPlaying = false;
        }

        void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeLabel.Text = ((int)(volumeSlider.Value * 10)).ToString();
            mediaElement1.Volume = volumeSlider.Value / 100;
        }

        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlist.SelectedIndex != -1 && playlist.SelectedItems != null)
            {
                mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                label1.Text = playlist.SelectedItem.ToString();
                mediaElement1.Play();
                _isPlaying = true;
            }

        }

        void folderToPlaylist(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            PlayList pl = new PlayList(fbd);
            if (fbd.SelectedPath != "")
            {
                _pathOfPlaylist = fbd.SelectedPath;
                for (int i = 0; i < pl._directory.Length; i++)
                {
                    String extension = System.IO.Path.GetExtension(pl._directory[i]);
                    if (extension == ".bmp" || extension == ".jpg" || extension == ".mp3" || extension == ".mp4" || extension == ".avi" ||
                    extension == ".mkv" || extension == ".ogg" || extension == ".flv")
                        pl.fileList.Add(System.IO.Path.GetFileName(pl._directory[i]));
                }
                playlist.ItemsSource = pl.fileList;
            }
        }

        void prevInPlaylistClick(object sender, RoutedEventArgs e)
        {
            prevInPlaylist();
        }

        void nextInPlaylistClick(object sender, RoutedEventArgs e)
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
                mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
                _isPlaying = true;
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
                mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
                _isPlaying = true;
            }
        }

        void mediaElement1_Ended(object sender, MediaScriptCommandRoutedEventArgs e)
        {
            if (playlist.SelectedIndex < playlist.Items.Count)
                playlist.SelectedIndex = playlist.SelectedIndex + 1;
            mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
            mediaElement1.Play();
            _isPlaying = true;
            
            //if (_isRepeat == true)
        }

        void fasterClick(object sender, RoutedEventArgs e)
        {
            mediaElement1.SpeedRatio += 2;
        }

        void PositionControlSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int SliderValue = (int)PositionControlSlider.Value;

            if (mediaElement1.Position.Seconds != SliderValue)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, SliderValue, 0);
                mediaElement1.Position = ts;
            }
        }

        public void Invokefunc(String cmd)
        {
            this._funcTab[cmd].Invoke();
        }

        void mediaElement1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PositionControlSlider.Value = mediaElement1.Position.Seconds;
        }

        void Button_Remote(object sender, RoutedEventArgs e)
        {
            _remoteServer._actived = (_remoteServer._actived == true) ? false : true;
            if (_remoteServer._actived == true)
            {
                    this._remoteServer.loopTcpRemote();//));
                    //              this._threadServer = new Thread(new ThreadStart(
                    //                this._threadServer.Start();
                    //               _remoteServer.startRemote();
                System.Windows.MessageBox.Show("Thread .Net launched");
            }
            else
            {
                    //this._threadServer.Abort();
                    //this._threadServer.Join();
                System.Windows.MessageBox.Show("Network Thread Stoped");
                    //                _remoteServer.stopRemote();
            }
        }

        private void repeatMedia(object sender, RoutedEventArgs e)
        {
            if (_isRepeat == false)
            {
                System.Windows.MessageBox.Show("Repeat ON");
                _isRepeat = true;
            }
            else
            {
                _isRepeat = false;
                System.Windows.MessageBox.Show("Repeat OFF");
            }
        }

        private void randomPlay(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, playlist.Items.Count);

            playlist.SelectedIndex = randomNumber;
            if (_pathOfPlaylist != "" && playlist.Items.Count != 0)
            {
                mediaElement1.Source = new Uri(_pathOfPlaylist + "\\" + playlist.SelectedItem.ToString());
                mediaElement1.Play();
                _isPlaying = true;
            }
        }

        private void resetSpeed(object sender, RoutedEventArgs e)
        {
            mediaElement1.SpeedRatio = 1;
        }
    }
}