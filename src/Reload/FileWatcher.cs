/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace Blindness.Reload;

/// <summary>
/// A object to watch update and creation of files.
/// </summary>
public class FileWatcher
{
    DateTime lastTimeUpdate;
    List<string> filters = [];
    FileSystemWatcher watcher = null;

    /// <summary>
    /// The time waited to return true from Verify func.
    /// </summary>
    public TimeSpan TimeFactor { get; set; } = TimeSpan.FromSeconds(1f);

    /// <summary>
    /// Add a file to watch, use *.extension to find many files.
    /// </summary>
    public FileWatcher AddFilter(string filePath)
    {
        filters.Add(filePath);
        return this;
    }

    /// <summary>
    /// Clear all filters.
    /// </summary>
    public FileWatcher ClearFilters()
    {
        filters.Clear();
        return this;
    }

    /// <summary>
    /// Add a filter to CSharp files.
    /// </summary>
    public FileWatcher AddCSharpFilter()
    {
        filters.Add("*.cs");
        return this;
    }

    /// <summary>
    /// Reset all FileWatcher metadata.
    /// </summary>
    public FileWatcher Reset()
    {
        watcher = null;
        filters = [];
        return this;
    }

    /// <summary>
    /// Returns true if the files have been modified and 
    /// have not been modified in the last 'TimeFactor'
    /// seconds. That is, they are no longer being modified.
    /// </summary>
    public virtual bool Verify()
    {
        InitWatcherIfNeeded();

        if (lastTimeUpdate == DateTime.MinValue)
            return false;

        var timePassed = DateTime.Now - lastTimeUpdate;
        if (timePassed < TimeFactor)
            return false;
        
        lastTimeUpdate = DateTime.MinValue;
        return true;
    }

    protected virtual void InitWatcherIfNeeded()
    {
        if (watcher is not null)
            return;
        
        InitWatcher();
    }

    protected virtual void InitWatcher()
    {
        watcher = new() {
            Path = Environment.CurrentDirectory,
            IncludeSubdirectories = true
        };
        foreach (var filter in filters)
            watcher.Filters.Add(filter);

        lastTimeUpdate = DateTime.MinValue;
        void onChange(object sender, FileSystemEventArgs e)
            => lastTimeUpdate = DateTime.Now;

        watcher.Created += onChange;
        watcher.Changed += onChange;
        watcher.EnableRaisingEvents = true;
    }
}