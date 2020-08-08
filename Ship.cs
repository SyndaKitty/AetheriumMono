using System;
using AetheriumMono.Core;
using Microsoft.Xna.Framework;

namespace AetheriumMono
{
    public class Ship : PhysicsObject
    {
        float forwardThrust = 6;
        float strafeThrust = 2;
        float rotationThrust = 2;

        float cancelVelocityBonus = 1; // Percent gained (1 = +100%)
        float cancelAngularVelocityBonus = 1; // Percent gained (1 = +100%)

        public void Control(float forwardAmount, float strafeAmount, float rotationAmount)
        {
            forwardAmount = Mathf.Clamp(forwardAmount, -1, 1);
            rotationAmount = Mathf.Clamp(rotationAmount, -1, 1);

            var forwardVector = Forward * forwardAmount * forwardThrust;
            var strafeVector = Right * strafeAmount * strafeThrust;
            var targetVector = forwardVector + strafeVector;

            // Cancel velocity bonus
            Vector2 velNorm = Vector2.Normalize(Body.LinearVelocity);
            Vector2 forwardNorm = Vector2.Normalize(forwardVector);
            Vector2 strafeNorm = Vector2.Normalize(strafeVector);

            var forwardDot = Vector2.Dot(velNorm, forwardNorm);
            var strafeDot = Vector2.Dot(velNorm, strafeNorm);

            if (forwardDot < 0)
            {
                forwardVector *= (1 + -forwardDot * cancelVelocityBonus);
            }
            if (strafeDot < 0)
            {
                strafeVector *= (1 + -strafeDot * cancelAngularVelocityBonus);
            }

            Body.ApplyForce(forwardVector + strafeVector,Body.WorldCenter);

            // Cancel angular velocity bonus
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Mathf.Sign(Body.AngularVelocity) != Mathf.Sign(rotationAmount))
            {
                rotationAmount *= (1 + cancelAngularVelocityBonus);
            }
            Body.ApplyTorque(rotationAmount * rotationThrust);
        }
    }
}
