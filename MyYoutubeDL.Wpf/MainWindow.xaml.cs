using NYoutubeDL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Path = System.IO.Path;

namespace MyYoutubeDL.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string youtubeUrl = tbUrl.Text;
            Task.Run(async () =>
            {
                await DownloadVideoAsync(youtubeUrl);

            });

        }

        private async Task DownloadVideoAsync(string youtubeUrl)
        {
            try
            {
                var youtubeDl = new YoutubeDL();
                WriteToStatusBar("Started downloading");
                DirectoryInfo dir = new DirectoryInfo("Videos");
                if (!dir.Exists)
                {
                    dir.Create();
                }

                var exportDir = dir.CreateSubdirectory(DateTime.Now.ToString("yyyy-MM-dd"));
                var exportFilePath = Path.Combine(exportDir.FullName, "video" + DateTime.Now.ToString("hh-mm-ss") + ".mkv");
                
                //set format output and library locations
                youtubeDl.Options.FilesystemOptions.Output = exportFilePath;
                youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
                youtubeDl.Options.SubtitleOptions.AllSubs = true;
                youtubeDl.Options.PostProcessingOptions.KeepVideo = true;
                youtubeDl.Options.PostProcessingOptions.ConvertSubs = NYoutubeDL.Helpers.Enums.SubtitleFormat.srt;
                youtubeDl.Options.PostProcessingOptions.FfmpegLocation = @"ffmpeg-static\bin\ffmpeg.exe";
                youtubeDl.VideoUrl = youtubeUrl;

                // Optional, required if binary is not in $PATH
                youtubeDl.YoutubeDlPath = @"youtube-dl\youtube-dl.exe";
                youtubeDl.StandardOutputEvent += (sender, output) => WriteToStatusBar(output);
                youtubeDl.StandardErrorEvent += (sender, errorOutput) => WriteToStatusBar(errorOutput);
                // youtubeDl.Info.PropertyChanged += delegate { < your code here> };

                // Prepare the download (in case you need to validate the command before starting the download)
                string commandToRun = await youtubeDl.PrepareDownloadAsync();

                // Just let it run
                youtubeDl.Download();

                // Wait for it

                WriteToStatusBar("Done! " + exportFilePath);
            }
            catch (Exception e)
            {
                WriteToStatusBar(e.Message.ToString());

            }
        }

        public void WriteToStatusBar(string message)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                lblStatus.Content = message;
            }));
         
        }
    }
}
