namespace WpfMinesweeper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using Models;
    using ViewModels;
    using Views;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static TraceSource Tracer;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SettingsProvider.Instance.Save();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //this.InitializeTracer();

            var window = new MainWindow();
            var view = new MainWindowViewModel();
            window.DataContext = view;
            window.Show();
        }

        private void InitializeTracer()
        {
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;

            App.Tracer = new TraceSource("Tracer", SourceLevels.All);
            App.Tracer.Listeners.Clear();

            var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Log.txt", FileMode.Create, FileAccess.Write, FileShare.None);
            var listener = new TextWriterTraceListener(fs);
            App.Tracer.Listeners.Add(listener);
        }
    }
}
