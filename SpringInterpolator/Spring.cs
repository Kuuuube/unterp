using System;
using System.Numerics;

// Code written by Kyle "KuNet" Nguyen

namespace SpringInterpolator
{
    public class Spring2D
    {
        private readonly SpringSettings2D settings;

        // the current state of the spring
        private Vector2 position;
        private Vector2 velocity;

        // controls the "anchor" of the spring (where the cursor will spring down to)
        public Vector2 Target { get; set; }

        // the last time that the update was called used to determine the difference in time
        private DateTime lastTime = DateTime.Now;

        public Spring2D(SpringSettings2D settings)
        {
            this.settings = settings;
        }

        private static bool IsZero(float a)
        {
            return MathF.Abs(a) < 0.0001f;
        }

        public Vector2 Update()
        {
            var currentTime = DateTime.Now;
            var deltaTime = currentTime - lastTime;

            // "performance"
            if (deltaTime.TotalMilliseconds == 0) return position;

            if (
                IsZero(Vector2.DistanceSquared(position, Target)) &&
                IsZero(velocity.X * velocity.X * velocity.Y * velocity.Y))
            {
                position = Target;
                velocity = Vector2.Zero;
                return position;
            }

            // milliseconds might be too small to make the spring effect visible so the divide variable is there
            var delta = (float) deltaTime.TotalMilliseconds / settings.StepSize;
            // since the spring is updating, we can update the last time the spring was updated
            lastTime = currentTime;

            // apply the stiffness and multiply it with the distance between the target and position to go
            // towards the target and multiply it by the difference in time so it is correct in respect to time
            velocity += 1f / settings.Stiffness * (Target - position) * delta;

            // damp the velocity so that the spring does not oscillate forever (is really funny when is 0)
            // might be a feature to remove this in the feature ???? lol
            velocity *= MathF.Pow(1f / settings.Damping, delta);

            // applies the velocity to the current position and correct for time difference
            position += velocity * delta;

            return position;
        }
    }

    public class SpringSettings2D
    {
        public SpringSettings2D(float stiffness, float damping, float stepSize)
        {
            Stiffness = stiffness;
            Damping = damping;
            StepSize = stepSize;
        }

        private float stiffness;

        public float Stiffness
        {
            get => stiffness;
            set => stiffness = MathF.Max(value, 0.0001f);
        }

        private float damping;

        public float Damping
        {
            get => damping;
            set => damping = MathF.Max(value, 0.0001f);
        }

        private float stepSize;

        public float StepSize
        {
            get => stepSize;
            set => stepSize = MathF.Max(value, 0.0001f);
        }
    }
}