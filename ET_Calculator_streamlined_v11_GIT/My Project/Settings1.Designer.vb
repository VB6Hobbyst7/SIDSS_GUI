﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On



<Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
 Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0"),  _
 Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
Partial Friend NotInheritable Class MySettings
    Inherits Global.System.Configuration.ApplicationSettingsBase
    
    Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
    
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
    
    Public Shared ReadOnly Property [Default]() As MySettings
        Get
            
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
            Return defaultInstance
        End Get
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbx_csv1_settings() As String
        Get
            Return CType(Me("tbx_csv1_settings"),String)
        End Get
        Set
            Me("tbx_csv1_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property KC_MS_file_path_settings() As String
        Get
            Return CType(Me("KC_MS_file_path_settings"),String)
        End Get
        Set
            Me("KC_MS_file_path_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbx_EB_MS_settings() As String
        Get
            Return CType(Me("tbx_EB_MS_settings"),String)
        End Get
        Set
            Me("tbx_EB_MS_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbx_EB_Thermal_settings() As String
        Get
            Return CType(Me("tbx_EB_Thermal_settings"),String)
        End Get
        Set
            Me("tbx_EB_Thermal_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxSoilDepth_1_settings() As String
        Get
            Return CType(Me("tbxSoilDepth_1_settings"),String)
        End Get
        Set
            Me("tbxSoilDepth_1_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxSoilDepth_2_settings() As String
        Get
            Return CType(Me("tbxSoilDepth_2_settings"),String)
        End Get
        Set
            Me("tbxSoilDepth_2_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxSoilDepth_3_settings() As String
        Get
            Return CType(Me("tbxSoilDepth_3_settings"),String)
        End Get
        Set
            Me("tbxSoilDepth_3_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxSoilDepth_4_settings() As String
        Get
            Return CType(Me("tbxSoilDepth_4_settings"),String)
        End Get
        Set
            Me("tbxSoilDepth_4_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxSoilDepth_5_settings() As String
        Get
            Return CType(Me("tbxSoilDepth_5_settings"),String)
        End Get
        Set
            Me("tbxSoilDepth_5_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxRAW_1_settings() As String
        Get
            Return CType(Me("tbxRAW_1_settings"),String)
        End Get
        Set
            Me("tbxRAW_1_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxRAW_2_settings() As String
        Get
            Return CType(Me("tbxRAW_2_settings"),String)
        End Get
        Set
            Me("tbxRAW_2_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxRAW_3_settings() As String
        Get
            Return CType(Me("tbxRAW_3_settings"),String)
        End Get
        Set
            Me("tbxRAW_3_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxRAW_4_settings() As String
        Get
            Return CType(Me("tbxRAW_4_settings"),String)
        End Get
        Set
            Me("tbxRAW_4_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxRAW_5_settings() As String
        Get
            Return CType(Me("tbxRAW_5_settings"),String)
        End Get
        Set
            Me("tbxRAW_5_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxMinRootDepth_settings() As String
        Get
            Return CType(Me("tbxMinRootDepth_settings"),String)
        End Get
        Set
            Me("tbxMinRootDepth_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property tbxMaxRootDepth_settings() As String
        Get
            Return CType(Me("tbxMaxRootDepth_settings"),String)
        End Get
        Set
            Me("tbxMaxRootDepth_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property PlantDate_settings() As Date
        Get
            Return CType(Me("PlantDate_settings"),Date)
        End Get
        Set
            Me("PlantDate_settings") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property HarvestDate_settings() As Date
        Get
            Return CType(Me("HarvestDate_settings"),Date)
        End Get
        Set
            Me("HarvestDate_settings") = value
        End Set
    End Property
End Class

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.ET_Calculator_streamlined_v11_GIT.MySettings
            Get
                Return Global.ET_Calculator_streamlined_v11_GIT.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
