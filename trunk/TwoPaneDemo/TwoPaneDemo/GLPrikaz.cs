﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using WiiApi;
namespace TwoPaneDemo
{
    class GLPrikaz : Form
    {
         // --- Fields ---
        #region Private Static Fields
        private static IntPtr hDC;                                              // Private GDI Device Context
        private static IntPtr hRC;                                              // Permanent Rendering Context
        private static Form form;                                               // Our Current Windows Form
        private static bool[] keys = new bool[256];                             // Array Used For The Keyboard Routine
        private static bool active = true;                                      // Window Active Flag, Set To True By Default
        private static bool fullscreen = false;                                  // Fullscreen Flag, Set To Fullscreen Mode By Default
        private static bool done = false;                                       // Bool Variable To Exit Main Loop

        private static bool light;                                              // Lighting ON/OFF ( NEW )
        private static bool lp;                                                 // L Pressed? ( NEW )
        private static bool fp;                                                 // F Pressed? ( NEW )
        private static float objX1 =  0.0f;
        private static float objY1 = 0.0f;
        private static float objZ1 = 0.0f;
        private static float objX2 = 0.0f;
        private static float objY2 = 0.0f;
        private static float objZ2 = 0.0f;
        private static float z = -5;                                            // Depth Into The Screen
        private static float[] lightAmbient = {0.99f, 0.99f, 0.99f, 1};
        private static float[] lightDiffuse = {1, 1, 1, 1};
        private static float[] lightSpecular = { 1, 1, 1, 1 };
        private static float[] lightDirection = { 0, 0, -3};
        //private static float[] lightPosition = {0, 0, 2, 1};
        private static float[] lightPosition = { -1, 1, 2, 0 };
        private static int filter;                                              // Which Filter To Use
        private static int[] texture = new int[3];                              // Storage For 3 Textures


        static float[] g_red = { 1.0f, 0.0f, 0.0f, 1.0f };
        static float[] g_white = { 1.0f, 1.0f, 1.0f, 1.0f };
        static float[] g_green = { 0.0f, 1.0f, 0.0f, 1.0f };

        static Kontroler wLeft;
        static Kontroler wRight;
        static bool connected = false;
        private delegate void Obrada(ParametriDogadjaja p);

        #endregion Private Static Fields

        // --- Constructors & Destructors ---
        #region GLPrikaz
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public GLPrikaz() {
            this.CreateParams.ClassStyle = this.CreateParams.ClassStyle |       // Redraw On Size, And Own DC For Window.
                User.CS_HREDRAW | User.CS_VREDRAW | User.CS_OWNDC;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);            // No Need To Erase Form Background
            this.SetStyle(ControlStyles.DoubleBuffer, true);                    // Buffer Control
            this.SetStyle(ControlStyles.Opaque, true);                          // No Need To Draw Form Background
            this.SetStyle(ControlStyles.ResizeRedraw, true);                    // Redraw On Resize
            this.SetStyle(ControlStyles.UserPaint, true);                       // We'll Handle Painting Ourselves

            this.Activated += new EventHandler(this.Form_Activated);            // On Activate Event Call Form_Activated
            this.Closing += new CancelEventHandler(this.Form_Closing);          // On Closing Event Call Form_Closing
            this.Deactivate += new EventHandler(this.Form_Deactivate);          // On Deactivate Event Call Form_Deactivate
            this.KeyDown += new KeyEventHandler(this.Form_KeyDown);             // On KeyDown Event Call Form_KeyDown
            this.KeyUp += new KeyEventHandler(this.Form_KeyUp);                 // On KeyUp Event Call Form_KeyUp
            this.Resize += new EventHandler(this.Form_Resize);                  // On Resize Event Call Form_Resize
        }
        #endregion GLPrikaz

        // --- Entry Point ---
        #region Run()
        /// <summary>
        ///     The application's entry point.
        /// </summary>
        /// <param name="commandLineArguments">
        ///     Any supplied command line arguments.
        /// </param>
        [STAThread]
        public static void Run() {
            // Ask The User Which Screen Mode They Prefer
                fullscreen = false;                                             // Windowed Mode

            // Create Our OpenGL Window
            if(!CreateGLWindow("NeHe's Textures, Lighting & Keyboard Tutorial", 640, 480, 16, fullscreen)) {
                return;                                                         // Quit If Window Was Not Created
            }
            Gl.glEnable(Gl.GL_LIGHTING);

            //WII REGION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            WiiFabrika.dobaviInstancu().postaviTipKontrolera(WiiTip.WII_KONTROLER);
            wLeft = WiiFabrika.dobaviInstancu().kreirajKontroler();
            wRight = WiiFabrika.dobaviInstancu().kreirajKontroler();
            //lblMessages.Text = wLeft.Identifikator.ToString() + "\n" + wRight.Identifikator.ToString();
            /*Kontroler k = null;
            while ((k = WiiFabrika.dobaviInstancu().kreirajKontroler()) != null)
            {
                Guid x = k.Identifikator;
                lblMessages.Text += x.ToString() + "\n";
            }*/
            //lblMessages.Text = wLeft.Identifikator.ToString() + " ****** " + wRight.Identifikator.ToString();
            try
            {
                wLeft.kreni(true);
                wRight.kreni(true);
                connected = true;
                wLeft.postaviLED(0, true);
                wLeft.postaviLED(1, false);
                wLeft.postaviLED(2, true);
                wLeft.postaviLED(3, false);
                //wRight.postaviVibrator(true);
                wRight.postaviLED(0, false);
                wRight.postaviLED(1, true);
                wRight.postaviLED(2, false);
                wRight.postaviLED(3, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            while(!done) {                                                      // Loop That Runs While done = false
                Application.DoEvents();                                         // Process Events

                // Draw The Scene.  Watch For ESC Key And Quit Messages From DrawGLScene()
                if((active && (form != null) && !DrawGLScene()) || keys[(int) Keys.Escape]) {   //  Active?  Was There A Quit Received?
                    done = true;                                            // ESC Or DrawGLScene Signalled A Quit
                }
                else {                                                      // Not Time To Quit, Update Screen
                    Gdi.SwapBuffers(hDC);                                   // Swap Buffers (Double Buffering)

                    if(keys[(int) Keys.L] && !lp) {
                        lp = true;
                        light = !light;
                        if(!light) {
                            Gl.glDisable(Gl.GL_LIGHTING);
                        }
                        else {
                            Gl.glEnable(Gl.GL_LIGHTING);
                        }
                    }
                    if(!keys[(int) Keys.L]) {
                        lp = false;
                    }
                    if(keys[(int) Keys.F] && !fp) {
                        fp = true;
                        filter += 1;
                        if(filter > 2) {
                            filter = 0;
                        }
                    }
                    if(!keys[(int) Keys.F]) {
                        fp = false;
                    }
                    if(keys[(int) Keys.PageUp]) {
                        objZ1 -= 0.02f;
                    }
                    if(keys[(int) Keys.PageDown]) {
                        objZ1 += 0.02f;
                    }
                    if(keys[(int) Keys.Up]) {
                        objY1 += 0.01f;
                    }
                    if(keys[(int) Keys.Down]) {
                        objY1 -= 0.01f;
                    }
                    if(keys[(int) Keys.Right]) {
                        objX1 += 0.01f;
                    }
                    if(keys[(int) Keys.Left]) {
                        objX1 -= 0.01f;
                    }

                    if(keys[(int) Keys.F1]) {                               // Is F1 Being Pressed?
                        keys[(int) Keys.F1] = false;                        // If So Make Key false
                        KillGLWindow();                                     // Kill Our Current Window
                        fullscreen = !fullscreen;                           // Toggle Fullscreen / Windowed Mode
                        // Recreate Our OpenGL Window
                        if(!CreateGLWindow("NeHe's Textures, Lighting & Keyboard Tutorial", 640, 480, 16, fullscreen)) {
                            return;                                         // Quit If Window Was Not Created
                        }
                        done = false;                                       // We're Not Done Yet
                    }
                    calculate();
                }
            }

            // Shutdown
            KillGLWindow();                                                     // Kill The Window
            return;                                                             // Exit The Program
        }
        #endregion Run()


        private static void calculate()
        {
            if (wLeft.Stanje.Senzori[0].Nadjen && wRight.Stanje.Senzori[0].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[0].X;
                float ly = wLeft.Stanje.Senzori[0].Y;

                float rx = wRight.Stanje.Senzori[0].X;
                float ry = wRight.Stanje.Senzori[0].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                float z = (cz - 1.5f) / 8.5f;

                float zsize = 15f / z;

                objX1 = (cx - 0.5f) * 30.0f;
                objY1 = -(cy - 0.5f) * 12.0f;
                objZ1 = (z - 0.5f) * 36.0f;
            }

            if (wLeft.Stanje.Senzori[1].Nadjen && wRight.Stanje.Senzori[1].Nadjen)
            {
                float lx = wLeft.Stanje.Senzori[1].X;
                float ly = wLeft.Stanje.Senzori[1].Y;

                float rx = wRight.Stanje.Senzori[1].X;
                float ry = wRight.Stanje.Senzori[1].Y;

                float cx = (lx + rx) / 2;
                float cy = (ly + ry) / 2;

                double alpha = (1 - lx) * Math.PI / 4 + (Math.PI / 8) * 3;
                double beta = rx * Math.PI / 4 + (Math.PI / 8) * 3;

                float cz = (float)(1 / ((1 / Math.Tan(alpha)) + (1 / Math.Tan(beta))));

                float z = (cz - 1.5f) / 8.5f;

                float zsize = 15f / z;

                objX2 = (cx - 0.5f) * 30.0f;
                objY2 = -(cy - 0.5f) * 12.0f;
                objZ2 = (z - 0.5f) * 36.0f;
            }
        }

        // --- Private Static Methods ---
        #region bool CreateGLWindow(string title, int width, int height, int bits, bool fullscreenflag)
        /// <summary>
        ///     Creates our OpenGL Window.
        /// </summary>
        /// <param name="title">
        ///     The title to appear at the top of the window.
        /// </param>
        /// <param name="width">
        ///     The width of the GL window or fullscreen mode.
        /// </param>
        /// <param name="height">
        ///     The height of the GL window or fullscreen mode.
        /// </param>
        /// <param name="bits">
        ///     The number of bits to use for color (8/16/24/32).
        /// </param>
        /// <param name="fullscreenflag">
        ///     Use fullscreen mode (<c>true</c>) or windowed mode (<c>false</c>).
        /// </param>
        /// <returns>
        ///     <c>true</c> on successful window creation, otherwise <c>false</c>.
        /// </returns>
        private static bool CreateGLWindow(string title, int width, int height, int bits, bool fullscreenflag) {
            int pixelFormat;                                                    // Holds The Results After Searching For A Match
            fullscreen = fullscreenflag;                                        // Set The Global Fullscreen Flag
            form = null;                                                        // Null The Form

            GC.Collect();                                                       // Request A Collection
            // This Forces A Swap
            Kernel.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

            if(fullscreen) {                                                    // Attempt Fullscreen Mode?
                Gdi.DEVMODE dmScreenSettings = new Gdi.DEVMODE();               // Device Mode
                // Size Of The Devmode Structure
                dmScreenSettings.dmSize = (short) Marshal.SizeOf(dmScreenSettings);
                dmScreenSettings.dmPelsWidth = width;                           // Selected Screen Width
                dmScreenSettings.dmPelsHeight = height;                         // Selected Screen Height
                dmScreenSettings.dmBitsPerPel = bits;                           // Selected Bits Per Pixel
                dmScreenSettings.dmFields = Gdi.DM_BITSPERPEL | Gdi.DM_PELSWIDTH | Gdi.DM_PELSHEIGHT;

                // Try To Set Selected Mode And Get Results.  NOTE: CDS_FULLSCREEN Gets Rid Of Start Bar.
                if(User.ChangeDisplaySettings(ref dmScreenSettings, User.CDS_FULLSCREEN) != User.DISP_CHANGE_SUCCESSFUL) {
                    // If The Mode Fails, Offer Two Options.  Quit Or Use Windowed Mode.
                    if(MessageBox.Show("The Requested Fullscreen Mode Is Not Supported By\nYour Video Card.  Use Windowed Mode Instead?", "NeHe GL",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
                        fullscreen = false;                                     // Windowed Mode Selected.  Fullscreen = false
                    }
                    else {
                        // Pop up A Message Box Lessing User Know The Program Is Closing.
                        MessageBox.Show("Program Will Now Close.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;                                           // Return false
                    }
                }
            }

            form = new GLPrikaz();                                              // Create The Window

            if(fullscreen) {                                                    // Are We Still In Fullscreen Mode?
                form.FormBorderStyle = FormBorderStyle.None;                    // No Border
                Cursor.Hide();                                                  // Hide Mouse Pointer
            }
            else {                                                              // If Windowed
                form.FormBorderStyle = FormBorderStyle.Sizable;                 // Sizable
                Cursor.Show();                                                  // Show Mouse Pointer
            }

            form.Width = width;                                                 // Set Window Width
            form.Height = height;                                               // Set Window Height
            form.Text = title;                                                  // Set Window Title

            Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();    // pfd Tells Windows How We Want Things To Be
            pfd.nSize = (short) Marshal.SizeOf(pfd);                            // Size Of This Pixel Format Descriptor
            pfd.nVersion = 1;                                                   // Version Number
            pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW |                              // Format Must Support Window
                Gdi.PFD_SUPPORT_OPENGL |                                        // Format Must Support OpenGL
                Gdi.PFD_DOUBLEBUFFER;                                           // Format Must Support Double Buffering
            pfd.iPixelType = (byte) Gdi.PFD_TYPE_RGBA;                          // Request An RGBA Format
            pfd.cColorBits = (byte) bits;                                       // Select Our Color Depth
            pfd.cRedBits = 0;                                                   // Color Bits Ignored
            pfd.cRedShift = 0;
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cAlphaBits = 0;                                                 // No Alpha Buffer
            pfd.cAlphaShift = 0;                                                // Shift Bit Ignored
            pfd.cAccumBits = 0;                                                 // No Accumulation Buffer
            pfd.cAccumRedBits = 0;                                              // Accumulation Bits Ignored
            pfd.cAccumGreenBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 16;                                                // 16Bit Z-Buffer (Depth Buffer)
            pfd.cStencilBits = 0;                                               // No Stencil Buffer
            pfd.cAuxBuffers = 0;                                                // No Auxiliary Buffer
            pfd.iLayerType = (byte) Gdi.PFD_MAIN_PLANE;                         // Main Drawing Layer
            pfd.bReserved = 0;                                                  // Reserved
            pfd.dwLayerMask = 0;                                                // Layer Masks Ignored
            pfd.dwVisibleMask = 0;
            pfd.dwDamageMask = 0;

            hDC = User.GetDC(form.Handle);                                      // Attempt To Get A Device Context
            if(hDC == IntPtr.Zero) {                                            // Did We Get A Device Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Device Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            pixelFormat = Gdi.ChoosePixelFormat(hDC, ref pfd);                  // Attempt To Find An Appropriate Pixel Format
            if(pixelFormat == 0) {                                              // Did Windows Find A Matching Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Find A Suitable PixelFormat.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(!Gdi.SetPixelFormat(hDC, pixelFormat, ref pfd)) {                // Are We Able To Set The Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Set The PixelFormat.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            hRC = Wgl.wglCreateContext(hDC);                                    // Attempt To Get The Rendering Context
            if(hRC == IntPtr.Zero) {                                            // Are We Able To Get A Rendering Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Rendering Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if(!Wgl.wglMakeCurrent(hDC, hRC)) {                                 // Try To Activate The Rendering Context
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Activate The GL Rendering Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            form.Show();                                                        // Show The Window
            form.TopMost = true;                                                // Topmost Window
            form.Focus();                                                       // Focus The Window

            if(fullscreen) {                                                    // This Shouldn't Be Necessary, But Is
                Cursor.Hide();
            }
            ReSizeGLScene(width, height);                                       // Set Up Our Perspective GL Screen

            if(!InitGL()) {                                                     // Initialize Our Newly Created GL Window
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Initialization Failed.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;                                                        // Success
        }
        #endregion bool CreateGLWindow(string title, int width, int height, int bits, bool fullscreenflag)

        #region bool DrawGLScene()
        /// <summary>
        ///     Here's where we do all the drawing.
        /// </summary>
        /// <returns>
        ///     <c>true</c> on successful drawing, otherwise <c>false</c>.
        /// </returns>
        private static bool DrawGLScene()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);        // Clear The Screen And The Depth Buffer
            Gl.glLoadIdentity();// Reset The View
            Gl.glPushMatrix();
            {
                Gl.glTranslatef(objX1, objY1, objZ1 + z);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, g_red);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, g_white);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, 1.0f);
                Glu.gluSphere(Glu.gluNewQuadric(), 0.7f, 100, 100);
            }
            Gl.glPopMatrix();


            Gl.glPushMatrix();
            {
                Gl.glTranslatef(objX2, objY2, objZ2 + z);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, g_green);
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, g_white);
                Gl.glMaterialf(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, 1.0f);
                Glu.gluSphere(Glu.gluNewQuadric(), 0.7f, 100, 100);
            }
            Gl.glPopMatrix();

            return true;                                                        // Keep Going
        }
        #endregion bool DrawGLScene()

        #region bool InitGL()
        /// <summary>
        ///     All setup for OpenGL goes here.
        /// </summary>
        /// <returns>
        ///     <c>true</c> on successful initialization, otherwise <c>false</c>.
        /// </returns>
        private static bool InitGL() {
            if(!LoadGLTextures()) {                                             // Jump To Texture Loading Routine
                return false;                                                   // If Texture Didn't Load Return False
            }

            Gl.glEnable(Gl.GL_TEXTURE_2D);                                      // Enable Texture Mapping
            Gl.glShadeModel(Gl.GL_SMOOTH);                                      // Enable Smooth Shading
            Gl.glClearColor(0, 0, 0, 0.5f);                                     // Black Background
            Gl.glClearDepth(1);                                                 // Depth Buffer Setup
            Gl.glEnable(Gl.GL_DEPTH_TEST);                                      // Enables Depth Testing
            Gl.glDepthFunc(Gl.GL_LEQUAL);                                       // The Type Of Depth Testing To Do
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);         // Really Nice Perspective Calculations
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, lightAmbient);            // Setup The Ambient Light
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, lightDiffuse);            // Setup The Diffuse Light
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, lightSpecular);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPOT_DIRECTION, lightDirection);
            Gl.glLightf(Gl.GL_LIGHT1, Gl.GL_SPOT_CUTOFF, 180.0f);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, lightPosition);          // Position The Light
            Gl.glEnable(Gl.GL_LIGHT1);                                          // Enable Light One

            return true;
        }
        #endregion bool InitGL()

        #region KillGLWindow()
        /// <summary>
        ///     Properly kill the window.
        /// </summary>
        private static void KillGLWindow() {
            if (connected)
            {
                wLeft.prekiniKomunikaciju();
                wRight.prekiniKomunikaciju();
            }
            if(fullscreen) {                                                    // Are We In Fullscreen Mode?
                User.ChangeDisplaySettings(IntPtr.Zero, 0);                     // If So, Switch Back To The Desktop
                Cursor.Show();                                                  // Show Mouse Pointer
            }

            if(hRC != IntPtr.Zero) {                                            // Do We Have A Rendering Context?
                if(!Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero)) {             // Are We Able To Release The DC and RC Contexts?
                    MessageBox.Show("Release Of DC And RC Failed.", "SHUTDOWN ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if(!Wgl.wglDeleteContext(hRC)) {                                // Are We Able To Delete The RC?
                    MessageBox.Show("Release Rendering Context Failed.", "SHUTDOWN ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                hRC = IntPtr.Zero;                                              // Set RC To Null
            }

            if(hDC != IntPtr.Zero) {                                            // Do We Have A Device Context?
                if(form != null && !form.IsDisposed) {                          // Do We Have A Window?
                    if(form.Handle != IntPtr.Zero) {                            // Do We Have A Window Handle?
                        if(!User.ReleaseDC(form.Handle, hDC)) {                 // Are We Able To Release The DC?
                            MessageBox.Show("Release Device Context Failed.", "SHUTDOWN ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                hDC = IntPtr.Zero;                                              // Set DC To Null
            }

            if(form != null) {                                                  // Do We Have A Windows Form?
                form.Hide();                                                    // Hide The Window
                form.Close();                                                   // Close The Form
                form = null;                                                    // Set form To Null
            }
        }
        #endregion KillGLWindow()

        #region Bitmap LoadBMP(string fileName)
        /// <summary>
        ///     Loads a bitmap image.
        /// </summary>
        /// <param name="fileName">
        ///     The filename to load.
        /// </param>
        /// <returns>
        ///     The bitmap if it exists, otherwise <c>null</c>.
        /// </returns>
        private static Bitmap LoadBMP(string fileName) {
            if(fileName == null || fileName == string.Empty) {                  // Make Sure A Filename Was Given
                return null;                                                    // If Not Return Null
            }

            string fileName1 = string.Format("Data{0}{1}",                      // Look For Data\Filename
                Path.DirectorySeparatorChar, fileName);
            string fileName2 = string.Format("{0}{1}{0}{1}Data{1}{2}",          // Look For ..\..\Data\Filename
                "..", Path.DirectorySeparatorChar, fileName);

            // Make Sure The File Exists In One Of The Usual Directories
            if(!File.Exists(fileName) && !File.Exists(fileName1) && !File.Exists(fileName2)) {
                return null;                                                    // If Not Return Null
            }

            if(File.Exists(fileName)) {                                         // Does The File Exist Here?
                return new Bitmap(fileName);                                    // Load The Bitmap
            }
            else if(File.Exists(fileName1)) {                                   // Does The File Exist Here?
                return new Bitmap(fileName1);                                   // Load The Bitmap
            }
            else if(File.Exists(fileName2)) {                                   // Does The File Exist Here?
                return new Bitmap(fileName2);                                   // Load The Bitmap
            }

            return null;                                                        // If Load Failed Return Null
        }
        #endregion Bitmap LoadBMP(string fileName)

        #region bool LoadGLTextures()
        /// <summary>
        ///     Load bitmaps and convert to textures.
        /// </summary>
        /// <returns>
        ///     <c>true</c> on success, otherwise <c>false</c>.
        /// </returns>
        private static bool LoadGLTextures() {
            bool status = false;                                                // Status Indicator
            Bitmap[] textureImage = new Bitmap[1];                              // Create Storage Space For The Texture

            textureImage[0] = LoadBMP("NeHe.Lesson07.Crate.bmp");               // Load The Bitmap
            // Check For Errors, If Bitmap's Not Found, Quit
            if(textureImage[0] != null) {
                status = true;                                                  // Set The Status To True

                Gl.glGenTextures(3, texture);                                   // Create Three Textures

                textureImage[0].RotateFlip(RotateFlipType.RotateNoneFlipY);     // Flip The Bitmap Along The Y-Axis
                // Rectangle For Locking The Bitmap In Memory
                Rectangle rectangle = new Rectangle(0, 0, textureImage[0].Width, textureImage[0].Height);
                // Get The Bitmap's Pixel Data From The Locked Bitmap
                BitmapData bitmapData = textureImage[0].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                // Create Nearest Filtered Texture
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, textureImage[0].Width, textureImage[0].Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                // Create Linear Filtered Texture
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[1]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, textureImage[0].Width, textureImage[0].Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                // Create MipMapped Texture
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[2]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, textureImage[0].Width, textureImage[0].Height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                if(textureImage[0] != null) {                                   // If Texture Exists
                    textureImage[0].UnlockBits(bitmapData);                     // Unlock The Pixel Data From Memory
                    textureImage[0].Dispose();                                  // Dispose The Bitmap
                }
            }

            return status;                                                      // Return The Status
        }
        #endregion bool LoadGLTextures()

        #region ReSizeGLScene(int width, int height)
        /// <summary>
        ///     Resizes and initializes the GL window.
        /// </summary>
        /// <param name="width">
        ///     The new window width.
        /// </param>
        /// <param name="height">
        ///     The new window height.
        /// </param>
        private static void ReSizeGLScene(int width, int height) {
            if(height == 0) {                                                   // Prevent A Divide By Zero...
                height = 1;                                                     // By Making Height Equal To One
            }

            Gl.glViewport(0, 0, width, height);                                 // Reset The Current Viewport
            Gl.glMatrixMode(Gl.GL_PROJECTION);                                  // Select The Projection Matrix
            Gl.glLoadIdentity();                                                // Reset The Projection Matrix
            Glu.gluPerspective(45, width / (double) height, 0.1, 100);          // Calculate The Aspect Ratio Of The Window
            Gl.glMatrixMode(Gl.GL_MODELVIEW);                                   // Select The Modelview Matrix
            Gl.glLoadIdentity();                                                // Reset The Modelview Matrix
        }
        #endregion ReSizeGLScene(int width, int height)

        // --- Private Instance Event Handlers ---
        #region Form_Activated
        /// <summary>
        ///     Handles the form's activated event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_Activated(object sender, EventArgs e) {
            active = true;                                                      // Program Is Active
        }
        #endregion Form_Activated

        #region Form_Closing(object sender, CancelEventArgs e)
        /// <summary>
        ///     Handles the form's closing event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_Closing(object sender, CancelEventArgs e) {
            done = true;                                                        // Send A Quit Message
        }
        #endregion Form_Closing(object sender, CancelEventArgs e)

        #region Form_Deactivate(object sender, EventArgs e)
        /// <summary>
        ///     Handles the form's deactivate event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_Deactivate(object sender, EventArgs e) {
            active = false;                                                     // Program Is No Longer Active
        }
        #endregion Form_Deactivate(object sender, EventArgs e)

        #region Form_KeyDown(object sender, KeyEventArgs e)
        /// <summary>
        ///     Handles the form's key down event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_KeyDown(object sender, KeyEventArgs e) {
            keys[e.KeyValue] = true;                                            // Key Has Been Pressed, Mark It As true
        }
        #endregion Form_KeyDown(object sender, KeyEventArgs e)

        #region Form_KeyUp(object sender, KeyEventArgs e)
        /// <summary>
        ///     Handles the form's key down event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_KeyUp(object sender, KeyEventArgs e) {
            keys[e.KeyValue] = false;                                           // Key Has Been Released, Mark It As false
        }
        #endregion Form_KeyUp(object sender, KeyEventArgs e)

        #region Form_Resize(object sender, EventArgs e)
        /// <summary>
        ///     Handles the form's resize event.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="e">
        ///     The event arguments.
        /// </param>
        private void Form_Resize(object sender, EventArgs e) {
            ReSizeGLScene(form.Width, form.Height);                             // Resize The OpenGL Window
        }
        #endregion Form_Resize(object sender, EventArgs e)
    }
}
