using System;

namespace BradlBot
{
    /// <summary>
    /// Used to denote that a class within a DLL is the starting point of an Addon.
    /// Please use only once in application with the class name Start
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AddonAttribute : Attribute
    {

    }
}