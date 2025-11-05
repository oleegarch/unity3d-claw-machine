using UnityEngine;

public static class CommandLine
{
    public static string GetArg(string name) {
        string[] args = System.Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++) {
            if(args[i] == name && args.Length > i + 1) {
                return args[i + 1];
            }
        }
        return null;
    }
}