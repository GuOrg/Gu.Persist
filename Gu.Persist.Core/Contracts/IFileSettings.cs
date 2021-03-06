﻿namespace Gu.Persist.Core
{
    /// <summary>
    /// Settings for storing and reading files.
    /// </summary>
    public interface IFileSettings
    {
        /// <summary>
        /// Gets the path to where backups are saved.
        /// </summary>
        string Directory { get; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        string Extension { get; }
    }
}