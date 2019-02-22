﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mellis.Lang.Python3.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Localized_Python3_Interpreter {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Localized_Python3_Interpreter() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Mellis.Lang.Python3.Resources.Localized_Python3_Interpreter", typeof(Localized_Python3_Interpreter).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. Alla anrop i funktionsstacken (call stack) blev inte korrekt stängda..
        /// </summary>
        internal static string Ex_CallStack_LastStackNotPopped {
            get {
                return ResourceManager.GetString("Ex_CallStack_LastStackNotPopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. Anrops stacken var oförväntat tom vid försök att stänga den..
        /// </summary>
        internal static string Ex_CallStack_PopEmpty {
            get {
                return ResourceManager.GetString("Ex_CallStack_PopEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. En operation försökte trycka på null på anrops stacken..
        /// </summary>
        internal static string Ex_CallStack_PushNull {
            get {
                return ResourceManager.GetString("Ex_CallStack_PushNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Koden har redan kört klart..
        /// </summary>
        internal static string Ex_Process_Ended {
            get {
                return ResourceManager.GetString("Ex_Process_Ended", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Koden inväntar att funktionen återupptas innan den kan köra vidare..
        /// </summary>
        internal static string Ex_Process_Yielded {
            get {
                return ResourceManager.GetString("Ex_Process_Yielded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. Alla variabelgrupper (scopes) blev inte korrekt stängda..
        /// </summary>
        internal static string Ex_Scope_LastScopeNotPopped {
            get {
                return ResourceManager.GetString("Ex_Scope_LastScopeNotPopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. En operation försökte stänga den globala variabelgruppen..
        /// </summary>
        internal static string Ex_Scope_PopGlobal {
            get {
                return ResourceManager.GetString("Ex_Scope_PopGlobal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Okänt internt fel. &quot;{0}&quot;.
        /// </summary>
        internal static string Ex_Unknown_Error {
            get {
                return ResourceManager.GetString("Ex_Unknown_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. Värde stacken var oförväntat tom vid försök att hämta värde..
        /// </summary>
        internal static string Ex_ValueStack_PopEmpty {
            get {
                return ResourceManager.GetString("Ex_ValueStack_PopEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Internt fel. En operation försökte trycka på null på värde stacken..
        /// </summary>
        internal static string Ex_ValueStack_PushNull {
            get {
                return ResourceManager.GetString("Ex_ValueStack_PushNull", resourceCulture);
            }
        }
    }
}
