using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace PingLib.LocalTools
{
    static public class LocalPing
    {
        static string PingTarget { get; set; }

        /// <summary>
        /// PingAsync
        /// </summary>
        static public async Task<double> PingAsync(string pingTarget)
        {
            string roundtripTime = "0";

            PingTarget = pingTarget;
            try
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(PingTarget);
                if (reply.Status == IPStatus.Success)
                {
                    if (reply.RoundtripTime == 0)
                        roundtripTime = "0.9";
                    else
                        roundtripTime = reply.RoundtripTime.ToString();
                }
                
                return Convert.ToDouble(roundtripTime, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return -1.0;
            }
        }

        /// <summary>
        /// Ping
        /// </summary>
        /// <param name="pingTarget"></param>
        /// <returns></returns>
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
                        roundtripTime = "0.9";
                    else
                        roundtripTime = reply.RoundtripTime.ToString();
                }
                return Convert.ToDouble(roundtripTime, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return -1.0;
            }
        }
    }
}
