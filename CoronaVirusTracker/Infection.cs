using System;
using System.Collections.Generic;
using System.Linq;

using Android.Gms.Maps.Model;

namespace CoronaVirusTracker
{
    class InfectionData
    {
        public List<Infection> Infections { get; set; }
        public string[] Dates { get; set; }
        public int Count { get; set; }

        public InfectionData(string[] Dates, List<Infection> Infections)
        {
            this.Dates = Dates;
            this.Infections = Infections;
            this.Count = Dates.Length;

        }
    }

    class InfectionCount
    {
        public int ComfirmedCases { get; set; }
        public int DeathCases { get; set; }
        
        public int RecoveredCases { get; set; }

        public InfectionCount(int ComfirmedCases, int DeathCases, int RecoveredCases)
        {
            this.ComfirmedCases = ComfirmedCases;
            this.DeathCases = DeathCases;
            this.RecoveredCases = RecoveredCases;
        }
    }
    class Infection
    {
        public MarkerOptions MarkerOption { get; set; }

        public CircleOptions CircleOption { get; set; }
        public string ProvinceState { get; set; }

        public string CountryRegion { get; set; }

        LatLng LatLng;

        public List<Tuple<string, InfectionCount>> InfectionCountList;

        public float radius = 0.0f;

    public Infection(string provinceState, string countryRegion, double Lat, double lon, List<Tuple<string, InfectionCount>> InfectionCountList) 
        {
            this.ProvinceState = provinceState;
            this.CountryRegion = countryRegion;
            this.LatLng = new LatLng(Lat, lon);
            this.InfectionCountList = InfectionCountList;

            MarkerOption = new MarkerOptions()
                .SetPosition(this.LatLng)
                .Anchor(0.5f, 0.5f)
                .SetAlpha(1.0f)
                .SetTitle(this.ProvinceState)
                .SetSnippet("Cases: " + InfectionCountList.Last().Item2.ComfirmedCases + "\n" +
                "Deaths: " + InfectionCountList.Last().Item2.DeathCases + "\n" +
                "Recovered: " + InfectionCountList.Last().Item2.RecoveredCases + "\n" +
                "Active: " + (InfectionCountList.Last().Item2.ComfirmedCases - InfectionCountList.Last().Item2.RecoveredCases - InfectionCountList.Last().Item2.DeathCases));
        }


    }
}