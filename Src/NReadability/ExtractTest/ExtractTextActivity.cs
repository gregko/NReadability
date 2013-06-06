using System;
using System.IO;
using System.Net;
using Android.App;
using Android.Util;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using NReadability;

namespace ExtractTest
{
    [Activity(Label = "ExtractTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class ExtractTextActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            String strOrig =
                "http://www.npr.org/blogs/codeswitch/2013/06/05/188914328/the-first-lady-a-heckler-and-public-dissent";
            String strClean = GetPageClean(strOrig);
            var wbc = FindViewById<WebView>(Resource.Id.webView1);
            wbc.LoadData(strClean, "text/html; charset=UTF-8", "UTF-8");

            var actv = FindViewById<EditText>(Resource.Id.textView1);
            actv.Text = strOrig;
            actv.EditorAction += actv_EditorAction;
        }

        void actv_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.ImeNull && e.Event != null && e.Event.UnicodeChar == 10) // Enter pressed
            {
                Log.Debug("ExtractText", e.ToString());
                var actv = (EditText)sender;
                var wbc = FindViewById<WebView>(Resource.Id.webView1);
                wbc.LoadData(GetPageClean(actv.Text), "text/html; charset=UTF-8", "UTF-8");
            }
        }

        private String GetPageClean(String sUrl)
        {
            var client = new WebClient();


            // Add a user agent header in case the 
            // requested URI contains a query.
            // client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = client.OpenRead(sUrl);
            var reader = new StreamReader(data);
            string strRaw = reader.ReadToEnd();
            data.Close();
            reader.Close();

            var nReadabilityTranscoder = new NReadabilityTranscoder();
            bool mainContentExtracted;

            String strClean = nReadabilityTranscoder.Transcode(strRaw, out mainContentExtracted);
            return strClean;
        }
    }

}

