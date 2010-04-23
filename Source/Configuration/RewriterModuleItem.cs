/** 
 * Copyright (C) 2007-2010 Nicholas Berardi, Managed Fusion, LLC (nick@managedfusion.com)
 * 
 * <author>Nicholas Berardi</author>
 * <author_email>nick@managedfusion.com</author_email>
 * <company>Managed Fusion, LLC</company>
 * <product>Url Rewriter and Reverse Proxy</product>
 * <license>Microsoft Public License (Ms-PL)</license>
 * <agreement>
 * This software, as defined above in <product />, is copyrighted by the <author /> and the <company />, all defined above.
 * 
 * For all binary distributions the <product /> is licensed for use under <license />.
 * For all source distributions please contact the <author /> at <author_email /> for a commercial license.
 * 
 * This copyright notice may not be removed and if this <product /> or any parts of it are used any other
 * packaged software, attribution needs to be given to the author, <author />.  This can be in the form of a textual
 * message at program startup or in documentation (online or textual) provided with the packaged software.
 * </agreement>
 * <product_url>http://www.managedfusion.com/products/url-rewriter/</product_url>
 * <license_url>http://www.managedfusion.com/products/url-rewriter/license.aspx</license_url>
 */

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