// 
// System.Web.Services.Description.ServiceDescriptionImporter.cs
//
// Author:
//   Tim Coleman (tim@timcoleman.com)
//   Lluis Sanchez Gual (lluis@ximian.com)
//
// Copyright (C) Tim Coleman, 2002
//

using System;
using System.CodeDom;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Services.Description;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.Configuration;

namespace System.Web.Services.Description {
	public class ServiceDescriptionImporter {

		#region Fields

		string protocolName;
		XmlSchemas schemas;
		ServiceDescriptionCollection serviceDescriptions;
		ServiceDescriptionImportStyle style;

		ArrayList importInfo = new ArrayList ();
		

		#endregion // Fields

		#region Constructors
	
		public ServiceDescriptionImporter ()
		{
			protocolName = String.Empty;
			schemas = new XmlSchemas ();
			serviceDescriptions = new ServiceDescriptionCollection ();
			style = ServiceDescriptionImportStyle.Client;
		}
		
		#endregion // Constructors

		#region Properties

		public string ProtocolName {
			get { return protocolName; }
			set { protocolName = value; }
		}

		public XmlSchemas Schemas {
			get { return schemas; }
		}

		public ServiceDescriptionCollection ServiceDescriptions {
			get { return serviceDescriptions; }
		}

		public ServiceDescriptionImportStyle Style {
			get { return style; }
			set { style = value; }
		}
	
		#endregion // Properties

		#region Methods

		public void AddServiceDescription (ServiceDescription serviceDescription, string appSettingUrlKey, string appSettingBaseUrl)
		{
			if (appSettingUrlKey != null && appSettingUrlKey == string.Empty && style == ServiceDescriptionImportStyle.Server)
				throw new InvalidOperationException ("Cannot set appSettingUrlKey if Style is Server");

			ImportInfo info = new ImportInfo ();
			info.ServiceDescription = serviceDescription;
			info.AppSettingUrlKey = appSettingUrlKey;
			info.AppSettingBaseUrl = appSettingBaseUrl;
			importInfo.Add (info);
			serviceDescriptions.Add (serviceDescription);
			
			if (serviceDescription.Types != null)
				schemas.Add (serviceDescription.Types.Schemas);
		}

		public ServiceDescriptionImportWarnings Import (CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
		{
			ProtocolImporter importer = GetImporter ();
			importer.Import (this, codeNamespace, codeCompileUnit, importInfo);
			
			return importer.Warnings;
		}
		
		ProtocolImporter GetImporter ()
		{
			switch ((protocolName != null && protocolName != "") ? protocolName : "Soap")
			{
				case "Soap": return new SoapProtocolImporter ();
				default: throw new NotSupportedException ();
			}
		}
#endregion
	}

	internal class ImportInfo
	{
		public ServiceDescription ServiceDescription;
		public string AppSettingUrlKey;
		public string AppSettingBaseUrl;
	}

}
