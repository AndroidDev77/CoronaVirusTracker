using System;

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
using Android.Util;
using Android.Gms.Common;
using Android.Views;
using Android.Text;
using Android.Text.Style;

namespace CoronaVirusTracker
{
    
    [Activity(Label = "@string/activity_label_mapwithmarkers", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme = "@style/AppTheme")]
    public class MapWithMarkersActivity : AppCompatActivity, IOnMapReadyCallback
    {
        public static readonly int RC_INSTALL_GOOGLE_PLAY_SERVICES = 1000;
        public static readonly string TAG = "CoronaVirusTracker";
        public static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;
        bool IsGooglePlayServicesInstalled;
        GoogleMap googleMap;
        SeekBar TimeBar;
        TextView TimeText;
        TextView CaseText;

        Button PrevButton;
        Button NextButton;

        InfectionData InfectionData;
        int numCases = 0;
        int timeIndex = 5;
        //GeoJsonLayer layer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MapLayout);
            PrevButton = FindViewById<Button>(Resource.Id.prevButton);
            NextButton = FindViewById<Button>(Resource.Id.nextButton);

            PrevButton.Click += new EventHandler(PrevButtonClicked);
            NextButton.Click += new EventHandler(NextButtonClicked);
           
            TimeText = FindViewById<TextView>(Resource.Id.timeText);
            CaseText = FindViewById<TextView>(Resource.Id.casesText);
            TimeBar = FindViewById<SeekBar>(Resource.Id.timeBar);
            
            TimeText.BringToFront();
            TimeBar.BringToFront();
            TimeBar.ProgressChanged += new EventHandler<SeekBar.ProgressChangedEventArgs>(TimeBarProgressChanged);

            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

        }
        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.mapStyle));
            
            AddMarkersToMap();

            googleMap.SetInfoWindowAdapter(new CustomInfoWindowAdapter(this));

            if (this.PerformRuntimePermissionCheckForLocation(REQUEST_PERMISSIONS_LOCATION))
            {
                InitializeUiSettingsOnMap(); 
            }
        }
        void InitializeUiSettingsOnMap()
        {
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = false;
            googleMap.UiSettings.MapToolbarEnabled = false;
            googleMap.MyLocationEnabled = true;
            

        }
        protected void PrevButtonClicked(object sender, EventArgs e)
        {
            int progress = TimeBar.Progress - 1;
            progress = Math.Max(Math.Min(progress, 100), 0);
            TimeBar.SetProgress(progress, true);
        }
        protected void NextButtonClicked(object sender, EventArgs e)
        {
            int progress = TimeBar.Progress + 1;
            progress = Math.Max(Math.Min(progress, 100), 0);
            TimeBar.SetProgress(progress, true);
        }

        protected void TimeBarProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            timeIndex = Remap(e.Progress, 0, 100, 5, InfectionData.Count-1);
            TimeText.Text = InfectionData.Dates[timeIndex].ToString();
            UpdateIcons();
        }

        protected void UpdateIcons()
        {

            googleMap.Clear();
            numCases = 0;
            foreach (Infection aInfection in InfectionData.Infections)
            {
                int count = aInfection.InfectionCountList.Count - 1;
                int comfirmedcases = 0;
                int deathcases = 0;
                int recoveredcases = 0;

                if (count >= timeIndex && timeIndex > 3)
                {
                    comfirmedcases = aInfection.InfectionCountList[timeIndex].Item2.ComfirmedCases;
                    deathcases = aInfection.InfectionCountList[timeIndex].Item2.DeathCases;
                    recoveredcases = aInfection.InfectionCountList[timeIndex].Item2.RecoveredCases;
                }
                else
                {
                    comfirmedcases = aInfection.InfectionCountList[count].Item2.ComfirmedCases;
                    deathcases = aInfection.InfectionCountList[count].Item2.DeathCases;
                    recoveredcases = aInfection.InfectionCountList[count].Item2.RecoveredCases;
                }

                if (comfirmedcases != 0)
                {
                    numCases += comfirmedcases;
                    MarkerOptions markerOptions = aInfection.MarkerOption.SetIcon(MakeCircle(comfirmedcases))
                                                                         .SetSnippet("Comfirmed: " + comfirmedcases + "\n"+
                                                                                   "Deaths: " + deathcases + "\n" +
                                                                                   "Recovered: " + recoveredcases + "\n" +
                                                                                   "Active: " + (comfirmedcases - recoveredcases - deathcases));
                    googleMap.AddMarker(markerOptions);
                }
            }

            CaseText.Text = "Comfirmed Cases: " + numCases;

        }

        public int Remap(int value, int fromMin, int fromMax, int toMin, float toMax)
        {
            float fromAbs = value - fromMin;
            float fromMaxAbs = fromMax - fromMin;

            float normal = fromAbs / fromMaxAbs;

            float toMaxAbs = toMax - toMin;
            float toAbs = toMaxAbs * normal;

            int to = (int)(toAbs + toMin);

            return to;
        }

        void AddMarkersToMap()
        {
            InfectionData = MarkerMaker.CreateMarkers(GetString(Resource.String.comfirmedCasesURL), GetString(Resource.String.deathCasesURL), GetString(Resource.String.recoveredCasesURL));

            numCases = 0;
            foreach(Infection aInfection in InfectionData.Infections)
            {
                int count = aInfection.InfectionCountList.Count-1;
                int cases = aInfection.InfectionCountList[count].Item2.ComfirmedCases;
                numCases += cases;
                MarkerOptions markerOptions = aInfection.MarkerOption.SetIcon(MakeCircle(cases));
                googleMap.AddMarker(markerOptions);
                
            }
            CaseText.Text = "Active Cases: " +numCases;
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (RC_INSTALL_GOOGLE_PLAY_SERVICES == requestCode && resultCode == Result.Ok)
            {
                IsGooglePlayServicesInstalled = true;
            }
            else
            {
                Log.Warn(TAG, $"Don't know how to handle resultCode {resultCode} for request {requestCode}.");
            }
        }

        bool TestIfGooglePlayServicesIsInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(TAG, "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(TAG, "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
            }

            return false;
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
            d = DpToPx(CirclePxSize(d));
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
            else if (20000 >= cases && cases < 50000)
                size = 60;
            else if (20000 >= cases && cases < 50000)
                size = 80;
            else
                size = 10;

            return size;
        }

        /** Demonstrates customizing the info window and/or its contents. */
        class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
        {
            MapWithMarkersActivity parent;
            private readonly RadioGroup mOptions;

            // These a both viewgroups containing an ImageView with id "badge" and two TextViews with id
            // "title" and "snippet".
            private readonly View mWindow;
            private readonly View mContents;

            internal CustomInfoWindowAdapter(MapWithMarkersActivity parent)
            {
                this.parent = parent;
                mWindow = parent.LayoutInflater.Inflate(Resource.Layout.custom_info_window, null);
                mContents = parent.LayoutInflater.Inflate(Resource.Layout.custom_info_contents, null);
            }

            public View GetInfoWindow(Marker marker)
            {
                Render(marker, mWindow);
                return mWindow;
            }

            public View GetInfoContents(Marker marker)
            {
                Render(marker, mContents);
                return mContents;
            }

            private void Render(Marker marker, View view)
            {

                String title = marker.Title;
                TextView titleUi = ((TextView)view.FindViewById(Resource.Id.title));
                if (title != null)
                {
                    // Spannable string allows us to edit the formatting of the text.
                    SpannableString titleText = new SpannableString(title);
                    SpanTypes st = (SpanTypes)0;
                    titleText.SetSpan(new ForegroundColorSpan(parent.Resources.GetColor(Resource.Color.colorAccent)), 0, titleText.Length(), st);
                    titleUi.TextFormatted = (titleText);
                }
                else
                {
                    titleUi.Text = ("");
                }

                String snippet = marker.Snippet;
                TextView snippetUi = ((TextView)view.FindViewById(Resource.Id.snippet));
                if (snippet != null)
                {
                    SpannableString snippetText = new SpannableString(snippet);
                    snippetUi.TextFormatted = (snippetText);
                }
                else
                {
                    snippetUi.Text = ("");
                }
            }
        }

    }


}
