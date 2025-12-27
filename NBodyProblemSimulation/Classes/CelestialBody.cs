using System.Numerics;

namespace NBodyProblemSimulation.Classes
{
    internal class CelestialBody
    {
        // Properties
        public string Name { get; set; }
        public double Mass { get; set; } // Real astronomical mass in solar mass, the mass of the sun is 1.989e30 kg || Therefore, 1 solar mass = 1.989e30 kg
        public Vector2 Position { get; set; } // Uses astronomical units ( 1 AU = 150 million km)
        public Vector2 Velocity { get; set; } // AU / Timestep (Timestep is in years)
        public Vector2 Acceleration { get; set; }
        public Vector2 OldAcceleration { get; set; } // Needed for Verlet Integration
        public float Radius { get; set; }
        public List<Vector2> Trail { get; set; }
        public int TrailLength { get; set; } // Default trail length
        public Color ColorHex { get; set; }

        // Constructor
        public CelestialBody(string name, double mass, Vector2 position, Vector2 velocity, Vector2 acceleration, float radius, Color colorHex)
        {
            Name = name;
            Mass = mass;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            OldAcceleration = acceleration;
            Radius = radius;
            Trail = new List<Vector2>();
            TrailLength = 1000;
            ColorHex = colorHex;
        }
    }
}
