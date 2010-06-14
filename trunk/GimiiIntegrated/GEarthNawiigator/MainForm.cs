using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EARTHLib;
using WiiApi;
using WiiApi.Tracking3d;
using Microsoft.Xna.Framework;
using GEarthNawiigator.NeuralNetwork;

namespace GEarthNawiigator
{
    public partial class MainForm : Form
    {
        private ApplicationGE geApp;
        private CameraInfoGE geCamera;
        private Tracker3d tracker;

        private Vector3 min;
        private Vector3 max;

        private Vector3 wiipos1;
        private Vector3 wiipos2;

        private CommandCalculator cc;

        private bool gestureMode = false;
        private List<Vector> gestureBuffer;
        private RBFNetwork homeRBF;

        public MainForm(Kontroler l, Kontroler r, Vector3 min, Vector3 max)
        {
            InitializeComponent();
            cc = new CommandCalculator();
            this.min = min;
            this.max = max;

            tracker = new Tracker3d(l, r);
            tracker.Calibrated = true;
            tracker.Filtered = true;
            tracker.Level = TrackerLevel.COOKED;
            tracker.calibrate(min.X, max.X, min.Y, max.Y, min.Z, max.Z);

            geApp = new ApplicationGEClass();
            geCamera = geApp.GetCamera(1);
            updateTimer.Start();

            gestureBuffer = new List<Vector>(200);
            homeRBF = new RBFNetwork("home.xml");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //Hide();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (tracker.found(0))
            {
                wiipos1 = Convert(tracker.getPosition(0));
                cc.wiipos1 = wiipos1;
                cc.history[0].add(wiipos1);
                if (!cc.touchFound1)
                {
                    cc.touch1 = wiipos1;
                    cc.touchFound1 = true;
                }
            }
            else
            {
                cc.touchFound1 = false;
            }
            if (tracker.found(1))
            {
                wiipos2 = Convert(tracker.getPosition(1));
                cc.wiipos2 = wiipos2;
                cc.history[1].add(wiipos2);
                if (!cc.touchFound2)
                {
                    cc.touch2 = wiipos2;
                    cc.touchFound2 = true;
                }
            }
            else
            {
                cc.touchFound2 = false;
            }
            bool changed = false;
            geCamera = geApp.GetCamera(1);
            geCamera.FocusPointAltitudeMode = AltitudeModeGE.RelativeToGroundAltitudeGE;
            if (tracker.found(0) && !tracker.found(1) && !gestureMode)
            {
                //TRANSLATION
                Vector3 tran = cc.getTranslation();
                Matrix rot = new Matrix();
                Matrix x = Matrix.CreateRotationX(MathHelper.ToRadians((float)geCamera.Tilt));
                float azimuth = (float)geCamera.Azimuth;
                azimuth = -azimuth;
                Matrix z = Matrix.CreateRotationZ(MathHelper.ToRadians(azimuth));
                rot = x * z;
                Vector3 tran_rotated = Vector3.Transform(tran, rot);
                geCamera.FocusPointLatitude = geCamera.FocusPointLatitude - tran_rotated.Y / 100 * (geCamera.Range / 1765390);
                geCamera.FocusPointLongitude = geCamera.FocusPointLongitude - tran_rotated.X / 100 * (geCamera.Range / 1765390);

                changed = true;
            }
            if (tracker.found(0) && tracker.found(1))
            {
                //SCALE
                float scale = cc.getScale();
                if (scale != 0.0f && !gestureMode)
                {
                    geCamera.Range *= 1.0f + scale / 10;
                    changed = true;
                }
            }
            if (tracker.found(0) && tracker.found(1))
            {
                //ROTATION
                Vector3 rot = cc.getRotate();
                if (Math.Abs(rot.Z) < 0.5 && !gestureMode)
                {
                    geCamera.Azimuth += 2 * rot.Y;
                    geCamera.Tilt += 2 * rot.X;
                    changed = true;
                }
                else
                {
                        tsmGestureMode.Checked = (rot.Z > 0);
                }
            }
            if (changed)
            {
                geApp.SetCamera(geCamera, 6);
            }
        }

        private Vector3 Convert(Vector x)
        {
            return new Vector3(x.X, x.Y, x.Z);
        }

        private void tsmExit_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            Close();
        }

        private void tsmGestureMode_CheckedChanged(object sender, EventArgs e)
        {
           if (tsmGestureMode.Checked)
            {
                EnterGestureMode();
            }
            else
            {
                ExitGestureMode();
            }
        }

        private void EnterGestureMode()
        {
            gestureMode = true;
            icoNotify.ShowBalloonTip(1000, "Gesture Mode", "Enabled", ToolTipIcon.None);
            gestureTimer.Start();
        }

        private void ExitGestureMode()
        {
            gestureMode = false;
            icoNotify.ShowBalloonTip(1000, "Gesture Mode", "Disabled", ToolTipIcon.None);
            gestureTimer.Stop();
        }

        private void gestureTimer_Tick(object sender, EventArgs e)
        {
            if (tracker.found(0))
            {
                gestureBuffer.Add(tracker.getPosition(0));
            }
            else
            {
                if (gestureBuffer.Count > 0)
                {
                    ProcessGesture();
                    gestureBuffer.Clear();
                }
            }
        }

        private void ProcessGesture()
        {
            if (gestureBuffer.Count < 40)
                return;

            icoNotify.ShowBalloonTip(100, "Gesture points count", gestureBuffer.Count.ToString(), ToolTipIcon.None);
        }
    }
}
