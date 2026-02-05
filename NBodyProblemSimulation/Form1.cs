using NBodyProblemSimulation.Classes;
using NBodyProblemSimulation.Physics;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace NBodyProblemSimulation
{
    public partial class Form1 : Form
    {
        // Variables
        private PhysicsEngine physicsEngine = new PhysicsEngine();
        private bool showAccelerationVector = true;
        private float integrationDelta = 10f; // Number of years per frame

        private TrackBar integrationTrackbar;
        private Timer timer;
        private DateTime lastFrameTime;
        private Label lblTime;
        private Label lblStarCount;
        private Label lblScenario;
        private Label lblIntegrationMethod;
        private Label lblIntegrationDelta;
        private Button btnAddStar;
        private Bitmap? _backgroundCache = null;
        private float simulationTime = 0f;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black; // Set bg

            // Timer label
            lblTime = new Label
            {
                ForeColor = Color.White,
                Location = new Point(10, 30),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            lblTime.BackColor = Color.Transparent;
            lblTime.Text = "Time: 0.0 s";
            lblTime.BringToFront();
            Controls.Add(lblTime);

            // Star count label
            lblStarCount = new Label
            {
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(10, 50),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Text = $"Stars counter: {physicsEngine.Bodies.Count}"
            };
            Controls.Add(lblStarCount);

            // Add star button
            btnAddStar = new Button
            {
                Text = "Add Star",
                Location = new Point(10, 70),
            };
            btnAddStar.Click += BtnAddStar_Click;
            btnAddStar.BackColor = Color.DarkGray;
            btnAddStar.ForeColor = Color.White;
            Controls.Add(btnAddStar);

            // Add a button to toggle acceleration vector visibility
            Button btnToggleAccelerationVectors = new Button
            {
                Text = "Toggle Acceleration Vectors",
                Location = new Point(10, 90),
            };
            btnToggleAccelerationVectors.BackColor = Color.DarkGray;
            btnToggleAccelerationVectors.ForeColor = Color.White;
            btnToggleAccelerationVectors.Click += (s, e) =>
            {
                showAccelerationVector = !showAccelerationVector;
                Invalidate();  // Redraw the form to show/hide vectors
            };
            Controls.Add(btnToggleAccelerationVectors);

            // Add a button to switch scenarios
            Button btnSwitchScenario = new Button
            {
                Text = "Switch Scenario",
                Location = new Point(10, 110),
                BackColor = Color.DarkGray,
                ForeColor = Color.White
            };
            btnSwitchScenario.Click += (s, e) =>
            {
                physicsEngine.SwitchScenario();
                simulationTime = 0f; // Reset simulation time
                Invalidate();
            };
            Controls.Add(btnSwitchScenario);

            // Label to show current scenario
            lblScenario = new Label
            {
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(10, 130),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Text = $"Scenario number: {physicsEngine.ScenarioIndex + 1}"
            };
            Controls.Add(lblScenario);

            // Add a button to switch integration methods
            Button btnSwitchIntegration = new Button
            {
                Text = "Switch Integrator",
                Location = new Point(10, 160),
                BackColor = Color.DarkGray,
                ForeColor = Color.White
            };
            btnSwitchIntegration.Click += (s, e) =>
            {
                physicsEngine.SwitchIntegrationMethod();
                Invalidate();
            };
            Controls.Add(btnSwitchIntegration);

            // Label to show integration method
            lblIntegrationMethod = new Label
            {
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(10, 180),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Text = $"Integration method: {physicsEngine.CurrentIntegrationMethodName}"
            };
            Controls.Add(lblIntegrationMethod);

            // Trackbar to show integration delta
            integrationTrackbar = new TrackBar
            {
                Minimum = 1,
                Maximum = 500,
                Value = 50,
                TickFrequency = 100,
                Dock = DockStyle.Top,
            };
            integrationTrackbar.Scroll += TrackBar_Scroll;
            Controls.Add(integrationTrackbar);

            // Add label to display the integration delta
            lblIntegrationDelta = new Label
            {
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(10, 200),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Text = $"Integration Delta: {integrationTrackbar.Value / 10.0f:F1} years"
            };
            Controls.Add(lblIntegrationDelta);

            // Initialize Physics Engine with the first scenario
            physicsEngine.LoadFirstScenario();

            // Timer setup
            timer = new Timer();
            timer.Interval = 8; // 60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();

            lastFrameTime = DateTime.Now;
            DoubleBuffered = true;
        }

        private void TrackBar_Scroll(object? sender, EventArgs e)
        {
            // Map the TrackBar value (1-500) to a float value (0.1 to 50.0 years)
            integrationDelta = integrationTrackbar.Value / 10.0f;

            // Update the label to show the new integration delta
            lblIntegrationDelta.Text = $"Integration Delta: {integrationDelta:F1} years";
        }

        private void BtnAddStar_Click(object? sender, EventArgs e)
        {
            physicsEngine.AddSunlikeBody();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            float deltaTime = (float)(now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // Update Physics
            float timeStep = integrationDelta;
            float targetTimeStep = 0.01f;
            int RequiredSubSteps = (int) Math.Ceiling(timeStep / targetTimeStep);

            for (int i = 0; i < RequiredSubSteps; i++)
            {
                physicsEngine.Update(targetTimeStep, i == 0 || i == (RequiredSubSteps / 2) );
            }


            // Update sim labels
            simulationTime += deltaTime;
            lblTime.Text = $"Time: {simulationTime:F1} s";
            lblStarCount.Text = $"Stars counter: {physicsEngine.Bodies.Count}";
            lblScenario.Text = $"Scenario number: {physicsEngine.ScenarioIndex + 1}";
            lblIntegrationMethod.Text = $"Integration method: {physicsEngine.CurrentIntegrationMethodName}";

            Invalidate(); // cause repaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 1. Setup Graphics
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 2. Draw Background (Cached)
            if (_backgroundCache == null || _backgroundCache.Width != ClientSize.Width || _backgroundCache.Height != ClientSize.Height)
            {
                _backgroundCache = new Bitmap(Math.Max(1, ClientSize.Width), Math.Max(1, ClientSize.Height));
                using (var bgG = Graphics.FromImage(_backgroundCache))
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(-Width / 2, -Height / 2, Width * 2, Height * 2);
                    using (var brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.FromArgb(30, 30, 60);
                        brush.SurroundColors = new[] { Color.Black };
                        bgG.FillRectangle(brush, ClientRectangle);
                    }
                }
            }
            // Blit the cached image (Instant operation)
            g.DrawImageUnscaled(_backgroundCache, 0, 0);


            // 3. Draw Stars
            foreach (CelestialBody star in physicsEngine.Bodies)
            {
                // --- OPTIMIZED TRAIL DRAWING ---
                // Instead of 1000 DrawLine calls, we do ~10 DrawLines calls (Batches)
                if (star.Trail.Count > 1)
                {
                    int batches = 10; // Divide trail into 10 transparency segments
                    int pointsPerBatch = star.Trail.Count / batches;

                    if (pointsPerBatch > 1)
                    {
                        for (int b = 0; b < batches; b++)
                        {
                            // Calculate opacity for this entire batch
                            float alphaPct = (float)b / batches;
                            int alpha = (int)(alphaPct * 255);
                            if (alpha < 5) continue; // Skip invisible parts

                            // Create points for this segment
                            int startIndex = b * pointsPerBatch;
                            // Ensure batches overlap by 1 point so there are no gaps
                            int count = (b == batches - 1) ? (star.Trail.Count - startIndex) : (pointsPerBatch + 1);

                            if (count < 2) continue;

                            PointF[] batchPoints = new PointF[count];
                            for (int j = 0; j < count; j++)
                            {
                                var vec = star.Trail[startIndex + j];
                                // Apply Offset directly here
                                batchPoints[j] = new PointF(vec.X + star.Radius, vec.Y + star.Radius);
                            }

                            using (var pen = new Pen(Color.FromArgb(alpha, star.ColorHex), 1.5f))
                            {
                                g.DrawLines(pen, batchPoints);
                            }
                        }
                    }
                }

                // --- STAR GLOW & BODY ---
                // (Optimized: Removed repeated math)
                float sX = star.Position.X;
                float sY = star.Position.Y;

                // Draw glow (Outer to Inner)
                for (int i = 3; i > 0; i--)
                {
                    int glowSize = (int)(star.Radius * 2 + i * 6);
                    int offset = (glowSize - (int)(star.Radius * 2)) / 2;

                    using (var brush = new SolidBrush(Color.FromArgb(50 / i, star.ColorHex)))
                    {
                        g.FillEllipse(brush, sX - offset, sY - offset, glowSize, glowSize);
                    }
                }

                // Draw solid body
                using (var brush = new SolidBrush(star.ColorHex))
                {
                    g.FillEllipse(brush, sX, sY, star.Radius * 2, star.Radius * 2);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _backgroundCache?.Dispose();
            _backgroundCache = null;
            Invalidate();
        }
    }
}
