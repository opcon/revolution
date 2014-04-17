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
		Vector3 newUp = Vector3.Zero;
		int maxRotStep = 30;
		int step = 0;
		bool rotate = false;
		float rotateAngle = 0;
		Vector3 initialView = Vector3.Zero;
		Vector3 initialUp = Vector3.Zero;
		Vector3 initialDown = Vector3.Zero;
		public Vector3 CurrentUp = Vector3.UnitY;
		public Vector3 lastRayHitNormal = -Vector3.UnitY;

		public Player(Camera c, Space s)
		{
			playerController = new CharacterControllerInput(s, c);

			playerController.Activate();

			//TODO Sort out friction on ground surface - far too low at the moment and adjusting LinearDamping is a rough hack
			playerController.CharacterController.Down = new Vector3(0, -1, 0);
			//playerController.CharacterController.Body.LinearDamping = 0.7f;
			playerController.CharacterController.HorizontalMotionConstraint.Speed = 7.0f;
			playerController.CharacterController.JumpSpeed = 9;
			playerController.CharacterController.HorizontalMotionConstraint.AirSpeed = 1.0f;
			playerController.CharacterController.Body.Mass = 80;
			playerController.CharacterController.StepManager.MaximumStepHeight = 0.5f;
			playerController.CharacterController.HorizontalMotionConstraint.MaximumForce = 9999;
		}

		public void Update(double time)
		{
			if (rotate) {
				if (step <= maxRotStep) {
					float a = ((float)step / maxRotStep) * rotateAngle;
					var rot = BEPUutilities.Matrix3x3.CreateFromAxisAngle(new Vector3(initialView.X, 0, initialView.Z), a);
					//Console.WriteLine (a);
					var tempUp = BEPUutilities.Matrix3x3.Transform(initialUp, rot);
					var tempDown = BEPUutilities.Matrix3x3.Transform(initialDown, rot);
					var tempView = BEPUutilities.Matrix3x3.Transform(initialView, rot);
					playerController.Camera.LockedUp = tempUp;
					playerController.CharacterController.Down = tempDown;
					//Console.WriteLine (tempDown);
					playerController.CharacterController.ViewDirection = tempView;
					step++;
				}
				else {
					rotate = false;
					playerController.CharacterController.Down = -newUp;
					playerController.Camera.LockedUp = newUp;
				}
			}
			playerController.Update((float)time, prevKeyboardState, OpenTK.Input.Keyboard.GetState());
			CurrentUp = playerController.Camera.LockedUp;

			prevKeyboardState = OpenTK.Input.Keyboard.GetState();

			BEPUutilities.RayHit result;

			var r = new BEPUutilities.Ray(playerController.Camera.Position + playerController.Camera.ViewDirection, playerController.CharacterController.ViewDirection);

			if (playerController.CharacterController.QueryManager.RayCastAgainstSpace(r, 900f, out result))
			{
				Console.WriteLine("Ray Hit!");
				lastRayHitNormal = result.Normal;
			}
			//if (result.HitData.Location != null) Console.WriteLine(result.HitData.T);
		}

		public void BeginRotation(Vector3 dir)
		{
			var ang = Vector3.CalculateAngle(playerController.Camera.LockedUp, dir);
			Console.WriteLine(ang);
			playerController.CharacterController.Body.GravityRotation = Matrix3.Mult(BEPUutilities.Matrix3x3.CreateFromAxisAngle(
				Vector3.UnitX, ang), playerController.CharacterController.Body.GravityRotation);
			var t = ((Matrix3)playerController.CharacterController.Body.GravityRotation);
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					if (Math.Abs(t[i, j]) < 0.000001)
						t[i, j] = 0;
				}

			}
			playerController.CharacterController.Body.GravityRotation = t;
			Console.WriteLine(playerController.CharacterController.Body.GravityRotation);
			newUp = dir;
			rotate = true;
			step = 0;
			rotateAngle = ang;
			initialView = playerController.Camera.ViewDirection;
			initialUp = playerController.Camera.LockedUp;
			initialDown = playerController.CharacterController.Down;
		}
	}
}
