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
            physicsEngine.Update(timeStep);


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
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background Graphics
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(-ClientSize.Width / 2, -ClientSize.Height / 2, ClientSize.Width * 2, ClientSize.Height * 2);

                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(30, 30, 60);   // center
                    brush.SurroundColors = new[] { Color.Black };     // edges

                    g.FillRectangle(brush, ClientRectangle);
                }
            }

            // Loop through stars to draw
            foreach (CelestialBody star in physicsEngine.Bodies)
            {
                // Draw star trail
                for (int i = 0; i < star.Trail.Count - 1; i++)
                {
                    // Fade trail: older points are more transparent
                    float alpha = (float)i / star.Trail.Count;
                    using (var pen = new Pen(Color.FromArgb((int)(alpha * 255), star.ColorHex)))
                    {
                        g.DrawLine(pen,
                            star.Trail[i].X + star.Radius,
                            star.Trail[i].Y + star.Radius,
                            star.Trail[i + 1].X + star.Radius,
                            star.Trail[i + 1].Y + star.Radius);
                    }
                }

                // Star Graphics
                // Draw glow layers
                int glowLayers = 3;
                for (int i = glowLayers; i > 0; i--)
                {
                    int glowRadius = (int)(star.Radius * 2.0 + i * 6); // larger than star
                    int alpha = 50 / i; // outer layers more transparent
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, star.ColorHex)))
                    {
                        g.FillEllipse(brush,
                            star.Position.X + star.Radius - glowRadius / 2,
                            star.Position.Y + star.Radius - glowRadius / 2,
                            glowRadius,
                            glowRadius);
                    }
                }

                using (var brush = new SolidBrush(star.ColorHex))
                {
                    g.FillEllipse(brush, star.Position.X, star.Position.Y, star.Radius * 2, star.Radius * 2);
                }
            }
        }
    }
}
