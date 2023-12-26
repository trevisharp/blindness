namespace Blindness.Abstracts;

public static class HotReload
{
    public static bool IsActive { get; set; } = false;

    public static void Invoke(
        string methodName,
        object obj,
        params object[] parameters
    )
    {
        if (!IsActive)
            return;
        

    }
}