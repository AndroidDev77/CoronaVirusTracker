using System;
using System.Collections.Generic;

using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Content.Res;
using Android.Support.V7.App;
using Android.Widget;

using Android.Graphics;
using static Android.Graphics.Bitmap;
using Android.Util;

namespace CoronaVirusTracker
{
    [Activity(Label = "@string/activity_label_mapwithmarkers")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        static readonly string TAG = "MapWithMarkersActivity";
        static readonly LatLng PasschendaeleLatLng = new LatLng(36.897778, -76.013333);
        static readonly LatLng VimyRidgeLatLng = new LatLng(36.379444, -77.773611);
        static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;
        GoogleMap googleMap;
        List<Infection> Infections;
        int numCases = 0;
        //GeoJsonLayer layer;


        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.mapStyle));
            
            AddMarkersToMap();

            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            {
                InitializeUiSettingsOnMap(); 
            }
        }
        void InitializeUiSettingsOnMap()
        {
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            
            googleMap.MyLocationEnabled = true;

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MapLayout);

            var mapFragment = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

        }


        void AnimateToPasschendaele(object sender, EventArgs e)
        {
            // Move the camera to the PasschendaeleLatLng Memorial in Belgium.
            var builder = CameraPosition.InvokeBuilder();
            builder.Target(PasschendaeleLatLng);
            builder.Zoom(18);
            builder.Bearing(155);
            builder.Tilt(65);
            var cameraPosition = builder.Build();

            // AnimateCamera provides a smooth, animation effect while moving
            // the camera to the the position.
            googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
        }

        void AddMarkersToMap()
        {
            string url = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv";
            Infections = MarkerMaker.CreateMarkers(url);
            numCases = 0;
            foreach(Infection aInfection in Infections)
            {
                numCases += aInfection.NumCases;
                MarkerOptions markerOptions = aInfection.MarkerOption.SetIcon(MakeCircle(DpToPx(CirclePxSize(aInfection.NumCases))));
                googleMap.AddMarker(markerOptions);
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == REQUEST_PERMISSIONS_LOCATION)
            {
                if (grantResults.AllPermissionsGranted())
                {
                    // Permissions granted, nothing to do.
                    // Carry on and let the MapFragment do it's own thing.
                    InitializeUiSettingsOnMap();
                }
                else
                {
                    // Permissions not granted!
                    Log.Info(TAG, "The app does not have location permissions");

                    var layout = FindViewById(Android.Resource.Id.Content);
                    Snackbar.Make(layout, Resource.String.location_permission_missing, Snackbar.LengthLong).Show();
                    Finish();
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }



        private BitmapDescriptor MakeCircle(int d)
        {
            // draw circle
            Bitmap bm = Bitmap.CreateBitmap(d, d, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bm);
            Paint p = new Paint();
            p.SetARGB(126, 255, 0, 0);
            c.DrawCircle(d / 2, d / 2, d / 2, p);
            return BitmapDescriptorFactory.FromBitmap(bm);
        }

        public static int DpToPx(int dp)
        {
            return (int)(dp * Resources.System.DisplayMetrics.Density);
        }

        public static int pxToDp(int px)
        {
            return (int)(px / Resources.System.DisplayMetrics.Density);
        }
        public int CirclePxSize(int cases)
        {
            int size = 5;

            if (cases <= 0)
                size = 1;
            else if (1 <= cases && cases < 5)
                size = 5;
            else if (5 <= cases && cases < 10)
                size = 10;
            else if (10 <= cases && cases < 25)
                size = 15;
            else if (25 <= cases && cases < 100)
                size = 20;
            else if (100 <= cases && cases < 200)
                size = 25;
            else if (200 <= cases && cases < 500)
                size = 30;
            else if (500 <= cases && cases < 1000)
                size = 35;
            else if (1000 <= cases && cases < 2000)
                size = 40;
            else if (2000 <= cases && cases < 5000)
                size = 45;
            else if (5000 <= cases && cases < 20000)
                size = 50;
            else if (20000 >= cases)
                size = 60;
            else
                size = 10;

            return size;
        }

    }


}
