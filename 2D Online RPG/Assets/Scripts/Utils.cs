using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Linq;

public class Utils 
{

    // PBKDF2 hashing recommended by NIST:
    // http://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-132.pdf
    // salt should be at least 128 bits = 16 bytes
    public static string PBKDF2Hash(string text, string salt)
    {
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(text, saltBytes, 10000);
        byte[] hash = pbkdf2.GetBytes(20);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }



    // invoke multiple functions by prefix via reflection
    static Dictionary<KeyValuePair<Type, string>, MethodInfo[]> lookup = new Dictionary<KeyValuePair<Type, string>, MethodInfo[]>();
    public static MethodInfo[] GetMethodsByPrefix(Type type, string methodPrefix)
    {
        KeyValuePair<Type, string> key = new KeyValuePair<Type, string>(type, methodPrefix);

        if(!lookup.ContainsKey(key))
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                                       .Where(m => m.Name.StartsWith(methodPrefix))
                                       .ToArray();
            lookup[key] = methods;
        }
        return lookup[key];
    }

    public static void InvokeMany(Type type, object onObject, string methodPrefix, params object[] args)
    {
        foreach(MethodInfo method in GetMethodsByPrefix(type, methodPrefix))
            method.Invoke(onObject, args);
    }
    
}