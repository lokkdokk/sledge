﻿// This file is automatically generated by ExtractBlobData.
// To regenerate it, run: ExtractBlobData "C:\Path\To\Steam"

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sledge.Providers;
using Sledge.Settings.Models;

namespace Sledge.Settings.GameDetection.GameFiles
{
    public static class SteamGames
    {
        public static List<SteamApp> SteamApps { get; set; }

        static SteamGames()
        {
            SteamApps = new List<SteamApp>
            {
                new SteamApp(10, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "cstrike", ExecutableName = "hl.exe" }, // Counter-Strike
                new SteamApp(20, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "tfc", ExecutableName = "hl.exe" }, // Team Fortress Classic
                new SteamApp(30, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "dod", ExecutableName = "hl.exe" }, // Day of Defeat
                new SteamApp(40, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "dmc", ExecutableName = "hl.exe" }, // Deathmatch Classic
                new SteamApp(50, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "gearbox", ExecutableName = "hl.exe" }, // Half-Life: Opposing Force
                new SteamApp(60, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "ricochet", ExecutableName = "hl.exe" }, // Ricochet
                new SteamApp(70, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "valve", ExecutableName = "hl.exe" }, // Half-Life
                new SteamApp(80, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "czero", ExecutableName = "hl.exe" }, // Condition Zero
                new SteamApp(100, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "czeror", ExecutableName = "hl.exe" }, // Condition Zero Deleted Scenes
                new SteamApp(130, Engine.Goldsource) { GameDirectory = "valve", ModDirectory = "bshift", ExecutableName = "hl.exe" }, // Half-Life: Blue Shift
                new SteamApp(225840, Engine.Goldsource) { GameDirectory = "svencoop", ModDirectory = "svencoop", ExecutableName = "svencoop.exe" }, // Sven Co-op
                // new SteamApp(220, Engine.Source), // Half-Life 2
                // new SteamApp(240, Engine.Source), // Counter-Strike: Source
                // new SteamApp(280, Engine.Source), // Half-Life: Source
                // new SteamApp(300, Engine.Source), // Day of Defeat: Source
                // new SteamApp(320, Engine.Source), // Half-Life 2: Deathmatch
                // new SteamApp(340, Engine.Source), // Half-Life 2: Lost Coast
                // new SteamApp(360, Engine.Source), // Half-Life Deathmatch: Source
                // new SteamApp(380, Engine.Source), // Half-Life 2: Episode One
                // new SteamApp(400, Engine.Source), // Portal
                // new SteamApp(420, Engine.Source), // Half-Life 2: Episode Two
                // new SteamApp(440, Engine.Source), // Team Fortress 2
                // new SteamApp(500, Engine.Source), // Left 4 Dead
                // new SteamApp(550, Engine.Source), // Left 4 Dead 2
                // new SteamApp(570, Engine.Source), // Dota 2
                // new SteamApp(620, Engine.Source), // Portal 2
                // new SteamApp(630, Engine.Source), // Alien Swarm
                // new SteamApp(1800, Engine.Source), // Counter-Strike: Global Offensive
                // new SteamApp(4000, Engine.Source), // Garry's Mod
                // new SteamApp(211, Engine.Source), // Source SDK
                // new SteamApp(215, Engine.Source), // Source SDK Base 2006
                // new SteamApp(218, Engine.Source), // Source SDK Base - Orange Box
                // new SteamApp(513, Engine.Source), // Left 4 Dead Authoring Tools
                // new SteamApp(629, Engine.Source), // Portal 2 Authoring Tools - Beta
            };
        }

        public static List<SteamGame> GetDetectedSteamGames(string steamInstallLocation)
        {
            var games = new List<SteamGame>();

            var folders = new List<string>();
            folders.Add(Path.Combine(steamInstallLocation, "SteamApps"));

            var libraryFoldersFile = Path.Combine(steamInstallLocation, "SteamApps", "libraryfolders.vdf");
            if (File.Exists(libraryFoldersFile))
            {
                try
                {
                    var gs = GenericStructure.Parse(libraryFoldersFile).FirstOrDefault(x => x.Name == "LibraryFolders");
                    if (gs != null)
                    {
                        foreach (var pk in gs.GetPropertyKeys())
                        {
                            int i;
                            if (!int.TryParse(pk, out i)) continue;

                            var value = gs.GetPropertyValue(pk, false);
                            folders.Add(Path.Combine(value, "SteamApps"));
                        }
                    }
                }
                catch
                {
                    // Unable to parse libraryfolders.vdf
                }
            }

            folders.RemoveAll(x => !Directory.Exists(x));

            foreach (var steamApp in SteamApps)
            {
                var manifestName = "appmanifest_" + steamApp.AppID + ".acf";
                var folder = folders.FirstOrDefault(x => File.Exists(Path.Combine(x, manifestName)));
                if (folder != null)
                {
                    try
                    {
                        var gs = GenericStructure.Parse(Path.Combine(folder, manifestName)).FirstOrDefault(x => x.Name == "AppState");
                        if (gs != null)
                        {
                            var name = gs.GetPropertyValue("name", true);
                            var installDir = gs.GetPropertyValue("installdir", true);
                            if (!Directory.Exists(installDir)) installDir = Path.Combine(folder, "common", installDir);
                            if (Directory.Exists(installDir))
                            {
                                games.Add(new SteamGame
                                {
                                    AppId = steamApp.AppID,
                                    Engine = steamApp.Engine,
                                    Name = name,
                                    InstallPath = installDir,
                                    GameDirectory = steamApp.GameDirectory,
                                    ModDirectory = steamApp.ModDirectory,
                                    ExecutableName = steamApp.ExecutableName
                                });
                            }
                        }
                    }
                    catch
                    {
                        // Unable to parse manifest
                    }
                }
            }

            return games;
        }
    }

    public class SteamGame
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string InstallPath { get; set; }
        public Engine Engine { get; set; }

        public string GameDirectory { get; set; }
        public string ModDirectory { get; set; }
        public string ExecutableName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
