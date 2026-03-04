/*
 * ii's Stupid Menu  PluginInfo.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using PlayFab.ProfilesModels;

namespace iiMenu
{
    public class PluginInfo
    {
        public const string GUID = "org.speech.gorillatag.Seravyn";
        public const string Name = "Seravyn Menu";
        public const string Description = "Created by @superiorspeech with love <3";
        public const string BuildTimestamp = "2026-03-2";
        public const string Version = "1.1.0";

        public const string BaseDirectory = "Seravyn";
        public const string ClientResourcePath = "Seravyn.Resources.Client";
        public const string ServerResourcePath = "https://raw.githubusercontent.com/lainprojects/Seravyn/master/Resources/Server";
        public const string ServerAPI = "https://iidk.online";
        
        public const string Logo = @"   _____ ______ _____       __      ____     ___   _ 
  / ____|  ____|  __ \     /\ \    / /\ \   / / \ | |
 | (___ | |__  | |__) |   /  \ \  / /  \ \_/ /|  \| |
  \___ \|  __| |  _  /   / /\ \ \/ /    \   / | . ` |
  ____) | |____| | \ \  / ____ \  /      | |  | |\  |
 |_____/|______|_|  \_\/_/    \_\/       |_|  |_| \_|
                                                     
                                                     ";

        public static bool BetaBuild = false;
        public static bool AdminBuild = false;
        public static bool CommunityBuild = true;
    }
}
