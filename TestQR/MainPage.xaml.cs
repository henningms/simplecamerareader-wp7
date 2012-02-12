using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using zxingwp7.SimpleCameraReader;
using zxingwp7;

namespace TestQR
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Test app for using SimpleCameraReader
        // Written by Henning M. Stephansen
        // http://www.henning.ms

        // UI and ideas for SimpleCameraReader: 
        // http://jonas.follesoe.no/2011/07/22/qr-code-scanning-on-windows-phone-75-using-zxlib/
        // Jonas Follesoe

        private readonly ObservableCollection<string> _matches;
        private SimpleCameraReader _reader;

        public MainPage()
        {
            InitializeComponent();

            // Holds a list of codes we've decoded
            // Displays it on top of the camera feed
            _matches = new ObservableCollection<string>();
            _matchesList.ItemsSource = _matches;


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Initialize a new instance of SimpleCameraReader with Auto-Focus mode on
            _reader = new SimpleCameraReader(true);

            // We need to set the VideoBrush we're going to display the preview feed on
            // IMPORTANT that it gets set before Camera initializes
            _previewVideo.SetSource(_reader.Camera);

            // The reader throws an event when a result is available 
            _reader.DecodingCompleted += (o, r) => DisplayResult(r.Text);

            // The reader throws an event when the camera is initialized and ready to use
            _reader.CameraInitialized += ReaderOnCameraInitialized;

            base.OnNavigatedTo(e);


        }

        private void ReaderOnCameraInitialized(object sender, bool initialized)
        {
            // We dispatch (invoke) to avoid access exceptions
            Dispatcher.BeginInvoke(() =>
            {
                _previewTransform.Rotation = _reader.CameraOrientation;
            });

            // We can set if Camera should flash or not when focused
            _reader.FlashMode = FlashMode.On;

            // Starts the capturing process
            _reader.Start();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _reader.Stop();

            base.OnNavigatingFrom(e);
        }

        private void DisplayResult(string text)
        {
            // Adds the code to the list if it doesn't already exists
            if (!_matches.Contains(text))
                _matches.Add(text);
        }
    }
}