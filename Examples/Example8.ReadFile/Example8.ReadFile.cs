using System;
using SharpPcap.Packets;

namespace SharpPcap.Test.Example8
{
    public class ReadFile
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example8.ReadFile.cs\n", ver);

            Console.WriteLine();
            Console.Write("-- Please enter an input capture file name: ");
            string capFile = Console.ReadLine();

            PcapDevice device;

            try
            {
                // Get an offline device
                device = new PcapOfflineDevice( capFile );

                // Open the device
                device.Open();
            } 
            catch(Exception e)
            {
                Console.WriteLine("Caught exception when opening file" + e.ToString());
                return;
            }

            //Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new SharpPcap.Pcap.PacketArrivalEvent( device_OnPacketArrival );

            Console.WriteLine();
            Console.WriteLine
                ("-- Capturing from '{0}', hit 'Ctrl-C' to exit...",
                capFile);

            // Start capture 'INFINTE' number of packets
            // This method will return when EOF reached.
            device.Capture( SharpPcap.Pcap.INFINITE );

            // Close the pcap device
            device.Close();
            Console.WriteLine("-- End of file reached.");
            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints the source and dest MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, PcapCaptureEventArgs e)
        {       
            if( e.Packet is EthernetPacket )
            {
                EthernetPacket etherFrame = (EthernetPacket)e.Packet;
                Console.WriteLine("At: {0}:{1}: MAC:{2} -> MAC:{3}",
                    etherFrame.PcapHeader.Date.ToString(),
                    etherFrame.PcapHeader.Date.Millisecond,
                    etherFrame.SourceHwAddress, 
                    etherFrame.DestinationHwAddress);
            }
        }
    }
}
