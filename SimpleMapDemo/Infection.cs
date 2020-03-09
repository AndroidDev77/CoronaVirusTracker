using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CsvHelper.Configuration.Attributes;
using static Android.Graphics.Bitmap;

//https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/01-22-2020.csv
//https://github.com/CSSEGISandData/COVID-19/blob/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv
namespace CoronaVirusTracker
{

    class Infection
    {
        public MarkerOptions MarkerOption { get; set; }

        public CircleOptions CircleOption { get; set; }
        public string ProvinceState { get; set; }

        public string CountryRegion { get; set; }

        LatLng LatLng;

        public List<Tuple<string, int>> ConfirmedList;

        public float radius = 0.0f;

    public Infection(string provinceState, string countryRegion, double Lat, double lon, List<Tuple<string, int>> ConfirmedList) 
        {
            this.ProvinceState = provinceState;
            this.CountryRegion = countryRegion;
            this.LatLng = new LatLng(Lat, lon);
            this.ConfirmedList = ConfirmedList;

            MarkerOption = new MarkerOptions()
                .SetPosition(this.LatLng)
                .Anchor(0.5f, 0.5f)
                .SetAlpha(1.0f)
                .SetTitle("" + ConfirmedList.Last().Item2)
                .SetSnippet(this.ProvinceState);
                //.SetIcon(MakeCircle(dpToPx(CirclePxSize(this.NumCases))));
            
        }


    }
}