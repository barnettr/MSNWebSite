//---------------------------------------------------------------------
// <copyright file="HSLWebSitePerformanceCounters.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Contains the HSLWebSitePerformanceCounters class
// </summary>
//---------------------------------------------------------------------

namespace MS.Msn.InternetAccess.Hsl.WebSite.Monitoring
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using MS.Msn.InternetAccess.Common.Monitoring;
    using MS.Msn.InternetAccess.Hsl.WebSite.Install;

    /// <summary>
    /// HSLWebSitePerformanceCounters contains perf counters for the HSL Web.
    /// </summary>
    public sealed class HSLWebSitePerformanceCounters : BasePerformanceCounters
    {
        #region public const members

        /// <summary>
        /// The category name of all the performance counters in this class
        /// </summary>
        public const String Category = HSLWebSiteInstall.AppName;

        /// <summary>
        /// Number of HSL Services Errors
        /// </summary>
        public const String HSLServicesError = "HSL Services Errors";

        /// <summary>
        /// Number of HSL Web Errors
        /// </summary>
        public const String HSLWebSiteUnexpectedError = "HSLWebSite Errors";

        /// <summary>
        /// Number of search requests received
        /// </summary>
        public const String SearchViewRequests = "SearchView Requests";

        /// <summary>
        /// Number of map view results requests received
        /// </summary>
        public const String MapViewRequests = "MapView Requests";

        /// <summary>
        /// Number of list view results requests received
        /// </summary>
        public const String ListViewRequests = "ListView Requests";

        /// <summary>
        /// Number of Details Page requests received
        /// </summary>
        public const String DetailsViewRequests = "DetailsView Requests";

        #region "Average counters"

        /// <summary>
        /// Average Search Page response
        /// </summary>
        public const String AveSearchViewResponseTime = "Average SearchView Response";

        /// <summary>
        /// Average List results page response
        /// </summary>
        public const String AveListViewResponseTime = "Average ListView Response";

        /// <summary>
        /// Average Map results page response
        /// </summary>
        public const String AveMapViewResponseTime = "Average MapView Response";

        /// <summary>
        /// Average Details Page response
        /// </summary>
        public const String AveDetailsViewResponseTime = "Average DetailsView Response";

        #endregion

        #endregion public const members

        #region private static members

        /// <summary>
        /// List of all perf counters to create with a matching per second counter
        /// </summary>
        private static String[] allRegularAndPerSecondCounters = 
        { 
            HSLServicesError,
            HSLWebSiteUnexpectedError,
            SearchViewRequests,
            MapViewRequests,
            ListViewRequests,
            DetailsViewRequests
        };

        /// <summary>
        /// List of all average perf counters to create
        /// </summary>
        private static String[] allAveTimerCounters = 
        {
            AveSearchViewResponseTime,
            AveListViewResponseTime,
            AveMapViewResponseTime,
            AveDetailsViewResponseTime
        };

        /// <summary>
        /// A lock object used for insuring that only one caller can cause creation of an instance.
        /// </summary>
        private static object lockObject = new object();

        /// <summary>
        /// The one and only instance of the perf counter class
        /// </summary>
        private static HSLWebSitePerformanceCounters instance;

        #endregion private static members

        /// <summary>
        /// Static constructor
        /// </summary>
        static HSLWebSitePerformanceCounters()
        {
            lock (lockObject)
            {
                HSLWebSitePerformanceCounters.instance = new HSLWebSitePerformanceCounters();
            }
        }

        /// <summary>
        /// default constructor
        /// </summary>
        private HSLWebSitePerformanceCounters()
            : base()
        {
        }

        /// <summary>
        /// Gets an instance of the one and only AuthenticationPerformanceCounters instance.
        /// The instance will be created if necessary.
        /// </summary>
        public static HSLWebSitePerformanceCounters Instance
        {
            get
            {
                return HSLWebSitePerformanceCounters.instance;
            }
        }

        /// <summary>
        /// Gets the Category name for this perf counter class
        /// </summary>
        public override String PerformanceCounterCategoryName
        {
            get
            {
                return Category;
            }
        }

        /// <summary>
        /// Gets the help text for this class's perf counter category
        /// </summary>
        public override String PerformanceCounterCategoryHelpText
        {
            get
            {
                return Category;
            }
        }

        /// <summary>
        /// Gets a list of the Average Timer perf counter this class uses
        /// </summary>
        /// <returns>A string array of perf counter names</returns>
        public override String[] GetAverageTimerPerformanceCounters()
        {
            return allAveTimerCounters;
        }

        /// <summary>
        /// Gets the list of Count Only performance counters
        /// </summary>
        /// <returns>A string array of perf counter names</returns>
        public override String[] GetCountOnlyPerformanceCounters()
        {
            return null;
        }

        /// <summary>
        /// Gets a list of perf counters that will be base and per second counters
        /// </summary>
        /// <returns>A string array of perf counter names</returns>
        public override String[] GetCountAndPerSecondPerformanceCounters()
        {
            return allRegularAndPerSecondCounters;
        }

        /// <summary>
        /// Get the list of per second only perf counters
        /// </summary>
        /// <returns>A string array of perf counter names</returns>
        public override String[] GetPerSecondOnlyPerformanceCounters()
        {
            return null;
        }

        /// <summary>
        /// This overrides the generic error handler from the base class.
        /// It sends any perf counter errors to the HSL Error Handler
        /// </summary>
        /// <param name="errorMsg">The error message to log</param>
        protected override void HandlePerformanceCounterError(String errorMsg)
        {
            this.HandlePerformanceCounterError(errorMsg, null);
        }

        /// <summary>
        /// This overrides the generic error handler from the base class.
        /// It sends any perf counter errors to the HSL Error Handler        
        /// </summary>
        /// <param name="errorMsg">The error message to log</param>
        /// <param name="ex">An exception to log</param>
        protected override void HandlePerformanceCounterError(String errorMsg, Exception ex)
        {
            HSLWebSiteErrorHandler.Instance.WriteToEventLog((UInt16)ErrorID.PerformanceCounterError, errorMsg, ex);
        }
    }
}
