using Microsoft.Win32;
using System.Diagnostics;
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
        private DispatcherTimer timer = new DispatcherTimer();
        int selectedSong = -1;
        List<string> songs = new List<string>();
        List<Button> buttons = new List<Button>();
        bool playing = false;
        string folder = System.IO.Path.Combine(
            AppContext.BaseDirectory,
            "mp3-files"
        );
        bool changes = true;

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += ReloadSongs;
            timer.Start();
            FileSystemWatcher watcher = new FileSystemWatcher(folder);

            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Filter = "*.mp3";
            watcher.Created += Watcher_Changed;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            changes = true;
        }

        private void ReloadSongs(object? sender, EventArgs? e)
        {
            if(player.playState == WMPPlayState.wmppsStopped && selectedSong != -1)
            {
                buttons[selectedSong].Background = Brushes.White;
                ButtonStop.Content = "▶️";
                playing = false;
                selectedSong = -1;
            }
            if (!changes)
            {
                return;
            }
            string[] songsArray = Directory.GetFiles(folder, "*.mp3");
            songs.Clear();
            foreach (string s in songsArray)
            {
                songs.Add(s);
            }
            try
            {
                CanvasMain.Children.Clear();
            }
            catch { }
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
                button.Content = song.Split(".mp3")[0].Split(@"\")[^1];
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
            changes = false;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSong != -1)
                buttons[selectedSong].Background = Brushes.White;
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
                changes = true;
                ReloadSongs(null, null);
            }
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            changes = true;
            ReloadSongs(null, null);
        }

        private void ButtonFront_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSong == (songs.Count - 1) || selectedSong == -1)
                return;
            playing = true;
            ButtonStop.Content = "||";
            buttons[selectedSong].Background = Brushes.White;
            player.controls.stop();
            player.URL = songs[selectedSong + 1];
            player.controls.play();
            buttons[selectedSong + 1].Background = Brushes.Coral;
            selectedSong++;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSong == 0 || selectedSong == -1)
                return;
            playing = true;
            buttons[selectedSong].Background = Brushes.White;
            player.controls.stop();
            player.URL = songs[selectedSong - 1];
            player.controls.play();
            ButtonStop.Content = "||";
            buttons[selectedSong - 1].Background = Brushes.Coral;
            selectedSong--;
        }

        private void LabelInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            LabelInfo.Foreground = Brushes.White;
        }

        private void LabelInfo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Info info = new Info();
            info.Owner = this;
            info.SetFolder(folder);
            info.Show();
            this.Hide();
            info.Closed += Info_Closed;
        }

        private void Info_Closed(object? sender, EventArgs e)
        {
            this.Show();
        }

        private void LabelInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            LabelInfo.Foreground = Brushes.Black;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }
    }
}