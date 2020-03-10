using System;
using System.Collections.Generic;
using System.Linq;

using Android.Gms.Maps.Model;

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
                .SetTitle("Active Cases: " + ConfirmedList.Last().Item2)
                .SetSnippet(this.ProvinceState);
        }


    }
}