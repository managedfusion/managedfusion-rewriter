****************************************************************************
                      MANAGED FUSION URL REWRITER
****************************************************************************

1. Getting Started
2. Available Rules
3. Enabling wildcards in IIS 5/6
4. Support

****************************************************************************
 REQUIREMENTS
****************************************************************************

1. This has been tested under Windows 2003 and Windows 2008, all others are untested.
2. .NET 2.0 SP1 or greater be installed on the machine.

****************************************************************************
 1. Getting Started
****************************************************************************

1. To get started with Managed Fusion Url Rewriter you need to integrate the following configuration settings
   in to your web.config file.

	<!-- Integrate the following in to the <configuration> tag -->
	<configSections>
		<section name="managedFusion.rewriter" type="ManagedFusion.Rewriter.Configuration.ManagedFusionRewriterSectionGroup"/>
	</configSections>
	<managedFusion.rewriter xmlns="http://managedfusion.com/xsd/managedFusion/rewriter">
		<!--
		This is just a minimal sample configuration file that shows how to declare
		the configuration sections.
		
		Because an XML Schema Definition (XSD) is generated for each configuration
		section, it should be trivial to edit these files because you have
		IntelliSense on the XML definition.
		-->
	</managedFusion.rewriter>
	
	For IIS 5/6:
	
	<!-- Integrate the following in to the <system.web>/<httpModules> tag -->
	<system.web>
		<httpModules>
			<add name="RewriterModule" type="ManagedFusion.Rewriter.RewriterModule, ManagedFusion.Rewriter"/>
		</httpModules>
	</system.web>
	
	For IIS 7:
	
	<!-- Integrate the following in to the <system.webServer> tag -->
	<system.webServer>
		<validation validateIntegratedModeConfiguration="false"/>
		<modules runAllManagedModulesForAllRequests="true">
			<add name="RewriterModule" type="ManagedFusion.Rewriter.RewriterModule, ManagedFusion.Rewriter"/>
		</modules>
		<handlers>
			<add name="RewriterProxyHandler" preCondition="integratedMode" verb="*" path="RewriterProxy.axd" type="System.Web.HttpForbiddenHandler, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"/>
		</handlers>
	</system.webServer>
	
2. And add the following files to the /bin directory in your web application.

   a. ManagedFusion.Rewriter.dll
   b. ManagedFusion.Rewriter.pdb
  
3. And create a rules file named 'ManagedFusion.Rewriter.txt' (which is just a plain text file that can be done 
   with Notepad)  To get the file started add the following as a test or see some of the sample applications:

   RewriteEngine On
   RewriteRule ^/(.*)      http://google.com [R=302]

   ** If you are converting your Apache rules over from .htaccess then you just need to copy everything
   between the <IfModule mod_rewrite.c> ... </IfModule> tags in your .htaccess file.
   
4. If you are using IIS 6 please make sure you read the instructions in Part 3.  This step is very important, and if 
   you are using IIS 6 and you forget to do it, nothing will work.

****************************************************************************
 2. Available Rules
****************************************************************************

All the following rules defined at http://httpd.apache.org/docs/2.0/mod/mod_rewrite.html are supported.

1. RewriteBase
2. RewriteCond
3. RewriteEngine
4. RewriteRule
5. RewriteLog
6. RewriteLogLevel

****************************************************************************
 3. Enabling wildcards in IIS 5/6
****************************************************************************

If you are using IIS 5 or 6 then you will want to enable wildcards to gain the full functionality of the 
Managed Fusion Url Rewriter, you can do so by adding a new application mapping to your websites IIS settings.  
You should note that if you use the server built in to Visual Studio or you use IIS 7 you do not need to follow 
these directions.

** This solution works only if your website is using ASP.NET server pages and not mixing with other dynamic server pages such as ASP and PHP.

The following instructions apply for IIS 5.

1. Open IIS and right-click on the website and select 'properties'.
2. Click the 'Configuration' button under Application Settings section
3. Click the 'Add' button to create a new application mapping
4. Set the executable textbox to aspnet_isapi.dll file location.
    for .net 2.0, 3.0, 3.5: C:\Windows\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll
5. Set the extension textbox to .*  to map extension-less URLs and custom extensions to the ASP.NET ISAPI Process.
6. Make sure the checkbox 'Check that file exists' is not checked.
7. Press 'OK' to confirm and close all the windows. 

The following instructions apply for IIS 6.

1. Open IIS and right-click on the website and select 'properties'.
2. Click the 'Configuration' button under Application Settings section
3. Click the 'Insert...' button to create a new wildcard mapping
4. Set the executable textbox to aspnet_isapi.dll file location.
    for .net 2.0, 3.0, 3.5: C:\Windows\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll
5. Make sure the checkbox 'Verify that file exists' is not checked.
6. Press 'OK' to confirm and close all the windows. 

****************************************************************************
 4. Support
****************************************************************************

If you have any questions or comments please e-mail them to support@managedfusion.com, but before you do so please
make sure to create a log of what is going on with your rules you can do that by adding the following right below
the RewriteEngine statement:

RewriteLog "log.txt"
RewriteLogLevel 9

After you do this a log.txt file will be produced in the root of your web application that will log any request the
rewriter services.