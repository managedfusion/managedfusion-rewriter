Managed Fusion URL Rewriter and Reverse Proxy
================================

* **NuGet:** http://nuget.org/packages/ManagedFusion.Rewriter
* **Source:** https://github.com/managedfusion/managedfusion-rewriter
* **Documentation:** https://github.com/managedfusion/managedfusion-rewriter/wiki
* **Discussion:** http://urlrewriter.codeplex.com/discussions
* **Issues:** https://github.com/managedfusion/managedfusion-rewriter-proxy/issues
* **Contrib Project:** https://github.com/managedfusion/managedfusion-rewriter-contrib

If you find some value in this project, and it has saved you time or money (lets be honest time is money), please consider donating a fraction of what you have saved to help fund the free development put forth by our developers. You can donate by sending money via PayPal to nberardi@gmail.com

About
---------------------------------

Managed Fusion URL Rewriter is a powerful URL manipulation engine based on the {"Apache mod_rewrite"} extension.  It is designed, from the ground up to bring all the features of {"Apache mod_rewrite"} to IIS 6.0 and IIS 7.0.  Managed Fusion Url Rewriter works with ASP.NET on Microsoft's Internet Information Server (IIS) 6.0 and Mono XPS Server and is fully supported, for all languages, in IIS 7.0, including ASP.NET and PHP.  Managed Fusion Url Rewriter gives you the freedom to go beyond the standard URL schemes and develop your own scheme.

URL Rewriter provides web site owners with the ability to replace URL paths and querystring into links that your users can easily remember without the need for bookmarks. As an added benefit your web site becomes search engine friendly, which means higher page ranking from the search engines, and should result in better lead generation.

**Advantages**

* Developed by my company but unlimited *FREE use for everybody*.
* Full .NET 2.0, 3.0, and 3.5 support.
* Full support for IIS 6.0 and IIS 7.0 (including integrated pipelines).
* Fully functional Proxy and Reverse Proxy integrated in at no extra cost.
* Full support for Mono XPS, and the integrated Visual Studio Web Development Server, two things that ISAPI_Rewrite and Ionic Rewriter cannot claim.
* Create short URLs that are easy for your users to remember.
* Structure your site for search engine readability.
* Hide the implementation of your URL from the URL that is displayed to the user.
* Provides easy implementation for standardizing your web site to help in SEO efforts.
* Block hot linking of your sites content, very useful for sites that steal your images and consume your bandwidth for their gain.
* Proxy content of one site into directory on another site.
* Create a gateway server that brings together all your companies proprietary web application under one standardized schema through the proxy feature.
* Create dynamic host-header based sites using a single physical site.
* Change your ASP.NET file extensions to other extensions (i.e. .html). This also helps in migrating old CGI technology to ASP.NET, without changing your URL structure.
* Return a browser-dependent content even for static files.
* Reverse proxy content from behind your firewall on to the internet, without exposing your machine to the internet.
* Ability to use the Reverse Proxy to make cross site AJAX requests.

**How does it work?**

Managed Fusion Url Rewriter is an HttpModule for the Microsoft ASP.NET Web Framework, or IIS 7.0 Integrated Pipeline. All rules are managed in a plain text using Apache mod_rewrite syntax. 

These rules provide the web request a way to validate how the requesting friendly URL should be handled. Managed Fusion Url Rewriter does its best to find a matching rule for the friendly URL, if found, the request is rewritten to something your application understands or an HTTP redirect, to a different url, is sent back to the client depending on the actions given to the rule.

The result is a friendly and clean URL that completely masks your file structure from the client.

*Unlike other existing URL rewriting process, Managed Fusion Url Rewrite is entirely masking the old URL so ASP.NET Form Postbacks are fully supported by this component and it uses the common Apache mod_rewrite syntax that is so popular with PHP, Cold Fusion, and Ruby.*

Featured at PDC 2008
====================================

<iframe width="640" height="360" src="https://www.youtube.com/embed/38Bt_0ufH1E" frameborder="0" allowfullscreen></iframe>

****************************************************************************

Setup
================================

1. Requirements
2. Getting Started
3. Available Rules
4. Enabling wildcards in IIS 6
5. Support

Requirements
---------------------------------------

1. This has been tested under Windows 2003 and Windows 2008, all others are untested.
2. .NET 3.5 or Mono 2.8 or greater must be installed on the machine.

Getting Started
----------------------------------------------

1. To get started with Managed Fusion Url Rewriter you need to integrate the following configuration settings in to your web.config file.

**Create web.config**

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
	
**For IIS 6:**
	
    <!-- Integrate the following in to the <system.web>/<httpModules> tag -->
    <system.web>
        <httpModules>
            <add name="RewriterModule" type="ManagedFusion.Rewriter.RewriterModule, ManagedFusion.Rewriter"/>
        </httpModules>
    </system.web>
	
**For IIS 7:**
	
    <!-- Integrate the following in to the <system.webServer> tag -->
    <system.webServer>
        <modules runAllManagedModulesForAllRequests="true">
            <add name="RewriterModule" type="ManagedFusion.Rewriter.RewriterModule, ManagedFusion.Rewriter"/>
        </modules>
    </system.webServer>
	
2. And add the following files to the /bin directory in your web application.

   * ManagedFusion.Rewriter.dll
  
3. And create a rules file named 'ManagedFusion.Rewriter.txt' (which is just a plain text file that can be done with Notepad)  To get the file started add the following as a test or see some of the sample applications:

    RewriteEngine On
    RewriteRule ^/(.*)      http://google.com [R=302]

*If you are converting your Apache rules over from .htaccess then you just need to copy everything between the <IfModule mod_rewrite.c> ... </IfModule> tags in your .htaccess file.*
   
4. If you are using IIS 6 please make sure you read the instructions in Part 3.  This step is very important, and if you are using IIS 6 and you forget to do it, nothing will work.

Available Rules
--------------------------------------

All the following rules defined at http://httpd.apache.org/docs/2.0/mod/mod_rewrite.html are supported.

1. RewriteBase
2. RewriteCond
3. RewriteEngine
4. RewriteRule
5. RewriteLog
6. RewriteLogLevel

Enabling wildcards in IIS 6
-----------------------------------

If you are using IIS 6 then you will want to enable wildcards to gain the full functionality of the 
Managed Fusion Url Rewriter, you can do so by adding a new application mapping to your websites IIS settings.  
You should note that if you use the server built in to Visual Studio or you use IIS 7 you do not need to follow 
these directions.

** This solution works only if your website is using ASP.NET server pages and not mixing with other dynamic server pages such as ASP and PHP.

The following instructions apply for IIS 6.

1. Open IIS and right-click on the website and select 'properties'.
2. Click the 'Configuration' button under Application Settings section
3. Click the 'Insert...' button to create a new wildcard mapping
4. Set the executable textbox to aspnet_isapi.dll file location.
	for .net 2.0, 3.0, 3.5: C:\Windows\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll
5. Make sure the checkbox 'Verify that file exists' is not checked.
6. Press 'OK' to confirm and close all the windows. 

Support
--------------------------------------

If you have any questions or comments please go to the *Discussions* link listed above, but before you do so please make sure to create a log of what is going on with your rules you can do that by adding the following right below the RewriteEngine statement:

    RewriteLog "log.txt"
    RewriteLogLevel 9

After you do this a **log.txt** file will be produced in the root of your web application that will log any request the rewriter services.