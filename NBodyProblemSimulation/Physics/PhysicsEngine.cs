using NBodyProblemSimulation.Classes;
using NBodyProblemSimulation.Utils;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace NBodyProblemSimulation.Physics
{
    internal class PhysicsEngine
    {
        public List<CelestialBody> Bodies { get;  } = new();
        public int ScenarioIndex = 0;
        public double GravitationalConstant = 4 * Math.Pow( Math.PI, 2); // This is the gravitational constant in AU^3 / (Solar Mass * Year^2)
        public int IntegrationMethodIndex = 1; // Default to Verlet Integration
        public string CurrentIntegrationMethodName = "Verlet Integration";
        Scenarios scenarios = new Scenarios();

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
                position: new Vector2(500, 500),
                velocity: new Vector2(0, 0),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Yellow
            );
            Bodies.Add(sun);
        }

        public void ClearBodies()
        {
            Bodies.Clear();
        }

        public void SwitchScenario()
        {
            // Set variables
            ScenarioIndex++;
            ClearBodies();

            // Load scenario

            if (scenarios.ScenarioDict.ContainsKey(ScenarioIndex))
            {
                scenarios.ScenarioDict[ScenarioIndex]().ForEach(body => AddBody(body));
            }
            else
            {
                ScenarioIndex = 0;
                scenarios.ScenarioDict[ScenarioIndex]().ForEach(body => AddBody(body));
            }
        }

        public void LoadFirstScenario()
        {
            Scenarios scenarios = new Scenarios();
            scenarios.ScenarioDict[ScenarioIndex]().ForEach(body => AddBody(body));

            // DragonFlyThreeBodyTest();
        }

        public void ResetSimulation()
        {
            ClearBodies();
            
            if (scenarios.ScenarioDict.ContainsKey(ScenarioIndex))
            {
                scenarios.ScenarioDict[ScenarioIndex]().ForEach(body => AddBody(body));
            }
            else
            {
                ScenarioIndex = 0;
                scenarios.ScenarioDict[ScenarioIndex]().ForEach(body => AddBody(body));
            }

        }

        // Test Setups

        public void DragonFlyThreeBodyTest()
        {
            /*
            * This method sets up a three-body system in a DragonFly configuration.
            * This configuration was discovered by Li and Liao in 2012.
            * It consists of three bodies following a periodic orbit that resembles the shape of a dragonfly.
            */

            float scalingFactor = 2 * (float)Math.PI * 1e-1f;
            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(900f - 100f,500f),
                velocity: new Vector2(0.080584f * scalingFactor, 0.588836f *scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(900f + 100f, 500f),
                velocity: new Vector2(0.080584f * scalingFactor, 0.588836f * scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(900f, 500f),
                velocity: new Vector2(0.161168f * scalingFactor, -1.177672f * scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
                );

            Bodies.Add(Star1);
            Bodies.Add(Star2);
            Bodies.Add(Star3);
        } // Maybe works but needs more precise integration
        public void ButterflyTest()
        {

            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(900f, 500f),
                velocity: new Vector2(0, 2.181e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(900f + 100f, 500f),
                velocity: new Vector2(-1.091e-1f, -1.091e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(900f - 100f, 500f),
                velocity: new Vector2(-1.091e-1f, -1.091e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
                );

            Bodies.Add(Star1);
            Bodies.Add(Star2);
            Bodies.Add(Star3);
        } // Doesn't work

        // Physics Methods

        public void ComputeAcceleration(CelestialBody body, double eps = 1e-3)
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
                Vector2 displacementVector = (body.Position - otherBody.Position) * -1; // We apply a negative sign to get the vector pointing from body to otherBody
                double distance = Math.Sqrt((body.Position - otherBody.Position).LengthSquared() + eps * eps); // The distance is softened to avoid short distance infinite forces
                double invertedDistanceCube = 1.0 / Math.Pow(distance, 3); // 1/r^3

                float factor = (float)(GravitationalConstant * otherBody.Mass * invertedDistanceCube);
                Vector2 acceleration = displacementVector * factor;
                body.Acceleration += acceleration;
            }   
        }

        public void ComputePositionBasedOnAccelerationEulersMethod(float timeStep, bool shouldRecordTrail)
        {
            /*
             * This method uses Euler's method to update the position and velocity of a celestial body.
             * Euler's method is a simple numerical procedure for solving ordinary differential equations (ODEs) with a given initial value.
             * In this context, it updates the position and velocity of a celestial body based on its acceleration and the time step.
             */

            foreach(CelestialBody body in Bodies)
            {
                if (body.Trail.Count > body.TrailLength)
                {
                    body.Trail.RemoveAt(0);
                }

                //
                ComputeAcceleration(body);
                body.Velocity = body.Velocity + body.Acceleration * timeStep;
                
                if ( shouldRecordTrail)
                    body.Trail.Add(body.Position);

                body.Position += body.Velocity * timeStep;
            }
        }

        public void ComputePositionBasedOnVerletIntegration(float timeStep, bool shouldRecordTrail)
        {
            /*
             * This method uses the Velocity Verlet integration algorithm to update the position and velocity of celestial bodies.
             * The Verlet method updates the position of an object based on its previous position,
             * its current acceleration (which is related to the force acting on it), and the time step.
             */

            // Update acceleration for every body first

            foreach(CelestialBody body in Bodies)
            {
                ComputeAcceleration(body);
                body.OldAcceleration = body.Acceleration;
            }

            foreach (CelestialBody body in Bodies)
            {
                if (body.Trail.Count > body.TrailLength)
                {
                    body.Trail.RemoveAt(0);
                }

                //
                Vector2 initialPosition = body.Position;

                if (shouldRecordTrail)
                    body.Trail.Add(initialPosition);

                body.Position += body.Velocity * timeStep + 0.5f * body.Acceleration * timeStep * timeStep;
            }

            foreach (CelestialBody body in Bodies) // Computes new acceleration at the new position for all bodies
            {
                ComputeAcceleration(body);
            }

            foreach (CelestialBody body in Bodies) // Finally updates velocity for all bodies
            {

                body.Velocity += 0.5f * (body.OldAcceleration + body.Acceleration) * timeStep; // Update velocity using average acceleration

            }
        }

        public void ComputePositionBasedOnYoshidaFourthOrder(float timeStep, bool shouldRecordTrail)
        {
            /*
             * This method uses the Fourth-Order Yoshida integration algorithm to update the position and velocity of celestial bodies.
             * The Yoshida method is a symplectic integrator that provides higher accuracy for simulating Hamiltonian systems, such as gravitational interactions.
             * It involves multiple sub-steps to update positions and velocities, ensuring better conservation of energy and momentum over long simulations.
             * Provides better performance compared to Velocity Verlet integration.
             */
            float cbrt2 = MathF.Pow(2f, 1f / 3f);
            float a1 = 1f / (2f * (2f - cbrt2));
            float a2 = (1f - cbrt2) / (2f * (2f - cbrt2));

            ComputePositionBasedOnVerletIntegration(a1 * timeStep, false); // Defaults to not recording trail in this instance, as it would clutter the trail
            ComputePositionBasedOnVerletIntegration(a2 * timeStep, false); // Defaults to not recording trail in this instance, as it would clutter the trail
            ComputePositionBasedOnVerletIntegration(a1 * timeStep, shouldRecordTrail);
        }

        public void ComputePositionBasedOnRungeKuttaFourthOrder(float timeStep, bool shouldRecordTrail)
        {
            /*
             * This method uses the Fourth-Order Runge-Kutta integration algorithm to update the position and velocity of celestial bodies.
             * The Runge-Kutta method is a widely used numerical technique for solving ordinary differential equations (ODEs).
             * It provides a good balance between accuracy and computational efficiency, making it suitable for simulating complex systems like gravitational interactions.
             */
            
            
        }

        // Update Function

        public void SwitchIntegrationMethod()
        {
            IntegrationMethodIndex++;
            if (IntegrationMethodIndex > 2)
            {
                IntegrationMethodIndex = 0;
            }

            // Reset simulation to avoid instability when switching methods
            ResetSimulation();

            // Update current scenario name
            switch (IntegrationMethodIndex)
            {
                case 0:
                    CurrentIntegrationMethodName = "Euler's Method";
                    break;
                case 1:
                    CurrentIntegrationMethodName = "Verlet Integration";
                    break;
                case 2:
                    CurrentIntegrationMethodName = "Yoshida Fourth Order";
                    break;
                default:
                    CurrentIntegrationMethodName = "Verlet Integration";
                    break;
            }

        }

        public void Update(float timeStep, bool shouldRecordTrail)
        {
            switch (IntegrationMethodIndex)
            {
                case 0:
                    ComputePositionBasedOnAccelerationEulersMethod(timeStep, shouldRecordTrail);
                    break;
                case 1:
                    ComputePositionBasedOnVerletIntegration(timeStep, shouldRecordTrail);
                    break;
                case 2:
                    ComputePositionBasedOnYoshidaFourthOrder(timeStep, shouldRecordTrail);
                    break;
                default:
                    ComputePositionBasedOnVerletIntegration(timeStep, shouldRecordTrail);
                    break;
            }
        }
    }
}
