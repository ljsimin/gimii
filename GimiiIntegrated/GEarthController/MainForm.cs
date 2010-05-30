using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EARTHLib;

namespace GEarthController
{
    public partial class MainForm : Form
    {
        private ApplicationGE geApp;
        private CameraInfoGE geCamera;

        private bool changing = false;

        public MainForm()
        {
            InitializeComponent();
            geApp = new ApplicationGEClass();
            geCamera = geApp.GetCamera(1);
            UpdateChange();
            timerUpdate.Start();
        }

        public void UpdateChange()
        {
            sliderLat.Value = (int)geCamera.FocusPointLatitude;
            txtLatitude.Text = geCamera.FocusPointLatitude.ToString();
            sliderLong.Value = (int)geCamera.FocusPointLongitude;
            txtLongtitude.Text = geCamera.FocusPointLongitude.ToString();
            int zoom = (int)geCamera.Range;
            sliderZoom.Value = (zoom > sliderZoom.Maximum) ? sliderZoom.Maximum : zoom;
            txtZoom.Text = geCamera.Range.ToString();
            sliderAzimuth.Value = (int)geCamera.Azimuth;
            txtAzimuth.Text = geCamera.Azimuth.ToString();
            sliderTilt.Value = (int)geCamera.Tilt;
            txtTilt.Text = geCamera.Tilt.ToString();
            
        }

        public void ExecuteChange()
        {
            int latitude = sliderLat.Value;
            int longitude = sliderLong.Value;
            int zoom = sliderZoom.Value;
            int azimuth = sliderAzimuth.Value;
            int tilt = sliderTilt.Value;

            geCamera.FocusPointLatitude = latitude;
            geCamera.FocusPointLongitude = longitude;
            geCamera.Range = zoom;
            geCamera.Azimuth = azimuth;
            geCamera.Tilt = tilt;
            geCamera.FocusPointAltitudeMode = AltitudeModeGE.RelativeToGroundAltitudeGE;
        }

        private void onChange(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            ExecuteChange();
            geApp.SetCamera(geCamera, 6);
            UpdateChange();
            timerUpdate.Start();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (!changing)
            {
                geCamera = geApp.GetCamera(1);
                UpdateChange();
            }
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            //changing = true;
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            //changing = false;
        }
    }
}
