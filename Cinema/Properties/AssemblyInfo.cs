using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(Cinema.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Cinema.BuildInfo.Company)]
[assembly: AssemblyProduct(Cinema.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + Cinema.BuildInfo.Author)]
[assembly: AssemblyTrademark(Cinema.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(Cinema.BuildInfo.Version)]
[assembly: AssemblyFileVersion(Cinema.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(Cinema.Cinema), Cinema.BuildInfo.Name, Cinema.BuildInfo.Version, Cinema.BuildInfo.Author, Cinema.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]
[assembly: InternalsVisibleTo("CinemaEntanglement")]