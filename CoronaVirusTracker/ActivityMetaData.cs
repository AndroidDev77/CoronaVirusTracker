using Android.App;
using Android.Content;

namespace CoronaVirusTracker
{
    using System;

    /// <summary>
    ///     This class holds meta-data about the various activities that are used in this application.
    /// </summary>
    class ActivityMetaData
    {
        public ActivityMetaData(int titleResourceId, int descriptionId, Type activityToLaunch)
        {
            ActivityToLaunch = activityToLaunch;
            TitleResource = titleResourceId;
            DescriptionResource = descriptionId;
        }

        public Type ActivityToLaunch { get; }
        public int DescriptionResource { get; }
        public int TitleResource { get; }

        public void Start(Activity context)
        {
            var i = new Intent(context, ActivityToLaunch);
            context.StartActivity(i);
        }
    }
}
