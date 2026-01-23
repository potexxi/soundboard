using Microsoft.Win32;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WMPLib;

namespace soundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowsMediaPlayer player = new WindowsMediaPlayer();
        private DispatcherTimer TimerSearchSongs = new DispatcherTimer();
        int selectedSong = -1;
        List<string> songs = new List<string>();
        List<Button> buttons = new List<Button>();
        bool playing = false;
        string folder = "mp3-files";
        public MainWindow()
        {
            InitializeComponent();
            ReloadSongs();
            TimerSearchSongs.Tick += TimerSearchSongs_Tick;
            TimerSearchSongs.Interval = TimeSpan.FromSeconds(1);
            TimerSearchSongs.Start();
        }

        private void TimerSearchSongs_Tick(object? sender, EventArgs e)
        {
            ReloadSongs();
        }

        private void ReloadSongs()
        {
            string[] songsArray = Directory.GetFiles(folder, "*.mp3");
            songs.Clear();
            foreach (string s in songsArray)
            {
                songs.Add(s);
            }
            //CanvasMain.Children.Clear();
            buttons.Clear();
            int row = 0;
            int counter = 0;
            foreach (string song in songs)
            {   
                if (counter == 4)
                {
                    row++;
                    counter = 0;
                }
                Button button = new Button();
                button.Width = 150;
                button.Height = 60;
                button.Background = Brushes.White;
                button.Content = song.Split(@"\")[1].Split(".")[0];
                button.Click += Button_Click;
                CanvasMain.Children.Add(button);
                Canvas.SetLeft(button, (counter * 180) + 20);
                Canvas.SetTop(button, (row * 100) + 20);
                buttons.Add(button);
                counter++;
            }
            if (selectedSong != -1)
                buttons[selectedSong].Background = Brushes.Coral;
            CanvasMain.Height = Math.Ceiling(songs.Count / 4.0) * 100;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int counter = 0;
            int button_number = -1;
            foreach(Button button1 in buttons)
            {
                button1.Background = Brushes.White;
                if (button1 == button)
                {
                    button_number = counter;
                    selectedSong = counter;
                    button1.Background = Brushes.Coral;
                    break;
                }
                counter++;
            }
            player.controls.stop();
            if (button_number != -1)
            {
                string songPath = songs[button_number];

                player.URL = songPath;
                player.controls.play();
                ButtonStop.Content = "||";
                playing = true;
            }
            
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (playing == true)
            {
                ButtonStop.Content = "▶️";
                player.controls.pause();
                playing = false;
            }
            else if(playing == false)
            {
                if (player.playState == WMPPlayState.wmppsPaused)
                {
                    playing = true;
                    ButtonStop.Content = "||";
                    player.controls.play();
                }
            }
            

        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog{};

            if (folderDialog.ShowDialog() == true)
            {
                folder = folderDialog.FolderName;
            }
        }
    }
}