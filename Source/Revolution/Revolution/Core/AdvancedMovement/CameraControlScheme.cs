using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revolution.Core.AdvancedMovement
{
    /// <summary>
    /// Superclass of implementations which control the behavior of a camera.
    /// </summary>
    public abstract class CameraControlScheme
    {

        /// <summary>
        /// Gets the camera controlled by this control scheme.
        /// </summary>
        public Camera Camera { get; private set; }

        protected CameraControlScheme(Camera camera)
        {
            Camera = camera;
        }

        /// <summary>
        /// Updates the camera state according to the control scheme.
        /// </summary>
        /// <param name="dt">Time elapsed since previous frame.</param>
        public virtual void Update(float dt)
        {
#if XBOX360
            Yaw += Game.GamePadInput.ThumbSticks.Right.X * -1.5f * dt;
            Pitch += Game.GamePadInput.ThumbSticks.Right.Y * 1.5f * dt;
#else
            //Only turn if the mouse is controlled by the game.
            if (true)
            {
                if (InputSystem.MouseDelta != Vector2.Zero)
                    System.Console.WriteLine(InputSystem.MouseDelta.ToString());
                //Camera.Pitch(0.001f);
                Camera.Yaw(-(InputSystem.MouseDelta.X) * dt * 0.52f);
                Camera.Pitch((InputSystem.MouseDelta.Y) * dt * 0.52f);
            }
#endif
        }
    }
}
