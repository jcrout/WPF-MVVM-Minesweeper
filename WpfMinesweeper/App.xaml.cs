using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfMinesweeper.Properties;

namespace WpfMinesweeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static TraceSource Tracer;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.InitializeTracer();

            var window = new MainWindow();
            var view = new WpfMinesweeper.ViewModels.MainWindowViewModel();
            window.DataContext = view;
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {            
            base.OnExit(e);
            SettingsProvider.Instance.Save();
        }

        private void InitializeTracer()
        {
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;

            Tracer = new TraceSource("Tracer", SourceLevels.All);           
            Tracer.Listeners.Clear();

            var fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"Log.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            var listener = new TextWriterTraceListener(fs);
            Tracer.Listeners.Add(listener);
        }
    }
}
