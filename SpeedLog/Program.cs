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
            Console.WriteLine("SpeedLog has 3 optional parameters:");
            Console.WriteLine("{log file with path}");
            Console.WriteLine("{seconds between checks}");
            Console.WriteLine("{if speed is slower that this in milliseconds, log it}");
            Console.WriteLine();

            string logPath = "";
            int checkInterval = 10, maxDelay = 0;

            if (args.Length == 0 || String.IsNullOrEmpty(args[0]))
            {
                logPath = System.IO.Path.GetTempPath() + "log.csv";
                Console.WriteLine(String.Format("Empty first parameter, logging to {0}", logPath));
            }
            else
            {
                logPath = args[0];
                Console.WriteLine(String.Format("First parameter supplied, logging to {0}", logPath));
            }

            if (args.Length < 2 || String.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Empty second parameter for interval, using 10 seconds");
            }
            else
            {
                if (Int32.TryParse(args[1], out checkInterval))
                {
                    Console.WriteLine("Second parameter supplied, using {0} seconds for interval", checkInterval);
                }
                else
                {
                    Console.WriteLine("Invalid second parameter for interval, using 10 seconds");
                    checkInterval = 10;
                }
            }

            if (args.Length < 3 || String.IsNullOrEmpty(args[2]))
            {
                Console.WriteLine("Empty third parameter for max delay, using 100 milliseconds");
            }
            else
            {
                if (Int32.TryParse(args[2], out maxDelay))
                {
                    Console.WriteLine("Third parameter supplied, using {0} for max delay", maxDelay);
                }
                else
                {
                    Console.WriteLine("Invalid third parameter for max delay, using 100 milliseconds");
                    maxDelay = 0;
                }
            }


            Console.WriteLine("Press ESC to stop");

            Stopwatch stopWatch = new Stopwatch();
            string result;
            ConsoleKeyInfo keyPressed;

            do
            {
                using (WebClient Client = new WebClient())
                {
                    bool logIt = true;

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

                        logIt = maxDelay == 0 || ts1.Milliseconds > maxDelay || ts2.Milliseconds > maxDelay;

                        result = String.Format(@"{0:yyyy-MM-ddTHH:mm:sszz},{1:00},{2:00}", DateTime.Now, ts1.Milliseconds, ts2.Milliseconds);
                    }
                    catch
                    {
                        result = String.Format(@"{0:yyyy-MM-ddTHH:mm:sszz},0,0", DateTime.Now);
                    }

                    try
                    {
                        using (StreamWriter file = File.AppendText(logPath))
                        {
                            if (logIt)
                            {
                                file.WriteLine(result);
                                Console.WriteLine(result);
                            }
                        }
                    }
                    catch
                    {
                        if (logIt)                            
                        Console.WriteLine(String.Format("{0:MM/dd/yy H:mm:sszz} error writing to log file", DateTime.Now));
                    }

                }

                keyPressed = ConsoleHelper.ReadKeyWithTimeOut(checkInterval * 1000);

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

