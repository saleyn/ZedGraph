using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;
using System;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "ZedGraph" )]
[assembly: AssemblyDescription( "ZedGraph Library" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "John Champion, et al." )]
[assembly: AssemblyProduct( "ZedGraph Library" )]
[assembly: AssemblyCopyright( "Copyright © 2003-2007 John Champion" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "a552bf32-72a3-4d27-968c-72e7a90243f2" )]

[assembly: NeutralResourcesLanguageAttribute( "" )]

[assembly: CLSCompliant( true )]

// Make internal methods visible to unit test framework for testing
// NOTE: since ZedGraph is strongly signed the public key value was retrieved by
// runnint the sn tool found in "c:\Program Files (x86)\Microsoft SDKs\Windows\*\bin\*\sn.exe":
// sn -p  zedgraphkey.snk public.pk
// sn -tp public.pk

[assembly: InternalsVisibleTo("ZedGraph.LibTest,PublicKey="+
  "00240000048000009400000006020000002400005253413100040000010001002f86fc317ec78e"+
  "545188db70d915056f5a42bcc59f3e973eea23b771b6e6778c74d5b426a9ff9eb73447edee78fc"+
  "45ce3801471ae8070b846df0d8e327f2c9f493de3a761f1f6c2cab157cb4e64a7f0df833c95648"+
  "7dded5cb3fb80e104a0ba17b9cbd23eb3974e1724831f0fdcc431f4c08b7679ad3a1212fbcb173"+
  "bee104be")]
[assembly: InternalsVisibleTo("ZedGraph.UnitTest,PublicKey=" +
  "00240000048000009400000006020000002400005253413100040000010001002f86fc317ec78e" +
  "545188db70d915056f5a42bcc59f3e973eea23b771b6e6778c74d5b426a9ff9eb73447edee78fc" +
  "45ce3801471ae8070b846df0d8e327f2c9f493de3a761f1f6c2cab157cb4e64a7f0df833c95648" +
  "7dded5cb3fb80e104a0ba17b9cbd23eb3974e1724831f0fdcc431f4c08b7679ad3a1212fbcb173" +
  "bee104be")]
