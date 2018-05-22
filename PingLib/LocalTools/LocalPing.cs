using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PingLib.LocalTools
{
    static public class LocalPing
    {
        static string PingTarget { get; set; }

        /// <summary>
        /// Ping
        /// </summary>
        static public double Ping(string pingTarget)
        {
            string roundtripTime = "0";

            PingTarget = pingTarget;
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(PingTarget);
                if (reply.Status == IPStatus.Success)
                {
                    if (reply.RoundtripTime == 0)
                        roundtripTime = "0.0001";
                    else
                        roundtripTime = reply.RoundtripTime.ToString();
                }
                return Convert.ToDouble(roundtripTime);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }
    }
}
