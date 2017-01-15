using Microsoft.Tools.WindowsInstallerXml;
using System.Reflection;

namespace WixGAExtension
{
  public class GAExtension : WixExtension
  {
    private CompilerExtension compilerExtension;

    public override CompilerExtension CompilerExtension
    {
      get
      {
        if (this.compilerExtension == null)
        {
          this.compilerExtension =
             new GACompiler();
        }

        return this.compilerExtension;
      }
    }

    private Library library;

    public override Library GetLibrary(
       TableDefinitionCollection tableDefinitions)
    {
      if (this.library == null)
      {
        this.library =
           WixExtension.LoadLibraryHelper(
              Assembly.GetExecutingAssembly(),
              "WixGAExtension.GALibrary.wixlib",
               tableDefinitions);
      }

      return this.library;
    }

    private TableDefinitionCollection tableDefinitions;

    public override TableDefinitionCollection TableDefinitions
    {
      get
      {
        if (this.tableDefinitions == null)
        {
          this.tableDefinitions =
             WixExtension.LoadTableDefinitionHelper(
                Assembly.GetExecutingAssembly(),
                "WixGAExtension.TableDefinitions.xml");
        }

        return this.tableDefinitions;
      }
    }
  }
}