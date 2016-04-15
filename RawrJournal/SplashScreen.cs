using Gtk;
using System;
using System.Drawing;
using System.Drawing.Imaging;
// In order to compile this, you have to enable unsafe code because it accesses memory to do the image stuff.
// this is because i used a resource file. you could just as easily load the png as a gdk.pixbuf without issue.
// unsafe code is in right click Rawrjournal->properties->build->allow unsafe code.
// unfortunately, there is no better/cleaner way to do this than this method that i could find, save leaving the
// images in a directory.
namespace RawrJournal
{
    public class SplashScreen
    {
        static Window splashWin = new Gtk.Window("splash");
        static Gdk.Pixbuf splashImg = new Gdk.Pixbuf("../../Images/splash.png");
        
        //static Gdk.Pixbuf CreateFromResource( Bitmap bitmap )
        //{ // Method taken from the web and fixed, orig (c) Cody Russell http://www.gnome.org/~bratsche/blog/hacking/17
        //    // this function converts a System.Drawing.Bitmap object to Gdk.Pixbuf. Pretty handy.
        //    BitmapData data = bitmap.LockBits(new Rectangle(0,0,
        //                                                        bitmap.Width, bitmap.Height),ImageLockMode.ReadWrite,
        //                                                        PixelFormat.Format24bppRgb);

        //      IntPtr scan = data.Scan0;
        //      int size = bitmap.Width * bitmap.Height * 3;
        //      byte[] bdata = new byte[ size ];

        //      Gdk.Pixbuf pixbuf = null;

        //      unsafe
        //      {
        //        byte* p = (byte*)scan;
        //        for( int i = 0; i < size; i++ )
        //          bdata[ i ] = p[ i ];
        //      }

        //      pixbuf = new Gdk.Pixbuf( bdata, false, 8,
        //                               bitmap.Width, bitmap.Height,
        //                               data.Stride, null );

        //      bitmap.UnlockBits( data );

        //      return pixbuf;
        //}

        public static void showSplash()
        {
            VBox splashBox = new VBox();
            Gtk.Image imgConv = new Gtk.Image(splashImg); // this line is supposed to make a gtk.image widget. it doesn't.
            splashBox.PackStart(imgConv,false,false,0);
            splashWin.Decorated = false;
            splashWin.Resize(splashImg.Width, splashImg.Height);
            splashWin.Add(splashBox);
            splashWin.WindowPosition = WindowPosition.Center;
            splashWin.ShowAll();
            splashWin.Destroyed += new EventHandler(delete_splash);
            // just for testing, if I had ever gotten this to work, this would show for 3 seconds
            // (3000 miliseconds), then call killsplash in order to kill it. since i never got
            // the code working, no idea if it actually does.
            GLib.Timeout.Add(3000,killSplash);
        }

        static bool killSplash()
        {
            splashWin.Destroy();
            return true;
        }

        static void delete_splash(object obj, EventArgs args)
        {
            (new ApplicationWindow()).Initialize();
        }
    }
    
}
