# NBodyProblemSimulation

A simulation of the classical **n-body problem** implemented in **C# with WinForms**.

This project models the gravitational interaction between multiple bodies under Newtonian physics and visualizes their motion in a simple GUI.

## 📌 Features
- Simulates gravitational interactions between multiple bodies
- Windows desktop application (WinForms) with graphical output
- Provides a set of scenarios to visualise.

## 🧩 Technology
- **Language:** C#
- **Framework:** .NET (WinForms)
- **Platform:** Windows desktop

## 🔨 Physics
The acceleration of celestial bodies is computed using Newton's laws of gravitation; a softening factor is used to avoid division by zero errors and extremely high forces in close encounters.
To integrate the movement of the bodies given the acceleration there are a few methods that are commonly used. In this case: Euler's, velocity Verlet and Yoshida's integrators are currenlty available.
They are listed from least to most accurate. However, with the low and consistent time steps used in the application every method can predict a few times the orbit of any scenario. 
Eventually, every orbit (which should be periodic) drift away: that could be due to a few factors: softening, time step size, errors during integrations, approximations, and other (to me) unknown factors.

## 📸 Screenshot
<img width="295" height="238" alt="Screenshot 2025-12-27 130035" src="https://github.com/user-attachments/assets/78679365-cfdf-4278-936a-d5a67eb97038" />
