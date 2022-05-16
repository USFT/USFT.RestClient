using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using USFT.RestClient.v1;
using System.Configuration;

namespace USFT.RestClient.HistoryDownloader
{
    // Copying and pasting this is outright lazy of me, but we're in a bit of a time crunch, and
    // there's very little reason to create an entirely new DLL project for the sake of demo code.
    // If you're 
    class Formatter
    {
        #region Parameters
        static string FileLocation { get { return ConfigurationManager.AppSettings["targetfile"];} }
        static bool OutputSerial { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputserial"], out ret); return ret; } }
        static bool OutputName { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputname"], out ret); return ret; } }
        static bool OutputLatitude { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputlatitude"], out ret); return ret; } }
        static bool OutputLongitude { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputlongitude"], out ret); return ret; } }
        static bool OutputHeading { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputheading"], out ret); return ret; } }
        static bool OutputVelocity { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputvelocity"], out ret); return ret; } }
        static bool OutputSatellites { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputsatellites"], out ret); return ret; } }
        static bool OutputIgnition { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputignition"], out ret); return ret; } }
        static bool OutputLastMoved { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputlastmoved"], out ret); return ret; } }
        static bool OutputLastUpdated { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputlastupdated"], out ret); return ret; } }
        static bool OutputFlags { get { bool ret = true; bool.TryParse(ConfigurationManager.AppSettings["outputflags"], out ret); return ret; } }
        #endregion

        static string GetFormatString()
        {
            bool[] selected = new bool[] { OutputSerial, OutputName, OutputLatitude, OutputLongitude, OutputHeading, OutputVelocity, OutputSatellites, OutputIgnition, OutputLastMoved, OutputLastUpdated, OutputFlags };
            List<string> fields = new List<string>();
            for (int i = 0; i < selected.Length; i++)
                if (selected[i])
                    fields.Add("{" + i + "}");
            return string.Join(",", fields);
        }

        internal static async Task Save(List<Location> results, string destination = null)
        {
            var format = GetFormatString();
            try
            {
                using(StreamWriter writer = new StreamWriter(destination ?? FileLocation, false))
                {
                    await writer.WriteLineAsync(string.Format(format,
                        "Serial",
                        "Name",
                        "Latitude",
                        "Longitude",
                        "Heading",
                        "Velocity",
                        "Satellites",
                        "Ignition",
                        "LastMoved",
                        "LastUpdated",
                        "OutputFlags"));

                    foreach(var entry in results)
                    {
                        await writer.WriteLineAsync(string.Format(format,
                            entry.DeviceId,
                            FormatString(entry.DeviceName),
                            FormatLatLon(entry.Latitude),
                            FormatLatLon(entry.Longitude),
                            entry.Heading,
                            entry.Velocity,
                            entry.Satellites,
                            entry.Ignition,
                            entry.LastMoved.HasValue ? FormatDateTime(entry.LastMoved.Value) : "",
                            FormatDateTime(entry.LastUpdated),
                            entry.OutputFlags));
                    }
                    await writer.FlushAsync();
                }
            }
            catch(Exception exc)
            {
                throw new Exception("Formatting CSV to location " + (destination ?? FileLocation) + " failed!", exc);
            }
        }

        static string FormatDateTime(DateTime input)
        {
            return input.ToString("G");
        }
        static string FormatLatLon(double input)
        {
            return input.ToString("F6");
        }
        static string FormatString(string input)
        {
            return "\"" + input.Replace("\"", "\\\"") + "\"";
        }
    }
}
