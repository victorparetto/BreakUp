// <copyright file="Leaderboards.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    using System.Collections.Generic;
    using Internal;

    /// <summary>
    /// Provides access to leaderboards registered via the CloudOnce Editor.
    /// This file was automatically generated by CloudOnce. Do not edit.
    /// </summary>
    public static class Leaderboards
    {
        private static readonly UnifiedLeaderboard s_climbModeLeaderboard = new UnifiedLeaderboard("ClimbModeLeaderboard",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CggI-9Dc8x0QAhAB"
#else
            "ClimbModeLeaderboard"
#endif
            );

        public static UnifiedLeaderboard ClimbModeLeaderboard
        {
            get { return s_climbModeLeaderboard; }
        }

        private static readonly UnifiedLeaderboard s_classicModeLeaderboard = new UnifiedLeaderboard("ClassicModeLeaderboard",
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
            ""
#elif !UNITY_EDITOR && UNITY_ANDROID && CLOUDONCE_GOOGLE
            "CggI-9Dc8x0QAhAC"
#else
            "ClassicModeLeaderboard"
#endif
            );

        public static UnifiedLeaderboard ClassicModeLeaderboard
        {
            get { return s_classicModeLeaderboard; }
        }

        public static string GetPlatformID(string internalId)
        {
            return s_leaderboardDictionary.ContainsKey(internalId)
                ? s_leaderboardDictionary[internalId].ID
                : string.Empty;
        }

        private static readonly Dictionary<string, UnifiedLeaderboard> s_leaderboardDictionary = new Dictionary<string, UnifiedLeaderboard>
        {
            { "ClimbModeLeaderboard", s_climbModeLeaderboard },
            { "ClassicModeLeaderboard", s_classicModeLeaderboard },
        };
    }
}
