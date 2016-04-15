using Gtk;
using System;
using System.Xml;
using System.Collections.Generic;
using iTunesLib;
using Pango;
using WebKit;
namespace RawrJournal
{
    public class PreviewWindow
    {
        #region Public Properties
        private string _markupstring = "";
        public string MarkupString
        {
            get { return _markupstring; }
            set { _markupstring = value; }
        }
        #endregion Public Properties

        #region Properties	
        Pango.Layout layout;
        DrawingArea da;
        #endregion Properties

        #region Methods
        public void Initialize()
        {
            //window
            Window window = new Window("Preview");
            window.Resize(500, 500);
            window.WindowPosition = WindowPosition.Center;

            ScrolledWindow sw = new ScrolledWindow();
            WebView wv = new WebView();
            sw.Add(wv);
            window.Add(sw);

            wv.Open("http://www.yahoo.com");

            //da = new Gtk.DrawingArea();
            //da.SetSizeRequest(500, 500);
            //da.ExposeEvent += Expose_Event;

            //layout = new Pango.Layout(window.PangoContext);
            //layout.Width = Pango.Units.FromPixels(500);
            //layout.Wrap = Pango.WrapMode.Word;
            //layout.Alignment = Pango.Alignment.Left;
            //layout.FontDescription = Pango.FontDescription.FromString("Arial 10");
            //layout.SetMarkup(_markupstring);

            //window.Add(da);
            window.ShowAll();
        }
        #endregion Methods

        #region Event Handlers
        void Expose_Event(object obj, ExposeEventArgs args)
        {
            da.GdkWindow.DrawLayout(da.Style.TextGC(StateType.Normal), 5, 5, layout);
        }
        #endregion Event Handlers
    }
}