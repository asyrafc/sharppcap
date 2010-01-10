using System;
using System.Collections.Generic;

namespace SharpPcap.Test.Example11
{
    /// <summary>
    /// Stat collection capture example
    /// WinPcap specific feature
    /// </summary>
    public class WinPcapStatisticsMode
    {
        /// <summary>
        /// Stat collection capture example
        /// </summary>
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example11.Statistics.cs", ver);

            // Retrieve the device list
            var devices = PcapDeviceList.Instance;

            // If no devices were found print an error
            if(devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }
            
            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the available devices
            foreach(PcapDevice dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to gather statistics on: ");
            i = int.Parse( Console.ReadLine() );

            PcapDevice device = devices[i];

            // Register our handler function to the 'pcap statistics' event
            device.OnPcapStatistics += 
                new Pcap.PcapStatisticsModeEvent( device_OnPcapStatistics );

            // Open the device for capturing
            device.Open();

            // Handle TCP packets only
            device.SetFilter( "tcp" );

            // Set device to statistics mode
            device.Mode = PcapDevice.PcapMode.Statistics;

            Console.WriteLine();
            Console.WriteLine("-- Gathering statistics on \"{0}\", hit 'Enter' to stop...",
                device.Description);

            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();

            // Stop the capturing process
            device.StopCapture();

            // Print out the device statistics
            Console.WriteLine(device.Statistics().ToString());

            // Close the pcap device
            device.Close();
            Console.WriteLine("Capture stopped, device closed.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }

        static ulong oldSec=0;
        static ulong oldUsec=0;
        /// <summary>
        /// Gets a pcap stat object and calculate bps and pps
        /// </summary>
        private static void device_OnPcapStatistics(object sender, PcapStatisticsModeEventArgs e)
        {
            // Calculate the delay in microseconds from the last sample.
            // This value is obtained from the timestamp that's associated with the sample.
            ulong delay = (e.Statistics.Seconds - oldSec) * 1000000 - oldUsec + e.Statistics.MicroSeconds;

            // Get the number of Bits per second
            ulong bps = ((ulong)e.Statistics.RecievedBytes * 8 * 1000000) / delay;
            /*                                       ^       ^
                                                     |       |
                                                     |       | 
                                                     |       |
                            converts bytes in bits --        |
                                                             |
                        delay is expressed in microseconds --
            */

            // Get the number of Packets per second
            ulong pps = ((ulong)e.Statistics.RecievedPackets * 1000000) / delay;

            // Convert the timestamp to readable format
            var ts = e.Statistics.Date.ToLongTimeString();

            // Print Statistics
            Console.WriteLine("{0}: bps={1}, pps={2}", ts, bps, pps); 

            //store current timestamp
            oldSec = e.Statistics.Seconds;
            oldUsec = e.Statistics.MicroSeconds;
        }
    }
}
