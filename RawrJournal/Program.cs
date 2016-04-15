using Gtk;
using System;
namespace RawrJournal
{
    public class Program
    {
        static void Main()
        {
            Application.Init();
            SplashScreen.showSplash(); // show splash screen
            //ApplicationWindow window = new ApplicationWindow();
            //window.Initialize();

            Application.Run();
        }
    }
}