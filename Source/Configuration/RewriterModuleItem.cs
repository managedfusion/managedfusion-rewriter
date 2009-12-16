using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Compilation;
using System.Configuration;

namespace ManagedFusion.Rewriter.Configuration
{
	public partial class RewriterModuleItem
	{
		protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
		{
			reader.MoveToContent();

			string name = reader.GetAttribute(NamePropertyName);
			string type = reader.GetAttribute(TypePropertyName);

			if (String.IsNullOrEmpty(name))
				throw new ConfigurationErrorsException("The name attribute is required.", reader);

			if (String.IsNullOrEmpty(type))
				throw new ConfigurationErrorsException("The type attribute is required.", reader);

			Type foundType;

			try
			{
				foundType = BuildManager.GetType(type, true, true);
			}
			catch (Exception exc)
			{
				throw new ConfigurationErrorsException("The type was not able to be found.", exc, reader);
			}

			base[NamePropertyName] = name;
			base[TypePropertyName] = foundType;
		}
	}
}