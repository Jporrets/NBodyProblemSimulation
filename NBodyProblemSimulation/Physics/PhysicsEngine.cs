using NBodyProblemSimulation.Classes;
using System.Numerics;

namespace NBodyProblemSimulation.Physics
{
    internal class PhysicsEngine
    {
        public List<CelestialBody> Bodies { get;  } = new();
        public double G = 4 * Math.Pow( Math.PI, 2); // 6.67430e-11

        // Initializers

        public void AddBody(CelestialBody body)
        {
            Bodies.Add(body);
        }

        public void AddSunlikeBody()
        {
            CelestialBody sun = new CelestialBody(
                name: "Sun",
                mass: 1,
                position: new Vector2(0, 0),
                velocity: new Vector2(0, 0),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Yellow
            );
            Bodies.Add(sun);
        }

        public void TwoBodyOrbitTest()
        {
            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(100, 100),
                velocity: new Vector2((float)-5.1E-08, (float)5.7847E-08),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Yellow
            );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(1200, 600),
                velocity: new Vector2((float)5.1E-08, (float)-5.7847E-08),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
            );

            Bodies.Add(Star1);
            Bodies.Add(Star2);
        }

        public void PythagoreanThreeBodyTest()
        {
            /*
             * This method sets up a three-body system in a Pythagorean configuration.
             * This configuration was first described by Meissel in 1893.
             * It consists of three bodies placed at the vertices of a right triangle with precise variables:
             * Three masses in the ratio 3:4:5 are placed at rest at the vertices of a 3:4:5 right triangle.
             * With the heaviest body at the right angle and the lightest at the smaller acute angle.
             */

            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 3,
                position: new Vector2(300, 800),
                velocity: new Vector2(0, 0),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 5,
                position: new Vector2(1100, 800),
                velocity: new Vector2(0, 0),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LightSteelBlue
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 4,
                position: new Vector2(300, 200),
                velocity: new Vector2(0, 0),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.YellowGreen
                );

            Bodies.Add(Star1);
            Bodies.Add(Star2);
            Bodies.Add(Star3);
        }

        public void DragonFlyThreeBodyTest()
        {
            /*
            * This method sets up a three-body system in a DragonFly configuration.
            * This configuration was discovered by Li and Liao in 2020.
            * It consists of three bodies following a periodic orbit that resembles the shape of a dragonfly.
            */

            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(750,500),
                velocity: new Vector2((float)0.000000080584, (float) 0.000000588836),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(1050, 500),
                velocity: new Vector2((float)0.000000080584, (float)0.000000588836),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(900, 500),
                velocity: new Vector2((float) -0.000000161168, (float) -0.000001177672),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
                );

            Bodies.Add(Star1);
            Bodies.Add(Star2);
            Bodies.Add(Star3);
        } // Doesnt work

        public void EightShapeTest()
        {
            /*
             * This method sets up a three-body system in a figure-eight configuration.
             * This configuration was discovered by Moore in 1993.
             * It consists of three bodies of equal mass following a periodic orbit that traces out a figure-eight shape.
             */
            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(700f - 97.000436f, 500f + 24.308753f),
                velocity: new Vector2((float)(0.466203685e-6), (float)(0.432365730e-6)),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(700f + 97.000436f, 500f - 24.308753f),
                velocity: new Vector2((float)(0.466203685e-6), (float)(0.432365730e-6)),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(700f, 500f),
                velocity: new Vector2((float)(-0.932407370e-6), (float)(-0.864731460e-6)),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
                );
            Bodies.Add(Star1);
            Bodies.Add(Star2);
            Bodies.Add(Star3);
        }

        // Methods

        public void ComputeAcceleration(CelestialBody body, double gravitationalConstant = 6.67430e-11, double eps = 1e-3)
        {
            /*
             * This method computes the acceleration of a celestial body due to the gravitational forces exerted by all other bodies in the simulation.
             * It uses Newton's law of universal gravitation to calculate the gravitational force between two bodies,
             * and then derives the acceleration from that force.
             */

            // Reset acceleration
            body.Acceleration = Vector2.Zero;

            // Compute new acceleration
            foreach (CelestialBody otherBody in Bodies)
            {
                if (body == otherBody) continue;

                // Calculate gravitational force based on Newton's law of universal gravitation
                Vector2 displacementVector = (body.Position - otherBody.Position) * -1; // WHY IS THIS NEEDED? WHY DO WE NEED  * -1??????????????????????????????????????????????????????????????????????????????????????????????????
                System.Diagnostics.Debug.WriteLine($"Displacement vector between {body.Name} and {otherBody.Name}: {displacementVector}"); // DEBUG
                double distance = Math.Sqrt((body.Position - otherBody.Position).LengthSquared() + eps * eps); // The distance is softned to avoid short distance infinite forces
                double invertedDistanceCube = 1.0 / Math.Pow(distance, 3); // 1/r^3

                float factor = (float)(gravitationalConstant * otherBody.Mass * invertedDistanceCube);
                Vector2 acceleration = displacementVector * factor;
                System.Diagnostics.Debug.WriteLine($"Data for the CelestialBody {body.Name} is as follows: " +
                    $"Mass: {body.Mass} || OtherBody Mass: {otherBody.Mass} || Distance from {otherBody.Name}: {distance} ||" +
                    $"Acceleration computed as: {acceleration}"); // DEBUG

                body.Acceleration += acceleration;

            }   
        }

        public void ComputePositionBasedOnAccelerationEulersMethod(CelestialBody body, float timeStep)
        {
            /*
             * This method uses Euler's method to update the position and velocity of a celestial body.
             * Euler's method is a simple numerical procedure for solving ordinary differential equations (ODEs) with a given initial value.
             * In this context, it updates the position and velocity of a celestial body based on its acceleration and the time step.
             */
            if (body.Trail.Count > 500)
            {
                body.Trail.RemoveAt(0);
            }

            //
            ComputeAcceleration(body);
            body.Velocity = body.Velocity + body.Acceleration * timeStep;
            body.Trail.Add(body.Position);
            body.Position += body.Velocity * timeStep;


        }

        public void ComputePositionBasedOnVerletIntegration(CelestialBody body, float timeStep)
        {
            /*
             * This method uses the Velocity Verlet integration algorithm to update the position and velocity of a celestial body.
             * The Verlet method updates the position of an object based on its previous position,
             * its current acceleration (which is related to the force acting on it), and the time step.
             */
            if (body.Trail.Count > 500)
            {
                body.Trail.RemoveAt(0);
            }

            //
            ComputeAcceleration(body);
            Vector2 initialPosition = body.Position;
            body.Position += body.Velocity * timeStep + 0.5f * body.Acceleration * timeStep * timeStep;
            Vector2 newAcceleration = body.Acceleration; // Store the current acceleration
            ComputeAcceleration(body); // Recompute acceleration at the new position
            body.Velocity += 0.5f * (newAcceleration + body.Acceleration) * timeStep; // Update velocity using average acceleration
            body.Trail.Add(initialPosition);

            // DEBUG
            System.Diagnostics.Debug.WriteLine($"Body: {body.Name} || New Position: {body.Position} || New Velocity: {body.Velocity} || Acceleration: {body.Acceleration}");
        }

        // Temp Func
        public void Update(float timeStep)
        {
            System.Diagnostics.Debug.WriteLine(timeStep); // DEBUG
            foreach (var body in Bodies)
            {
                ComputePositionBasedOnVerletIntegration(body, timeStep);
            }
        }
    }
}
