using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace SpeedLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ESC to stop");

            Stopwatch stopWatch = new Stopwatch();
            string result;
            ConsoleKey keyPressed;

            do
            {
                while (!Console.KeyAvailable)
                {
                    using (WebClient Client = new WebClient())
                    {
                        try
                        {
                            stopWatch.Start();
                            Client.DownloadString("http://www.google.com/images/google_favicon_128.png");
                            stopWatch.Stop();
                            TimeSpan ts1 = stopWatch.Elapsed;

                            stopWatch.Restart();
                            Client.DownloadString("http://www.bing.com/s/a/bing_p.ico");
                            stopWatch.Stop();
                            TimeSpan ts2 = stopWatch.Elapsed;

                            result = String.Format("{0:MM/dd/yy H:mm:ss} {1:00}ms,{2:00}ms", DateTime.Now, ts1.Milliseconds, ts2.Milliseconds);
                        }
                        catch
                        {
                            result = String.Format("{0:MM/dd/yy H:mm:ss} timeout", DateTime.Now);
                        }

                        try
                        {
                            using (StreamWriter file = File.AppendText(@"c:\temp\speedlog.csv"))
                            {
                                file.WriteLine(result);
                                Console.WriteLine(result);
                            }
                        }
                        catch
                        {
                            Console.WriteLine(String.Format("{0:MM/dd/yy H:mm:ss} error writing to log file", DateTime.Now));
                        }

                    }

                    System.Threading.Thread.Sleep(10000);
                }  //while no key pressed

                // See what key they pressed
                keyPressed = Console.ReadKey(true).Key;

                if (keyPressed == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("Paused, any key to restart...");
                    while (!Console.KeyAvailable)
                    { }
                }

            } while (keyPressed != ConsoleKey.Escape);
        }
    }

}

