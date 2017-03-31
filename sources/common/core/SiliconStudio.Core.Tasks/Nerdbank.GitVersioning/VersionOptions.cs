﻿// Copyright (c) 2017 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using SiliconStudio.Core;

namespace Nerdbank.GitVersioning
{
    /// <summary>
    /// Store package version read from .xkpkg, implemented for <see cref="GitExtensions"/>.
    /// </summary>
    class VersionOptions
    {
        public int BuildNumberOffset => 0;

        public PackageVersion Version { get; set; }
    }
}