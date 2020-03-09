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
    class InfectionData
    {
        public List<Infection> Infections { get; set; }
        public string[] Dates { get; set; }
        public int Count { get; set; }

        public InfectionData (string[] Dates, List<Infection> Infections)
        {
            this.Dates = Dates;
            this.Infections = Infections;
            this.Count = Dates.Length;

        }
    }
    class MarkerMaker
    {
        public static InfectionData CreateMarkers(string url)
        {
            //https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv
            InfectionData infectionData;
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
                csv.ReadHeader();
                string[] headers = ((CsvFieldReader)((CsvParser)csv.Parser).FieldReader).Context.HeaderRecord;

                while (csv.Read())
                {
                    string state = csv.GetField<string>("Province/State");
                    string region = csv.GetField<string>("Country/Region");
                    double lat = csv.GetField<double>("Lat");
                    double lon = csv.GetField<double>("Long");
                    
                    List<Tuple<string,int>> ConfirmedList = new List<Tuple<string, int>>();
                    int value = 0;
                    for (int i = 4; i < headers.Length; i++)
                    {
                        value = 0;
                        csv.TryGetField<int>(i, out value);
                        ConfirmedList.Add(new Tuple<string, int>(headers[i], value));
                        
                    }

                    Infection infection = new Infection(state, region, lat, lon, ConfirmedList);
                    infections.Add(infection);


                }
                infectionData = new InfectionData(headers, infections);
            }
                
                return infectionData;
        }
    }

    
}