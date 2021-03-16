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

    // closest point from an entity's collider to another point
    // this is used all over the place, so let's put it into one place so it's
    // easier to modify the method if needed
    // Distance(startPosition, Utils.ClosestPoint(ALVO, SELF))
    public static Vector3 ClosestPoint(Entity entity, Vector3 point)
    {
        // IMPORTANT: DO NOT use the collider itself. the position changes
        //            during animations, causing situations where attacks are
        //            interrupted because the target's hips moved a bit out of
        //            attack range, even though the target didn't actually move!
        //            => use transform.position and collider.radius instead!
        //
        //            this is probably faster than collider.ClosestPoints too

        // first of all, get radius but in WORLD SPACE not in LOCAL SPACE.
        // otherwise parent scales are not applied.
        float radius = BoundsRadius(entity.GetComponent<Collider>().bounds); // 1

        // now get the direction from point to entity
        // IMPORTANT: use entity.transform.position not
        //            collider.transform.position. that would still be the hip!
        Vector3 direction = entity.transform.position - point;
        //Debug.DrawLine(point, point + direction, Color.red, 1, false);

        // subtract radius from direction's length
        Vector3 directionSubtracted = Vector3.ClampMagnitude(direction, direction.magnitude - radius);

        // return the point
        //Debug.DrawLine(point, point + directionSubtracted, Color.green, 1, false);
        return point + directionSubtracted;
    }

    

    // helper function to calculate a bounds radius in WORLD SPACE
    // -> collider.radius is local scale
    // -> collider.bounds is world scale
    // -> use x+y extends average just to be sure (for capsules, x==y extends)
    // -> use 'extends' instead of 'size' because extends are the radius.
    //    in other words: if we come from the right, we only want to stop at
    //    the radius aka half the size, not twice the radius aka size.
    public static float BoundsRadius(Bounds bounds) =>
        (bounds.extents.x + bounds.extents.y) / 2;

    
}