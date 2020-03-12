using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

using System.Net;
using System.Net.Http;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace CoronaVirusTracker
{

    class MarkerMaker
    {
        public static InfectionData CreateMarkers(string comfirmedUrl, string deathUrl, string recoveredUrl)
        {
            //https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv
            //https://raw.githubusercontent.com/CSSEGISandData/COVID-19/blob/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv"
            InfectionData infectionData;
            List<Infection> infections = new List<Infection>();

            CsvReader comfirmedCSV = CreateCsvReader(comfirmedUrl);
            CsvReader deathCSV = CreateCsvReader(deathUrl);
            CsvReader recoveredCSV = CreateCsvReader(recoveredUrl);
            {
                comfirmedCSV.Read();
                comfirmedCSV.ReadHeader();
                string[] headers = ((CsvFieldReader)((CsvParser)comfirmedCSV.Parser).FieldReader).Context.HeaderRecord;

                deathCSV.Read();
                deathCSV.ReadHeader();

                recoveredCSV.Read();
                recoveredCSV.ReadHeader();

                while (comfirmedCSV.Read())
                {
                    deathCSV.Read();
                    recoveredCSV.Read();

                    string state = comfirmedCSV.GetField<string>("Province/State");
                    string region = comfirmedCSV.GetField<string>("Country/Region");
                    double lat = comfirmedCSV.GetField<double>("Lat");
                    double lon = comfirmedCSV.GetField<double>("Long");
                    
                    List<Tuple<string, InfectionCount>> DataList = new List<Tuple<string, InfectionCount>>();
                    
                    for (int i = 4; i < headers.Length; i++)
                    {
                        comfirmedCSV.TryGetField<int>(i, out int comfirmedvalue);
                        deathCSV.TryGetField<int>(i, out int deathvalue);
                        recoveredCSV.TryGetField<int>(i, out int recoveredValue);

                        DataList.Add(new Tuple<string, InfectionCount>(headers[i], new InfectionCount(comfirmedvalue, deathvalue, recoveredValue))); 
                    }

                    Infection infection = new Infection(state, region, lat, lon, DataList);
                    infections.Add(infection);


                }
                infectionData = new InfectionData(headers, infections);
            }
                
                return infectionData;
        }

        public static CsvReader CreateCsvReader(string url)
        {
            using var webClient = new WebClient();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv;
        }
    }

    
}