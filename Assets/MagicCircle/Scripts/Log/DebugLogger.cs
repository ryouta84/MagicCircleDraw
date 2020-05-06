using System.Diagnostics;
using System.IO;

public static class DebugLogger {
    private static string logPath = "";

    [Conditional("UNITY_EDITOR")]
    public static void Log(object o) {
        UnityEngine.Debug.Log(o);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Dump2D(short[][] array) {
        var file = File.OpenWrite(DebugLogger.logPath);
        file.SetLength(0);
        file.Close();
        for (int i = 0; i < array.Length; i++) {
            File.AppendAllText(DebugLogger.logPath, string.Join(" ", array[i]) + "\n");
        }
    }
}
