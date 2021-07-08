using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(NoReload.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(NoReload.BuildInfo.Company)]
[assembly: AssemblyProduct(NoReload.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + NoReload.BuildInfo.Author)]
[assembly: AssemblyTrademark(NoReload.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(NoReload.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NoReload.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(NoReload.NoReload), NoReload.BuildInfo.Name, NoReload.BuildInfo.Version, NoReload.BuildInfo.Author, NoReload.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]