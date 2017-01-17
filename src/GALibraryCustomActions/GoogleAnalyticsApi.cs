using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

namespace GALibraryCustomActions
{
  // https://gist.github.com/0liver/11229128

  /* based on the docs at: https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide */
  /*
   * LICENSE: MIT
   * AUTOHR: oliver@teamaton.com
   */

  public class GoogleAnalyticsApi
  {
    public static void TrackEvent(string tid, string category, string action, string label, int? value = null)
    {
      Track(tid, HitType.@event, category, action, label, value);
    }

    public static void TrackPageview(string tid, string category, string action, string label, int? value = null)
    {
      Track(tid, HitType.@pageview, category, action, label, value);
    }

    private static void Track(string tid, HitType type, string category, string action, string label,
                              int? value = null)
    {
      if (string.IsNullOrEmpty(category)) throw new ArgumentNullException("category");
      if (string.IsNullOrEmpty(action)) throw new ArgumentNullException("action");

      var request = (HttpWebRequest)WebRequest.Create("http://www.google-analytics.com/collect");
      request.Method = "POST";

      // the request body we want to send
      var postData = new Dictionary<string, string>
                           {
                               { "v", "1" },
                               { "tid", tid },
                               { "cid", "555" },
                               { "t", type.ToString() },
                               { "ec", category },
                               { "ea", action },
                           };
      if (!string.IsNullOrEmpty(label))
      {
        postData.Add("el", label);
      }
      if (value.HasValue)
      {
        postData.Add("ev", value.ToString());
      }

      var postDataString = postData
          .Aggregate("", (data, next) => string.Format("{0}&{1}={2}", data, next.Key,
                                                       HttpUtility.UrlEncode(next.Value)))
          .TrimEnd('&');

      // set the Content-Length header to the correct value
      request.ContentLength = Encoding.UTF8.GetByteCount(postDataString);

      // write the request body to the request
      using (var writer = new StreamWriter(request.GetRequestStream()))
      {
        writer.Write(postDataString);
      }

      try
      {
        var webResponse = (HttpWebResponse)request.GetResponse();
        if (webResponse.StatusCode != HttpStatusCode.OK)
        {
          throw new HttpException((int)webResponse.StatusCode,
                                  "Google Analytics tracking did not return OK 200");
        }
      }
      catch (Exception ex)
      {
        // do what you like here, we log to Elmah
        // ElmahLog.LogError(ex, "Google Analytics tracking failed");
      }
    }

    private enum HitType
    {
      // ReSharper disable InconsistentNaming
      @event,
      @pageview,
      // ReSharper restore InconsistentNaming
    }
  }
}