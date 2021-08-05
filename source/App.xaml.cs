using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using LenovoController.Features;

namespace LenovoController
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // defines for commandline output
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorText = e.Exception.ToString();
            Trace.TraceError(errorText);
            MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
        }

        // Contains CLI logic for use when arguments are passed, else it opens the GUI
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool isCLI = (e.Args.Length > 0);
            if (isCLI)
            {

                string cliHelp = "\n\rLenovoController\n\r\n\rUsage:\n\rbatterymode: Gets current battery mode";

                // Fix the console (which returns immediately for Windows Applications)
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Write("\r");
                Console.Write(new string(' ', Console.WindowWidth));
                Console.Write("\r");

                // Initialize features
                AlwaysOnUsbFeature _alwaysOnUsbFeature = new AlwaysOnUsbFeature();
                BatteryFeature _batteryFeature = new BatteryFeature();
                PowerModeFeature _powerModeFeature = new PowerModeFeature();
                FnLockFeature _fnLockFeature = new FnLockFeature();
                OverDriveFeature _overDriveFeature = new OverDriveFeature();
                TouchpadLockFeature _touchpadLockFeature = new TouchpadLockFeature();

                // First argument is the feature, second is optional value to set feature to
                switch (e.Args[0])
                {
                    case "BatteryMode":
                        if(e.Args.Length == 2)
                        {
                            string[] ValidStates = { "Conservation", "Normal", "RapidCharge" };
                            if (Array.Exists(ValidStates, elem => elem == e.Args[1]))
                            {
                                int index = Array.IndexOf(ValidStates, e.Args[1]);
                                try
                                {
                                    _batteryFeature.SetState((BatteryState)index);
                                    Console.WriteLine("Success.");
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Battery mode not supported.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid battery mode.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Battery mode is " + _batteryFeature.GetState().ToString());
                        }
                        break;
                    case "--help":
                        printHelp();
                        break;
                    default:
                        Console.WriteLine("Invalid command use --help to list commands.");
                        break;
                }

                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                Application.Current.Shutdown();
            }
            else
            {
                // Create main application window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    
        private void printHelp()
        {
            string[] helpStrings =
            {
                "LenovoController - Help",
                "Usage:",
                " ",
                "BatteryMode                 - Gets current battery mode",
                "             Conservation   - Sets mode to conservation",
                "             Normal         - Sets mode to normal",
                "             RapidCharge    - Sets mode to rapid charge",
                " "
            };

            foreach (var item in helpStrings)
            {
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                Console.Write("\r");
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, Console.CursorTop - helpStrings.Length);

            foreach (var item in helpStrings)
            {
                Console.WriteLine(item);
            }

        }
    
    }
}