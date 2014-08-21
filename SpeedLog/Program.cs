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
            ConsoleKeyInfo keyPressed;

            do
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

                        result = String.Format(@"{0:yyyy-MM-ddTHH:mm:ss.fffffffzz},{1:00}ms,{2:00}ms", DateTime.Now, ts1.Milliseconds, ts2.Milliseconds);
                    }
                    catch
                    {
                        result = String.Format(@"{0:yyyy-MM-ddTHH:mm:ss.fffffffzz},timeout", DateTime.Now);
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

                keyPressed = ConsoleHelper.ReadKeyWithTimeOut(10000);

                if (keyPressed.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("Paused, any key to restart...");
                    while (!Console.KeyAvailable)
                    { }
                }

            } while (keyPressed.Key != ConsoleKey.Escape);
        }
    }

    // From http://hen.co.za/blog/2011/07/console-readkey-with-timeout/
    public static class ConsoleHelper
    {

        public static ConsoleKeyInfo ReadKeyWithTimeOut(int timeOutMS)
        {

            DateTime timeoutvalue = DateTime.Now.AddMilliseconds(timeOutMS);

            while (DateTime.Now < timeoutvalue)
            {

                if (Console.KeyAvailable)
                {

                    ConsoleKeyInfo cki = Console.ReadKey();
                    return cki;

                }
                else
                {

                    System.Threading.Thread.Sleep(100);

                }

            }
            return new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false);

        }

    }
}

