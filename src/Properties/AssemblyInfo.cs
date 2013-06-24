using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: InternalsVisibleTo("ManagedFusion.Rewriter.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e9540c4eff808f1d5e4e986463552b2af820ce3eff95669fa6e4c6124a86b494020e9212c67cec3570846f722b34afa14795ffb93addaceaf517a9b83e79d5e6a27c5bf36eba9ac7f58505cee32b7e3473dd9bc5a390337e5a69989adadc1aed9d68f025ada082551430a04499ba22992ffef5fb86d286dbb36fad1a19ea2fd1")]

// Specifies that an assembly cannot cause an elevation of privilege. 
[assembly: SecurityTransparent]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Managed Fusion URL Rewriter and Reverse Proxy")]
[assembly: AssemblyDescription("Managed Fusion URL Rewriter and Reverse Proxy is a powerful URL manipulation engine based on the Apache mod_rewrite extension.")]
[assembly: AssemblyProduct("ManagedFusion.Rewriter")]
[assembly: AssemblyCompany("Managed Fusion, LLC")]
[assembly: AssemblyCopyright("Copyright © Nick Berardi, Managed Fusion, LLC 2013")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The AllowPartiallyTrustedCallersAttribute requires the assembly to be signed with a strong name key.
// This attribute is necessary since the control is called by either an intranet or Internet
// Web page that should be running under restricted permissions.
[assembly: AllowPartiallyTrustedCallers]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3e309752-920a-47c3-9655-7b7680f0bfa0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("3.7.0.*")]
[assembly: AssemblyFileVersion("3.7.0")]
[assembly: AssemblyInformationalVersion("3.7")]