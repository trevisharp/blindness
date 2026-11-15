/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Blindness.Reload;

/// <summary>
/// A object to watch update and creation of files.
/// </summary>
public class FileWatcher
{
    long lastTimeUpdate = 0;
    readonly List<string> filters = [];
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
    /// Reset the FileWatcher state keeping the filters.
    /// </summary>
    public FileWatcher Reset()
    {
        watcher?.Dispose();
        watcher = null;
        Interlocked.Exchange(ref lastTimeUpdate, 0);
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

        var last = Interlocked.Read(ref lastTimeUpdate);
        if (last == 0)
            return false;

        var timePassed = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - last);
        if (timePassed < TimeFactor)
            return false;
        
        return Interlocked.CompareExchange(ref lastTimeUpdate, 0, last) == last;
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

        Interlocked.Exchange(ref lastTimeUpdate, 0);
        void onChange(object sender, FileSystemEventArgs e)
            => Interlocked.Exchange(ref lastTimeUpdate, DateTime.UtcNow.Ticks);

        watcher.Created += onChange;
        watcher.Changed += onChange;
        watcher.Renamed += onChange;
        watcher.Deleted += onChange;
        watcher.EnableRaisingEvents = true;
    }
}