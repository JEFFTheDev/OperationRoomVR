using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ErrorRegistry{

    private static List<Error> errors;
    public static AudioSource audioSource;

    public static void Register(string message)
    {
        if (errors == null)
            errors = new List<Error>();
        
        errors.Add(new Error(message));

        
    }

    public static string GetErrorString()
    {

        if (errors == null)
            errors = new List<Error>();

        string errorMessage = "";

        foreach(Error e in errors)
        {
            errorMessage += e.message + "\n";
        }

        return errorMessage;

    }

    public static void Reset()
    {
        errors = new List<Error>();
    }

    private struct Error
    {
        public string message;

        public Error(string message)
        {
            this.message = message;
        }
    
    }
}
