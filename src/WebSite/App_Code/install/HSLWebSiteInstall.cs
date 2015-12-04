//---------------------------------------------------------------------
// <copyright file="HSLWebSiteInstall.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    The HSLWebSite installer class
//      This class is used by InstallUtil to setup the the HSL Web.
//      It is responsible for doing things like creating the event source 
//      and the perfmon category
// </summary>
//---------------------------------------------------------------------

namespace MS.Msn.InternetAccess.Hsl.WebSite.Install
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Configuration.Install;

    using Microsoft.Win32;

    using MS.Msn.InternetAccess.Common.Utilities;
    using MS.Msn.InternetAccess.Hsl.WebSite.Monitoring;

    /// <summary>
    /// HSLWebInstall installs the HSL Web.
    /// The .NET framework will call this when InstallUtil is run against the exe.
    /// </summary>
    [RunInstaller(true)]
    public class HSLWebSiteInstall : Installer
    {
        /// <summary>
        /// The name of the App
        /// </summary>
        public const String AppName = "MSNIA HSL WebSite";

        /// <summary>
        /// Regkey that holds the event viewer access permissions value
        /// </summary>
        private const String EventViewerRegistryKey = @"SYSTEM\CurrentControlSet\Services\Eventlog\Application";

        /// <summary>
        /// The name of the value that contains the permission string
        /// </summary>
        private const String EventViewerRegistryKeyValue = "CustomSD";

        /// <summary>
        /// The permissions to add
        /// </summary>
        private const String EventViewerRegistryPermission = "(A;;0x2;;;NS)";

        /// <summary>
        /// Event Log Installer
        /// </summary>
        private EventLogInstaller eventLogInstaller = new EventLogInstaller();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HSLWebSiteInstall()
        {
            this.eventLogInstaller.Source = AppName;

            this.Installers.Add(this.eventLogInstaller);
        }

        /// <summary>
        /// This method is called by InstallUtil with /install
        /// </summary>
        /// <param name="stateSaver">The state object used by InstallUtil</param>
        [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
        public override void Install(IDictionary stateSaver)
        {
            Context.LogMessage("MSNIA HSL WebSite Installation Starting");
            
            try
            {
                base.Install(stateSaver);

                HSLWebSitePerformanceCounters.Instance.CreatePerformanceCounters();
                Context.LogMessage("HSLWebSitePerformanceCounters Installation Completed");

                // add event viewer registry permissions
                RegistryKey key = Registry.LocalMachine.CreateSubKey(EventViewerRegistryKey);
                RegistryModifier.AppendData(key, EventViewerRegistryKeyValue, EventViewerRegistryPermission);
            }
            catch (Exception exception)
            {
                Context.LogMessage("MSNIA HSL WebSite Install Failed" + exception.Message + exception.StackTrace);
                throw;
            }

            Context.LogMessage("MSNIA HSL WebSite Installation Completed");
        }

        /// <summary>
        /// The method called when InstallUtil is called with /uninstall
        /// </summary>
        /// <param name="savedState">The state object used by InstallUtil</param>
        public override void Uninstall(IDictionary savedState)
        {
            Context.LogMessage("MSNIA HSL WebSite Uninstallation Starting");

            try
            {
                base.Uninstall(savedState);

                HSLWebSitePerformanceCounters.Instance.DeletePerformanceCounters();
                Context.LogMessage("HSLWebSitePerformanceCounters Uninstall Completed");
            }
            catch (Exception exception)
            {
                Context.LogMessage("MSNIA HSL WebSite Uninstall Failed" + exception.Message + exception.StackTrace);
                throw;
            }

            try
            {
                // remove event viewer registry permissions
                RegistryKey key = Registry.LocalMachine.OpenSubKey(EventViewerRegistryKey);
                RegistryModifier.RemoveData(key, EventViewerRegistryKeyValue, EventViewerRegistryPermission);
            }
            catch (Exception exception)
            {
                // if exception, absorb and move on
                Context.LogMessage("MSNIA HSL WebSite registry removal failed. Uninstall continuing." + exception.Message + exception.StackTrace);
            }

            Context.LogMessage("MSNIA HSL WebSite Uninstallation Completed");
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">Are we disposing now?</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (null != this.eventLogInstaller)
                    {
                        this.eventLogInstaller.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
