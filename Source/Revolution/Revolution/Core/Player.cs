using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
        Vector3 initialHorizontal = Vector3.Zero;
        Vector3 rotAxis = Vector3.Zero;
        Quaternion rotationQuaternion = Quaternion.Identity;
		public Vector3 CurrentUp = Vector3.UnitY;
		public Vector3 lastRayHitNormal = -Vector3.UnitY;
        public Vector3 lastRayHitPosition = Vector3.Zero;

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
                    //float a = ((float)step / maxRotStep) * rotateAngle;
                    float a = ((float)step / maxRotStep);
                    //var rot = BEPUutilities.Matrix3x3.CreateFromAxisAngle(rotAxis, a);
                    var q = Quaternion.Slerp(Quaternion.Identity, rotationQuaternion, a);
                    var rot = Matrix3.CreateFromQuaternion(q);
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
                //Console.WriteLine("Ray Hit!");
                Console.WriteLine(result.T);
				lastRayHitNormal = result.Normal;
                lastRayHitPosition = result.Location;
			}
			//if (result.HitData.Location != null) Console.WriteLine(result.HitData.T);
		}

        public void Draw(double time)
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(OpenTK.Graphics.Color4.White);
            GL.Vertex3(lastRayHitPosition);
            GL.Vertex3(lastRayHitPosition + 2*lastRayHitNormal.Normalized());
            GL.End();

        }

		public void BeginRotation(Vector3 dir)
		{
            dir.Normalize();
            BEPUutilities.Quaternion rotQuat;
            BEPUutilities.Vector3 unitUp = Vector3.Normalize(playerController.Camera.LockedUp);
            BEPUutilities.Vector3 unitDir = dir;
            BEPUutilities.Quaternion.GetQuaternionBetweenNormalizedVectors(ref unitUp, ref unitDir, out rotQuat);
            rotQuat.Normalize();

			var ang = Vector3.CalculateAngle(playerController.Camera.LockedUp, dir);
			Console.WriteLine(ang);
            rotAxis = Vector3.Cross(playerController.Camera.ViewDirection, -dir).Normalized();
            var gAng = Vector3.CalculateAngle(playerController.Space.ForceUpdater.Gravity, -dir);
            BEPUutilities.Quaternion gravQuat;
            BEPUutilities.Vector3 unitGrav = Vector3.Normalize(playerController.Space.ForceUpdater.Gravity);
            BEPUutilities.Vector3 negUnitDir = -unitDir;
            BEPUutilities.Quaternion.GetQuaternionBetweenNormalizedVectors(ref unitGrav, ref negUnitDir, out gravQuat);
            gravQuat.Normalize();
            playerController.CharacterController.Body.GravityRotation = Matrix3.CreateFromQuaternion(gravQuat);
            //playerController.CharacterController.Body.GravityRotation = Matrix3.CreateFromAxisAngle(-rotAxis, gAng);
            //playerController.CharacterController.Body.GravityRotation = Matrix3.Mult(BEPUutilities.Matrix3x3.CreateFromAxisAngle(
            //    playerController.CharacterController.HorizontalViewDirection, ang), playerController.CharacterController.Body.GravityRotation);
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
            initialHorizontal = playerController.CharacterController.HorizontalViewDirection;
            rotationQuaternion = rotQuat;
		}
	}
}
