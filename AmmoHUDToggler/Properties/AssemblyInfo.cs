using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;

[assembly: AssemblyTitle(AmmoHUDToggler.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AmmoHUDToggler.BuildInfo.Company)]
[assembly: AssemblyProduct(AmmoHUDToggler.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + AmmoHUDToggler.BuildInfo.Author)]
[assembly: AssemblyTrademark(AmmoHUDToggler.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(AmmoHUDToggler.BuildInfo.Version)]
[assembly: AssemblyFileVersion(AmmoHUDToggler.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(AmmoHUDToggler.AmmoHUDToggler), AmmoHUDToggler.BuildInfo.Name, AmmoHUDToggler.BuildInfo.Version, AmmoHUDToggler.BuildInfo.Author, AmmoHUDToggler.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]