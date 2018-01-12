using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using DeezerPlayerLib.Engine;
using DeezerPlayerLib.Enum;
using System.IO;

namespace DeezerWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var playlist = "dzmedia:///playlist/1490558021";
            var station = "dzradio:///radio-30781";

            var stream = args.Length > 0 ? args[0] : playlist;

            var connectConfig = new ConnectConfig()
            {
                ccAppId = "180202",
                product_id = "DeezerWrapper",
                product_build_id = "00001",
                ccUserProfilePath = GetDeezerTempFolder(),
                ccConnectEventCb = OnConnect
            };

            var connect = new Connect(connectConfig);
            connect.Start();
            connect.SetAccessToken("fr49mph7tV4KY3ukISkFHQysRpdCEbzb958dB320pM15OpFsQs");
            connect.ConnectOfflineMode();

            var version = connect.GetSdkVersion();
            var deviceId = connect.GetDeviceId();

            Console.WriteLine($"SDK Version: {version}");
            Console.WriteLine($"Device Id: {deviceId}");

            Thread.Sleep(5000);

            var player = new Player(connect, null);
            player.SongChanged += Player_SongChanged;
            player.Start(OnPlayerEvent);
            player.SetRepeatMode(QUEUELIST_REPEAT_MODE.DZ_QUEUELIST_REPEAT_MODE_ALL);
            player.LoadStream(stream);
            player.Play();

            string line;

            while((line = Console.ReadLine()) != "Q")
            {
                switch (line)
                {
                    case "P":
                        player.Pause();
                        break;
                    case "R":
                        player.Resume();
                        break;
                    case "N":
                        player.Next();
                        break;
                    case "L":
                        Console.WriteLine("Enter station");
                        var radio = Console.ReadLine();
                        if(!string.IsNullOrWhiteSpace(radio))
                        {
                            player.LoadStream(radio);
                            player.Play();
                        }
                        break;
                    default:
                        break;
                }
            }

            player.Dispose();
            connect.Dispose();
        }

        private static void Player_SongChanged(object sender, DeezerPlayer.Model.Song e)
        {
            Console.WriteLine($"SongChanged: {e.Artist.Name}");

        }

        private static void OnConnect(Connect connect, ConnectEvent connectEvent)
        {
            Console.WriteLine(connectEvent.eventType);
        }

        private static void OnPlayerEvent(Player player, PlayerEvent playerEvent)
        {
            Console.WriteLine($"{DateTime.Now} - {playerEvent.eventType}");

            if (playerEvent.eventType == PLAYER_EVENT_TYPE.DZ_PLAYER_EVENT_RENDER_TRACK_UNDERFLOW)
                player.Next();
        }

        private static string GetDeezerTempFolder()
        {
            var tempPath = Path.GetTempPath();
            var tempFolder = Path.Combine(tempPath, "DeezerPlayer");

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }
}
