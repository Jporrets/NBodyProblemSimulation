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
        private int starCount = 0;
        private bool showAccelerationVector = true;

        private Timer timer;
        private DateTime lastFrameTime;
        private Label lblTime;
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
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            lblTime.BackColor = Color.Transparent;
            lblTime.Text = "Time: 0.0 s";
            lblTime.BringToFront();
            Controls.Add(lblTime);

            // Add star button
            btnAddStar = new Button
            {
                Text = "Add Star",
                Location = new Point(10, 40),
            };
            btnAddStar.Click += BtnAddStar_Click;
            btnAddStar.BackColor = Color.DarkGray;
            btnAddStar.ForeColor = Color.White;
            Controls.Add(btnAddStar);

            // Initialize Physics Engine with a test scenario
            physicsEngine.PythagoreanThreeBodyTest();

            // Add a button to toggle acceleration vector visibility
            Button btnToggleAccelerationVectors = new Button
            {
                Text = "Toggle Acceleration Vectors",
                Location = new Point(10, 70),
            };
            btnToggleAccelerationVectors.BackColor = Color.DarkGray;
            btnToggleAccelerationVectors.ForeColor = Color.White;
            btnToggleAccelerationVectors.Click += (s, e) =>
            {
                showAccelerationVector = !showAccelerationVector;
                Invalidate();  // Redraw the form to show/hide vectors
            };
            Controls.Add(btnToggleAccelerationVectors);

            // Timer setup
            timer = new Timer();
            timer.Interval = 16; // 60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();

            lastFrameTime = DateTime.Now;
            DoubleBuffered = true;
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
            float timeStep = (float)(3.154e+7 / 32); // Fraction of a year in seconds
            physicsEngine.Update(timeStep);


            // Update sim timer
            simulationTime += deltaTime;
            lblTime.Text = $"Time: {simulationTime:F1} s";

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
