using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace CurrentSystemVolume
{
    class Program
    {
        static void Main(string[] args)
        {
            int noOfSecondsToWatch = 30;

            var allPeekValues = new List<float>();

            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out noOfSecondsToWatch))
                {

                }
            }

            try
            {
                using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
                {
                    using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                    {
                        for (int i = 0; i < noOfSecondsToWatch*2; i++)
                        {
                            bool hasPeek = false;
                            foreach (var session in sessionEnumerator)
                            {
                                using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                                {
                                    var audioPeek = audioMeterInformation.GetPeakValue();
                                    if (audioPeek > 0)
                                    {
                                        Console.WriteLine(audioPeek);
                                        allPeekValues.Add(audioPeek);
                                        hasPeek = true;
                                    }
                                }
                                Thread.Sleep(500);
                            }

                            if (!hasPeek)
                            {
                                Console.WriteLine(0);
                            }
                        }
                    }
                }
                if (allPeekValues.Any())
                {
                    Console.WriteLine("Maximum Peek: " + allPeekValues.Max());
                }
                else
                {
                    Console.WriteLine("Maximum Peek: 0");
                }

            }
            catch (Exception exception)
            {
                Console.Write("Unexpected error occured. Might be no audio device configured in your system." + exception.Message);
            }            
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    Console.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }
}
