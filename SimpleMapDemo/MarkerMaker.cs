using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Graphics.Bitmap;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace CoronaVirusTracker
{
    class MarkerMaker
    {
        public static List<Infection> CreateMarkers(string url)
        {
            //https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv
            List<Infection> infections = new List<Infection>();

            using var webClient = new WebClient();
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                var header = csv.ReadHeader();
                while (csv.Read())
                {
                    string state = csv.GetField<string>("Province/State");
                    string region = csv.GetField<string>("Country/Region");
                    double lat = csv.GetField<double>("Lat");
                    double lon = csv.GetField<double>("Long");
                    int confirmed = csv.GetField<int>("3/7/20");

                    Infection infection = new Infection(state, region, lat, lon, confirmed);
                    infections.Add(infection);


                }
            }

                return infections;
        }
    }

    
}