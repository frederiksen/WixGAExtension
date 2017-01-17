using Microsoft.Tools.WindowsInstallerXml;
using System;
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
            case "TrackingId":
              googleAnalyticsId = this.Core.GetAttributeValue(
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
              "Tracking"));
      }

      if (!this.Core.EncounteredError)
      {
        Row elementRow =
             this.Core.CreateRow(
                sourceLineNumber,
                "GoogleAnalytics");

        elementRow[0] = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        elementRow[1] = googleAnalyticsId;

        this.Core.CreateWixSimpleReferenceRow(
           sourceLineNumber,
           "CustomAction",
           "TrackInstallationImmediate");
      }
    }
  }
}