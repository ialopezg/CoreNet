using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyCompany("Programmers Inc...")]
[assembly: AssemblyProduct("ProgrammersInc")]
[assembly: AssemblyCopyright("2000 - 2008 The One Angel")]
[assembly: AssemblyVersion(RevisionClass.FullVersion)]

internal static class RevisionClass
{
	public const string Major = "3";
	public const string Minor = "1";
	public const string Build = "1";
	public const string Revision = "1312";
	
	public const string MainVersion = Major + "." + Minor;
	public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;
}
	
