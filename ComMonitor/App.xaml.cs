﻿using System;
using System.Windows;

namespace ComMonitor
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public NLog.Logger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            try
            {
                NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.config");
                _logger = NLog.LogManager.GetCurrentClassLogger();

                Dispatcher.UnhandledException += OnDispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentDomainUnhandledException);

                string StrVersion;
#if DEBUG
                StrVersion = "Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#else
                StrVersion = "Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " Revision " + LocalTools.Globals._revision;
#endif
                _logger.Info("Starting ComMonitor " + StrVersion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~App()
        {
        }

        /// <summary>
        /// OnDispatcherUnhandledException
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string ErrorMessage = string.Format("An unhandled exception occurred in OnDispatcherUnhandledException (MainThread) {0}", e.Exception.Message);
            System.Diagnostics.Debug.WriteLine(ErrorMessage);
            _logger.Error(ErrorMessage);
            ErrorMessage = string.Format("StackTrace: {0}", e.Exception.StackTrace);
            _logger.Error(ErrorMessage);
            if (e.Exception.InnerException != null)
            {
                ErrorMessage = string.Format("InnerException: {0}", e.Exception.InnerException.Message);
                _logger.Error(ErrorMessage);
                ErrorMessage = string.Format("StackTrace: {0}", e.Exception.InnerException.StackTrace);
                _logger.Error(ErrorMessage);
            }
        }

        /// <summary>
        /// OnCurrentDomainUnhandledException
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string ErrorMessage = string.Format("An unhandled exception occurred in OnAppDomainUnhandledException in a working thread : {0}", e.ToString());
            System.Diagnostics.Debug.WriteLine(ErrorMessage);
            _logger.Error(ErrorMessage);
            Exception ex = e.ExceptionObject as Exception;
            ErrorMessage = string.Format("StackTrace: {0}", ex.StackTrace);
            _logger.Error(ErrorMessage);
            if (ex.InnerException as Exception != null)
            {
                ErrorMessage = string.Format("InnerException: {0}", ex.InnerException.Message);
                _logger.Error(ErrorMessage);
                ErrorMessage = string.Format("StackTrace: {0}", ex.InnerException.StackTrace);
                _logger.Error(ErrorMessage);
            }
            Environment.Exit(0);
        }
    }
}
