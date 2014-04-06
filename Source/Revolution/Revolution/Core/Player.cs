using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Revolution.Core.Loaders.Microbrush;
using BEPUphysics;
using Revolution.Core.AdvancedMovement;

namespace Revolution.Core
{
	class Player
	{
		public CharacterControllerInput playerController;

        OpenTK.Input.KeyboardState prevKeyboardState;

		public Player(Camera c, Space s)
		{
			playerController = new CharacterControllerInput(s, c);

			playerController.Activate();

			playerController.CharacterController.Down = new Vector3(0, -1, 0);
			playerController.CharacterController.HorizontalMotionConstraint.Speed = 13.0f;
            playerController.CharacterController.JumpSpeed = 5;
            playerController.CharacterController.HorizontalMotionConstraint.AirSpeed = 0.3f;
            playerController.CharacterController.Body.Mass = 80;
		}

		public void Update(double time)
		{
            playerController.Update((float)time, prevKeyboardState, OpenTK.Input.Keyboard.GetState());

            prevKeyboardState = OpenTK.Input.Keyboard.GetState();
		}
	}
}
