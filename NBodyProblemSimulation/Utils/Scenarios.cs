using NBodyProblemSimulation.Classes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NBodyProblemSimulation.Utils
{

    internal class Scenarios
    {
        public Dictionary<int, Func<List<CelestialBody>>> ScenarioDict;

        public Scenarios()
        {
            // Initialize the dictionary with scenario functions and corresponding keys (indices)
            ScenarioDict = new Dictionary<int, Func<List<CelestialBody>>>
        {
            { 0, TwoBodies },
            { 1, PythagoreanThreeBodies },
            { 2, EquilateralTriangle },
            { 3, EightShape },
            { 4, BHHConfiguration }
        };
        }

        /// <summary>
        /// Creates a scenario consisting of two celestial bodies representing a two-body system.
        /// </summary>
        /// <remarks>This scenario can be used to simulate gravitational interactions between two bodies
        /// of equal mass. The returned list contains the initial states of both bodies, which can be used as input for
        /// further simulation or visualization.</remarks>
        /// <returns>A list of two <see cref="CelestialBody"/> objects, each representing a star with predefined properties for
        /// mass, position, velocity, and color.</returns>
        public List<CelestialBody> TwoBodies ()
        {
            List < CelestialBody> returnList = new List<CelestialBody>();

            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(100, 100),
                velocity: new Vector2((float)-5.1E-02, (float)5.7847E-02),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Yellow
            );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(1200, 600),
                velocity: new Vector2((float)5.1E-02, (float)-5.7847E-02),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
            );

            returnList.Add(Star1);
            returnList.Add(Star2);
            return returnList;
        }

        /// <summary>
        /// Creates a list of three celestial bodies arranged in a Pythagorean three-body scenario with predefined
        /// masses and positions.
        /// </summary>
        /// <remarks>
        /// This configuration was first described by Meissel in 1893.
        /// It consists of three bodies placed at the vertices of a right triangle with precise variables:
        /// three masses in the ratio 3:4:5 are placed at rest at the vertices of a 3:4:5 right triangle.
        /// </remarks>
        /// <returns>A list of three <see cref="CelestialBody"/> instances representing the initial state of the Pythagorean
        /// three-body problem.</returns>
        public List<CelestialBody> PythagoreanThreeBodies()
        {
            List<CelestialBody> returnList = new List<CelestialBody>();

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
            returnList.Add(Star1);
            returnList.Add(Star2);
            returnList.Add(Star3);

            return returnList;
        }

        /// <summary>
        /// Creates a collection of three celestial bodies positioned at the vertices of an equilateral triangle, each
        /// with initial velocities suitable for simulating a stable orbital configuration.
        /// </summary>
        /// <remarks>
        /// This configuration was first described by Lagrange.
        /// </remarks>
        /// <returns>A list of three <see cref="CelestialBody"/> instances representing the vertices of an equilateral triangle.
        /// Each body is initialized with position, velocity, and other properties appropriate for orbital simulation.</returns>
        public List<CelestialBody> EquilateralTriangle()
        {
            List<CelestialBody> returnList = new List<CelestialBody>();

            float distanceToCenterOfMass = 200f;
            float angularVelocity = (float)Math.Sqrt(4 * Math.Pow(Math.PI, 2) * 1 /
                (distanceToCenterOfMass * distanceToCenterOfMass * distanceToCenterOfMass * (float)Math.Sqrt(3)));
            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(700f + 200f, 500f + 0), // Top vertex
                velocity: new Vector2(0, distanceToCenterOfMass * angularVelocity / 2),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
            );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(700f - distanceToCenterOfMass / 2, 500f + (float)Math.Sqrt(3) * distanceToCenterOfMass / 2), // Bottom left vertex
                velocity: new Vector2(-(float)Math.Sqrt(3) * distanceToCenterOfMass * angularVelocity / 4, -0.5f * angularVelocity * distanceToCenterOfMass / 2),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
            );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(700f - distanceToCenterOfMass / 2, 500f - (float)Math.Sqrt(3) * distanceToCenterOfMass / 2), // Bottom right vertex
                velocity: new Vector2((float)Math.Sqrt(3) * distanceToCenterOfMass * angularVelocity / 4, -0.5f * angularVelocity * distanceToCenterOfMass / 2),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
            );

            returnList.Add(Star1);
            returnList.Add(Star2);
            returnList.Add(Star3);
            return returnList;
        }

        /// <summary>
        /// Creates and returns a list of three celestial bodies arranged in an initial configuration suitable for
        /// simulating a figure-eight three-body orbit.
        /// </summary>
        /// <remarks>
        /// This configuration was first described by Moore in 1993.
        /// </remarks>
        /// <returns>A list of three <see cref="CelestialBody"/> instances representing the initial state of the system. Each
        /// body is positioned and given a velocity to form a stable figure-eight trajectory.</returns>
        public List<CelestialBody> EightShape()
        {
            List<CelestialBody> returnList = new List<CelestialBody>();
            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1,
                position: new Vector2(700f + 0.97000436e2f, 500f - 0.24308753e2f),
                velocity: new Vector2(2.929e-1f, 2.717e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
            );

            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 1,
                position: new Vector2(700f - 0.97000436e2f, 500f + 0.24308753e2f),
                velocity: new Vector2(2.929e-1f, 2.717e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
            );

            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(700f + 0f, 500f + 0f),
                velocity: new Vector2(-5.858e-1f, -5.434e-1f),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
            );
            returnList.Add(Star1);
            returnList.Add(Star2);
            returnList.Add(Star3);
            return returnList;
        }


        /// <summary>
        /// This configuration was designed to simulate a three-body system known as the BHH configuration. It does not have a specific name.
        /// </summary>
        /// <remarks>
        /// This configuration was first described by Liao, Li and Yang in 2022.
        /// </remarks>
        /// <returns>A list of <see cref="CelestialBody"/> objects, each representing a star with predefined mass, position,
        /// velocity, and color.</returns>
        public List<CelestialBody> BHHConfiguration()
        {
            List<CelestialBody> returnList = new List<CelestialBody>();
            float scalingFactor = 2 * (float)Math.PI * 1e-1f;

            CelestialBody Star1 = new CelestialBody(
                name: "Star 1",
                mass: 1.0283f,
                position: new Vector2(900f - 162.064f, 500f),
                velocity: new Vector2(0, -0.65955f * scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.OrangeRed
                );
            CelestialBody Star2 = new CelestialBody(
                name: "Star 2",
                mass: 0.9879f,
                position: new Vector2(900f + 100f, 500f),
                velocity: new Vector2(-0, -0.14784f * scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.Azure
                );
            CelestialBody Star3 = new CelestialBody(
                name: "Star 3",
                mass: 1,
                position: new Vector2(900f, 500f),
                velocity: new Vector2(-0, 0.8222f * scalingFactor),
                acceleration: new Vector2(0, 0),
                radius: 5,
                colorHex: Color.LimeGreen
                );

            returnList.Add(Star1);
            returnList.Add(Star2);
            returnList.Add(Star3);
            return returnList;
        }

    }
}
