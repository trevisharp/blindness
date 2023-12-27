using System;
using System.IO;
using System.Collections.Generic;

namespace Blindness.Abstracts;

using Internal;

public static class HotReload
{
    private static DateTime startTime;
    private static Dictionary<string, CallInfo> infos;
    private static FileSystemWatcher watcher;
    
    public static bool IsActive 
    {
        get => watcher.EnableRaisingEvents;
        set => watcher.EnableRaisingEvents = value;
    }

    static HotReload()
    {
        infos = new();
        IsActive = false;
        startTime = DateTime.Now;

        watcher = new FileSystemWatcher();
        watcher.Filter = "*.cs";
        watcher.Changed += (sender, e) =>
        {
            var file = e.FullPath;

            
        };
    }

    public static bool Invoke(
        string code,
        params object[] parameters
    )
    {
        if (!IsActive)
            return false;

        if (!infos.ContainsKey(code))
        {

        }


        return true;
    }


}