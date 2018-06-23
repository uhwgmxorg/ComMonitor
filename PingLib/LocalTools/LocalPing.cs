﻿using System;
using System.Globalization;
using System.Net.NetworkInformation;

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
