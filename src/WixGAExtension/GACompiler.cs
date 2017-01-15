using Microsoft.Tools.WindowsInstallerXml;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace WixGAExtension
{
  public class GACompiler : CompilerExtension
  {
    private XmlSchema schema;

    public GACompiler()
    {
      this.schema =
         CompilerExtension.LoadXmlSchemaHelper(
            Assembly.GetExecutingAssembly(),
            "WixGAExtension.ExtensionSchema.xsd");
    }

    public override XmlSchema Schema
    {
      get
      {
        return this.schema;
      }
    }

    public override void ParseElement(
       SourceLineNumberCollection sourceLineNumbers,
       XmlElement parentElement,
       XmlElement element,
       params string[] contextValues)
    {
      switch (parentElement.LocalName)
      {
        case "Product":
        case "Fragment":
          switch (element.LocalName)
          {
            case "GoogleAnalytics":
              this.ParseGoogleAnalytics(element);
              break;
            default:
              this.Core.UnexpectedElement(
                 parentElement,
                 element);
              break;
          }
          break;
        default:
          this.Core.UnexpectedElement(
             parentElement,
             element);
          break;
      }
    }

    private void ParseGoogleAnalytics(XmlNode node)
    {
      SourceLineNumberCollection sourceLineNumber =
         Preprocessor.GetSourceLineNumbers(node);

      string googleAnalyticsId = null;

      foreach (XmlAttribute attribute in node.Attributes)
      {
        if (attribute.NamespaceURI.Length == 0 ||
            attribute.NamespaceURI == this.schema.TargetNamespace)
        {
          switch (attribute.LocalName)
          {
            case "Id":
              googleAnalyticsId = this.Core.GetAttributeIdentifierValue(
                 sourceLineNumber,
                 attribute);
              break;
            default:
              this.Core.UnexpectedAttribute(
                 sourceLineNumber,
                 attribute);
              break;
          }
        }
        else
        {
          this.Core.UnsupportedExtensionAttribute(
             sourceLineNumber,
             attribute);
        }
      }

      if (string.IsNullOrEmpty(googleAnalyticsId))
      {
        this.Core.OnMessage(
           WixErrors.ExpectedAttribute(
              sourceLineNumber,
              node.Name,
              "Id"));
      }

      if (!this.Core.EncounteredError)
      {
        Row elementRow =
             this.Core.CreateRow(
                sourceLineNumber,
                "GoogleAnalyticsTable");

        elementRow[0] = googleAnalyticsId;

        this.Core.CreateWixSimpleReferenceRow(
           sourceLineNumber,
           "CustomAction",
           "TrackInstallationImmediate");
      }
    }
  }
}