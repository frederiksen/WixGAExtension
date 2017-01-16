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
        View view = db.OpenView("SELECT `Id`, `Tracking` FROM `GoogleAnalyticsTable`");
        view.Execute();

        CustomActionData data = new CustomActionData();

        foreach (Record row in view)
        {
          data[row["Id"].ToString()] = row["Tracking"].ToString();
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

        foreach (KeyValuePair<string, string> datum in data)
        {
          DisplayWarningMessage(
             session,
             string.Format("Install {0} => {1}", datum.Key, datum.Value));
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
        View view = db.OpenView("SELECT `Id`, `Tracking` FROM `GoogleAnalyticsTable`");
        view.Execute();

        CustomActionData data = new CustomActionData();

        foreach (Record row in view)
        {
          data[row["Id"].ToString()] = row["Tracking"].ToString();
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

        foreach (KeyValuePair<string, string> datum in data)
        {
          DisplayWarningMessage(
             session,
             string.Format("Uninstall {0} => {1}", datum.Key, datum.Value));
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

    #region Helpers

    private static void DisplayWarningMessage(Session session, string message)
    {
      Record record = new Record(0);
      record[0] = message;
      session.Message(InstallMessage.Warning, record);
    }

    #endregion
  }
}