using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Revolution.Core.Loaders.Microbrush;

namespace Revolution.Core
{
    class Player
    {
        public Vector3 Position = Vector3.Zero;

        public Quaternion Orientation { get; set; }

        public Vector3 Velocity { get; set; }

        public QuaternionCamera Camera { get; set; }

        public Vector3 CameraPos { get; set; }

        public Vector3 CameraOffset { get; set; }

        public Vector3 CameraPosition { get; set; }

        public CollisionPacket colPacket;

        private int recursion = 0;

        public Player(QuaternionCamera cam, Vector3 initialPos)
        {
            Camera = cam;
            Position = initialPos;
            Camera.Position = Camera.TargetPosition = Position;
            CameraPos = new Vector3(0, 2, 10);
        }

        public void Update(double time, MicrobrushScene scene)
        {
            recursion = 0;

            CameraOffset = new Vector3(0, 2, 10);

            Camera.UpdateRotations(time);
            Orientation = Camera.Orientation;
            Camera.UpdateMovement(time);
            Velocity = Camera.Velocity;

            Position.Y += 1;

            colPacket = new CollisionPacket();
            colPacket.ERadius = new Vector3(1, 1, 1);
            colPacket.R3Position = Position;
            //colPacket.R3Position.Y += 1;
            colPacket.R3Velocity = Velocity * (float)time;
            //collisionTest.R3Velocity = new Vector3(5, 5, 5);


            //Check for collisions and alter the velocity if needed
            Position = CollideWithWorld(time, scene);

            Position = Vector3.Multiply(Position, colPacket.ERadius);
            Position.Y -= 1;


            //Apply the velocity
            //Position += Velocity*(float) time;

            CalculateCameraPosition();

            //Camera.Position = Camera.TargetPosition = Position + new Vector3(0, -15, 0);
        }

        private Vector3 CollideWithWorld(double time, MicrobrushScene scene)
        {

            float unitsPerMeter = 1000.0f; //Set this to match application scale.
            // All hard-coded distances in this function is
            // scaled to fit the setting above..
            float unitScale = unitsPerMeter / 100.0f;
            float veryCloseDistance = 0.005f * unitScale;


            colPacket.Velocity = Collisions.R3ToESpace(colPacket.R3Velocity, ref colPacket);
            colPacket.BasePoint = Collisions.R3ToESpace(colPacket.R3Position, ref colPacket);
            colPacket.NormalisedVelocity = Vector3.Normalize(colPacket.Velocity);

            scene.CheckCollisions(ref colPacket);

            if (!colPacket.FoundCollision)
            {
                return Position + Velocity*(float)time;
            }

            Console.WriteLine("COLLISION!!, Collision Position:" + Vector3.Multiply(colPacket.IntersectionPoint, colPacket.ERadius) + " | Camera position:" + colPacket.R3Position);

            Console.WriteLine("Player velocity is " + -(Velocity + Position));

            Vector3 DestPoint = Position + Velocity*(float) time;
            Vector3 NewBasePoint = Position;

            // only update if we are not already very close
            // and if so we only move very close to intersection..not
            // to the exact spot.
            if (colPacket.NearestDistance >= veryCloseDistance)
            {
                Vector3 v = Velocity * (float) time;
                v.Normalize();
                v = v * (float)(colPacket.NearestDistance - veryCloseDistance);
                NewBasePoint = colPacket.BasePoint + v;

                // Adjust polygon intersection point (so sliding
                // plane will be unaffected by the fact that we
                // move slightly less than collision tells us)
                v.Normalize();
                colPacket.IntersectionPoint -= veryCloseDistance*v;
            }

            //Determine the sliding plane
            Vector3 SlidPlaneOrigin = colPacket.IntersectionPoint;
            Vector3 SlidePlaneNormal = NewBasePoint - colPacket.IntersectionPoint;
            SlidePlaneNormal.Normalize();
            Plane SlidingPlane = new Plane(SlidPlaneOrigin, SlidePlaneNormal);

            Vector3 NewDestinationPoint = DestPoint - (float)SlidingPlane.SignedDistanceTo(DestPoint)*SlidePlaneNormal;

            Vector3 NewVelocityVector = NewDestinationPoint - colPacket.IntersectionPoint;

            if (NewVelocityVector.Length < veryCloseDistance || recursion > 5)
            {
                return NewBasePoint;
            }
            recursion++;

            colPacket.BasePoint = NewBasePoint;
            colPacket.Velocity = NewVelocityVector;
            colPacket.FoundCollision = false;

            return CollideWithWorld(time, scene);
        }

        private void CalculateCameraPosition()
        {
            CameraPos = Vector3.Transform(CameraOffset, Quaternion.Invert(Orientation));
            CameraPos += Position;

            Camera.Position = Camera.TargetPosition = CameraPos;

        }

        public void Draw(double time)
        {
            
        }
    }
}
