using System;
using System.Diagnostics;
using System.IO;

namespace WebIDLDownloader
{
    public static class Program
    {
        private const string WebidlLocation = "webidl";

        public static void Main(string[] args)
        {
            if (Directory.Exists(WebidlLocation))
            {
                Console.WriteLine("Deleting old files...");
                Directory.Delete(WebidlLocation, true);
            }
            DownloadAllWebIdls();

            Console.WriteLine("Download Complete!");
            Console.ReadLine();
        }

        private static void DownloadAllWebIdls()
        {
            DownloadBlinkWebIdl();
            //DownloadGeckoWebIdl();
            //DownloadWebkitWebIdl();
            //DownloadEdgeWebIdl();
        }

        private static void DownloadBlinkWebIdl()
        {
            Console.WriteLine("Downloading Blink WebIDL files");

            PullLatestFiles(WebPlatformId.Blink);

            Console.WriteLine("Blink WebIDL file download complete");
        }

        private static void DownloadGeckoWebIdl()
        {
            Console.WriteLine("Downloading Gecko WebIDL files");

            PullLatestFiles(WebPlatformId.Gecko);

            Console.WriteLine("Gecko WebIDL file download complete");
        }

        private static void DownloadWebkitWebIdl()
        {
            Console.WriteLine("Downloading Webkit WebIDL files");

            PullLatestFiles(WebPlatformId.Webkit);

            Console.WriteLine("Webkit WebIDL file download complete");
        }

        private static void DownloadEdgeWebIdl()
        {
            Console.WriteLine("Downloading Edge WebIDL files");

            //Copy files over

            Console.WriteLine("Edge WebIDL file download complete");
        }

        private static void PullLatestFiles(WebPlatformId platform)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                //WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                WorkingDirectory = @"F:\GitHub\Browsers\" + platform,
                FileName = "cmd.exe",
                Arguments = "/K git pull --ff-only"
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }

    internal enum WebPlatformId
    {
        Blink,
        Gecko,
        Webkit,
        Edge
    }
}
