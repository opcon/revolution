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
		CharacterControllerInput playerController;

		public Player(Camera c, Space s)
		{
			playerController = new CharacterControllerInput(s, c);

			playerController.Activate();

			playerController.CharacterController.Down = new Vector3(0, -1, 0);
			playerController.CharacterController.HorizontalMotionConstraint.Speed = 13.0f;
		}

		public void Update(double time)
		{

		}
	}
}
