﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfMinesweeper.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Windows.Media.LinearGradientBrush TileBrushGradient {
            get {
                return ((global::System.Windows.Media.LinearGradientBrush)(this["TileBrushGradient"]));
            }
            set {
                this["TileBrushGradient"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" -300,-300 ")]
        public global::System.Windows.Point LastLocation {
            get {
                return ((global::System.Windows.Point)(this["LastLocation"]));
            }
            set {
                this["LastLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" #FF5F9EA0 ")]
        public global::System.Windows.Media.SolidColorBrush TileBrushSolid {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["TileBrushSolid"]));
            }
            set {
                this["TileBrushSolid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" #FF5F9EA0 ")]
        public global::System.Windows.Media.Color TileColor {
            get {
                return ((global::System.Windows.Media.Color)(this["TileColor"]));
            }
            set {
                this["TileColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" 261 ")]
        public double LastWindowMinHeight {
            get {
                return ((double)(this["LastWindowMinHeight"]));
            }
            set {
                this["LastWindowMinHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" 170 ")]
        public double LastWindowMinWidth {
            get {
                return ((double)(this["LastWindowMinWidth"]));
            }
            set {
                this["LastWindowMinWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" 261,170 ")]
        public global::System.Windows.Size LastWindowMinSize {
            get {
                return ((global::System.Windows.Size)(this["LastWindowMinSize"]));
            }
            set {
                this["LastWindowMinSize"] = value;
            }
        }
    }
}
