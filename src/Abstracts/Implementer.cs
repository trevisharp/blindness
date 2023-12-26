using System;
using System.Diagnostics;
using System.Reflection;

namespace Blindness.Abstracts;

public class Implementer
{
    public void ImplementAndRun(Action appFunc)
    {
        if (!needImplement())
        {
            appFunc();
            return;
        }

        implements();
        reRun();
    }

    private void implements()
    {

    }

    private void reRun()
    {
        execute("dotnet", "build");
        
        var assembly = Assembly.GetEntryAssembly();
        var dll = assembly.Location;
        var exe = dll.Replace(".dll", ".exe");
        execute(exe);
    }

    private void execute(string filename, string args = "")
    {
        var info = new ProcessStartInfo {
            FileName = filename,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = new Process {
            StartInfo = info
        };

        process.ErrorDataReceived += (o, e) =>
            System.Console.WriteLine(e.Data);
        process.OutputDataReceived += (o, e) =>
            System.Console.WriteLine(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }

    private bool needImplement()
    {
        return Random.Shared.Next(2) == 0;
    }
}