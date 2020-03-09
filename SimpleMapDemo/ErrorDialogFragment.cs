using Android.App;
using Android.OS;

namespace CoronaVirusTracker
{
    class ErrorDialogFragment : DialogFragment
    {
        public ErrorDialogFragment(Dialog dialog)
        {
            Dialog = dialog;
        }

        public new Dialog Dialog { get; }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return Dialog;
        }
    }
}
