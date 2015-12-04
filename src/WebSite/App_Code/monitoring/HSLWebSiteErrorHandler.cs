//---------------------------------------------------------------------
// <copyright file="HSLWebSiteErrorHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HSLWebSiteErrorHandler class
// </summary>
//---------------------------------------------------------------------
namespace MS.Msn.InternetAccess.Hsl.WebSite.Monitoring
{
    using System;
    using System.Web;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Configuration;
    using System.Web.Services.Protocols;

    using MS.Msn.InternetAccess.Common.Monitoring;
    using MS.Msn.InternetAccess.Hsl.WebSite.Install;

    /// <summary>
    /// A list of HSLServices Event ID's
    /// NOTE: These MUST be sequential
    /// </summary>
    public enum ErrorID
    {
        /// <summary>
        /// <![CDATA[
        /// A default value has to be assigned for fx cop fix
        /// http://fxcop/docs/rules.aspx?version=v1.34&url=/Design/EnumsShouldHaveZeroValue.html
        /// ]]>
        /// </summary>
        fxcop_fix = 0,

        /// <summary>
        /// HSL Web Services error
        /// </summary>
        HSLServicesError = 1000,

        /// <summary>
        /// Any perf counter error
        /// </summary>
        PerformanceCounterError = 1001,

        /// <summary>
        /// Unhandled programming exceptions eg. unexpected null pointer
        /// </summary>
        HSLWebSiteUnexpectedError = 1002,

        /// <summary>
        /// The caller is requesting a URL that is not valid
        /// </summary>
        InvalidPath = 1003
    }

    /// <summary>
    /// HSLWebErrorHandler contains error handler code for the HSL Web.
    /// </summary>
    public sealed class HSLWebSiteErrorHandler : BaseErrorHandler
    {
        /// <summary>
        /// The Error Code returned in an HttpException when you get a File Not Found error
        /// </summary>
        private const Int32 FileNotFoundErrorCode = -2147467259;

        /// <summary>
        /// The one and only instance of the Event Handler class
        /// </summary>
        private static HSLWebSiteErrorHandler instance = new HSLWebSiteErrorHandler();

        /// <summary>
        /// The list of errors
        /// </summary>
        private Dictionary<UInt16, ErrorInfo> errorList = new Dictionary<UInt16, ErrorInfo>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        private HSLWebSiteErrorHandler()
            : base(HSLWebSiteInstall.AppName)
        {
            UInt16 invalidPathThreshold = Convert.ToUInt16(ConfigurationManager.AppSettings["InvalidPath_Threshold"]);

            this.errorList.Add((UInt16) ErrorID.HSLServicesError, new ErrorInfo((UInt16) ErrorID.HSLServicesError, EventLogEntryType.Error, "HSL Web Services encountered an error", 0, 0));
            this.errorList.Add((UInt16) ErrorID.PerformanceCounterError, new ErrorInfo((UInt16) ErrorID.PerformanceCounterError, EventLogEntryType.Error, "Performance counter error", 0, 0));
            this.errorList.Add((UInt16) ErrorID.HSLWebSiteUnexpectedError, new ErrorInfo((UInt16)ErrorID.HSLWebSiteUnexpectedError, EventLogEntryType.Error, "HSL WebSite encountered an unexpected error", 0, 0));
            this.errorList.Add((UInt16) ErrorID.InvalidPath, new ErrorInfo((UInt16)ErrorID.InvalidPath, EventLogEntryType.Warning, "Caller requested a path that does not exist", invalidPathThreshold, 60));
        }

        /// <summary>
        /// Gets an instance of the one and only Event Handler instance.
        /// The instance will be created if necessary.
        /// </summary>
        public static HSLWebSiteErrorHandler Instance
        {
            get
            {
                return HSLWebSiteErrorHandler.instance;
            }
        }

        /// <summary>
        /// Gets the list of error information
        /// </summary>
        public override Dictionary<UInt16, ErrorInfo> ErrorList
        {
            get
            {
                return this.errorList;
            }
        }

        /// <summary>
        /// Handles the exception
        /// </summary>
        /// <param name="ex">Exception</param>
        public void HandleException(Exception ex)
        {
            // Log issues related to communicating with the HSLServices
            if (ex is System.Net.WebException)
            {
                HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.HSLServicesError);
                HSLWebSiteErrorHandler.Instance.WriteToEventLog(
                                                            (UInt16)ErrorID.HSLServicesError,
                                                            "HSL Web Services encountered an error",
                                                            ex);
            }

            // Only log server-side related SOAP issues
            else if (ex is SoapException)
            {
                // Ignore client-side issues because ClientFaultCode is caused by invalid search criteria
                // which is most likely caused by someone tampering with the URL
                if (((SoapException)ex).Code == SoapException.ServerFaultCode)
                {
                    HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.HSLServicesError);
                    HSLWebSiteErrorHandler.Instance.WriteToEventLog(
                                                                (UInt16)ErrorID.HSLServicesError,
                                                                "HSL Web Services encountered an error",
                                                                ex);
                }
            }

            // Log known HttpExceptions (i.e. file not found)
            else if (ex is HttpException && ((HttpException)ex).ErrorCode == FileNotFoundErrorCode)
            {
                HSLWebSiteErrorHandler.Instance.WriteToEventLog(
                                                            (UInt16)ErrorID.InvalidPath,
                                                            "Caller requested a path that does not exist",
                                                            ex);
            }

            // Log unknown errors
            else
            {
                HSLWebSitePerformanceCounters.Instance.IncrementPerformanceCounter(HSLWebSitePerformanceCounters.HSLWebSiteUnexpectedError);
                HSLWebSiteErrorHandler.Instance.WriteToEventLog(
                                                            (UInt16)ErrorID.HSLWebSiteUnexpectedError,
                                                            "HSL WebSite encountered an unexpected error",
                                                            ex);
            }
        }
    }
}