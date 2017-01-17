using System;
using System.Collections.Generic;
using Microsoft.Deployment.WindowsInstaller;

namespace GALibraryCustomActions
{
  public class CustomActions
  {
    #region Installation

    [CustomAction]
    public static ActionResult TrackInstallationImmediate(Session session)
    {
      Database db = session.Database;

      try
      {
        View view = db.OpenView("SELECT `TrackingId` FROM `GoogleAnalytics`");
        view.Execute();

        CustomActionData data = new CustomActionData();

        var productVersion = db.ExecutePropertyQuery("ProductVersion");
        data["ProductVersion"] = productVersion;

        foreach (Record row in view)
        {
          data["TrackingId"] = row["TrackingId"].ToString();
        }

        session["TrackInstallationDeferred"] = data.ToString();

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log(ex.Message);
        return ActionResult.Failure;
      }
      finally
      {
        db.Close();
      }
    }

    [CustomAction]
    public static ActionResult TrackInstallationDeferred(Session session)
    {
      try
      {
        CustomActionData data = session.CustomActionData;

        var productVersion = data["ProductVersion"]; 

        foreach (KeyValuePair<string, string> datum in data)
        {
          if (datum.Value.ToUpper().StartsWith("UA-"))
          {
            try
            {
              GoogleAnalyticsApi.TrackEvent(datum.Value, "WixGAExtension", "Install", productVersion);
            }
            catch (Exception ex)
            {
              session.Log("GoogleAnalytics tracking error: " + ex.Message);
            }
          }
        }

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log(ex.Message);
        return ActionResult.Failure;
      }
    }

    #endregion

    #region Uninstallation

    [CustomAction]
    public static ActionResult TrackUninstallationImmediate(Session session)
    {
      Database db = session.Database;

      try
      {
        View view = db.OpenView("SELECT `TrackingId` FROM `GoogleAnalytics`");
        view.Execute();

        CustomActionData data = new CustomActionData();

        var productVersion = db.ExecutePropertyQuery("ProductVersion");
        data["ProductVersion"] = productVersion;

        foreach (Record row in view)
        {
          data["TrackingId"] = row["TrackingId"].ToString();
        }

        session["TrackUninstallationDeferred"] = data.ToString();

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log(ex.Message);
        return ActionResult.Failure;
      }
      finally
      {
        db.Close();
      }
    }

    [CustomAction]
    public static ActionResult TrackUninstallationDeferred(Session session)
    {
      try
      {
        CustomActionData data = session.CustomActionData;

        var productVersion = data["ProductVersion"];

        foreach (KeyValuePair<string, string> datum in data)
        {
          if (datum.Value.ToUpper().StartsWith("UA-"))
          {
            try
            {
              GoogleAnalyticsApi.TrackEvent(datum.Value, "WixGAExtension", "Uninstall", productVersion);
            }
            catch (Exception ex)
            {
              session.Log("GoogleAnalytics tracking error: " + ex.Message);
            }
          }
        }

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log(ex.Message);
        return ActionResult.Failure;
      }
    }

    #endregion
  }
}