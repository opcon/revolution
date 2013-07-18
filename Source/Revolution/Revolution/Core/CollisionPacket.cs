using System;
using OpenTK;

namespace Revolution.Core
{
    public struct CollisionPacket
    {
        //Ellipsoid radius
        public Vector3 ERadius { get; set; }

        //Information about the move being requested (in R3)
        public Vector3 R3Velocity { get; set; }
        public Vector3 R3Position { get; set; }

        //Information about the move being requested (in eSpace)
        public Vector3 Velocity { get; set; }
        public Vector3 NormalisedVelocity { get; set; }
        public Vector3 BasePoint { get; set; }

        //Hit information
        public bool FoundCollision;
        public double NearestDistance;
        public Vector3 IntersectionPoint { get; set; }

        public float Time { get; set; }
    }

    class Collisions
    {
        public static Vector3 R3ToESpace(Vector3 r3, ref CollisionPacket collisionPacket)
        {
            //return new Vector3(r3.X / collisionPacket.ERadius.X, r3.Y / collisionPacket.ERadius.Y, r3.Z / collisionPacket.ERadius.Z);
            return Vector3.Divide(r3, collisionPacket.ERadius);
        }

        /// <summary>
        /// Assumes p1, p2, and p3 are given in ellipsoid space!
        /// </summary>
        /// <param name="colPacket"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="colFound"></param>
        public static void CheckTriangle(ref CollisionPacket colPacket, Vector3 p1, Vector3 p2, Vector3 p3, ref bool colFound)
        {
            //Make the plane containing this triangle
            var trianglePlane = new Plane(new Vector3[] {p1, p2, p3});
            //var trianglePlane = new Plane(new Vector3[] {p3, p2, p1});

            // Is triangle front-facing to the velocity vector?
            // We only check front-facing traingles
            // (your choice of course)
            if (trianglePlane.IsFrontFacingTo(colPacket.NormalisedVelocity))
            {
                //Get the interval of plane intersection
                double t0, t1;
                bool embeddedInPlane = false;

                //Calculate signed distarnce from sphere position to triangle plane
                double signedDistToTrianglePlane = trianglePlane.SignedDistanceTo(colPacket.BasePoint);

                //cache this as we're going to use it a few times below
                float normalDotVelocity = Vector3.Dot(trianglePlane.Normal, colPacket.Velocity);

                //if sphere is travelling parallel to the plane
                if (normalDotVelocity == 0.0f)
                {
                    if (Math.Abs(signedDistToTrianglePlane) >= 1.0f)
                    {
                        //Sphere is not embedded in plane
                        //No collision possible
                        return;
                    }
                    else
                    {
                        //Sphere is embedded in plane
                        //It intersects the whole range [0..1]
                        embeddedInPlane = true;
                        t0 = 0;
                        t1 = 1;
                    }
                }
                else
                {
                    //N dot D is not 0. Calculate intersection interval
                    t0 = (-1.0 - signedDistToTrianglePlane)/normalDotVelocity;
                    t1 = (1.0 - signedDistToTrianglePlane)/normalDotVelocity;

                    //Swap so t0 < t1
                    if (t0 > t1)
                    {
                        double temp = t1;
                        t1 = t0;
                        t0 = temp;
                    }

                    //Check that at least one result is within range
                    if (t0 > 1.0f || t1 < 0.0f)
                    {
                        //Both t values are outside values [0, 1]
                        //No collision possible
                        return;
                    }

                    //Clamp to [0, 1]
                    if (t0 < 0.0) t0 = 0.0;
                    if (t1 < 0.0) t1 = 0.0;
                    if (t0 > 1.0) t0 = 1.0;
                    if (t1 > 1.0) t1 = 1.0;
                }

                //Ok, at this point we have two time values, t0 and t1 between which the swept sphere intersects with the
                //triangle plane. If any collision is to occur it must happen within this interval.
                Vector3 collisionPoint = new Vector3();
                bool foundCollision = false;

                float t = 1.0f;

                //First we check for the easy case - collision inside the triangle. If this happens it must be at the time t0
                //as this is when the sphere rests on the front side of the triangle plane. Note, this can only happen if
                //the sphere is not embedded in the tirangle plane

                if (!embeddedInPlane)
                {
                    Vector3 planeIntersectionPoint = (colPacket.BasePoint - trianglePlane.Normal) +
                                                     (Vector3.Multiply(colPacket.Velocity, (float)t0));
                    
                    if (Utilities.CheckPointInTriangle(planeIntersectionPoint, p1, p2, p3))
                    {
                        foundCollision = true;
                        t = (float)t0;
                        collisionPoint = planeIntersectionPoint;
                    }
                }

                //If  we haven't found a collision already we'll have to sweep sphere against points and edges of the triangle.
                //Note: A collision inside the tirangle (the check above) will always happen before a vertex or edge collision!
                //This is why we can skip the sweep test if the above gives a collision!
                if (foundCollision == false)
                {
                    //some commonly used terms
                    Vector3 velocity = colPacket.Velocity;
                    Vector3 basePoint = colPacket.BasePoint;
                    float velocitySquaredLength = velocity.LengthSquared;
                    float a, b, c; //params for eqn
                    float newT;

                    //for each vertex or edge a quadratic eqn will have to be solved. We parameterize this eqn as
                    //a*t^2 + b*t + c = 0 and below we calculate the parameters a, b and c for each test

                    //Check against points
                    a = velocitySquaredLength;

                    //P1
                    b = 2*(Vector3.Dot(velocity, (basePoint - p1)));
                    c = (p1 - basePoint).LengthSquared - 1.0f;
                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        t = newT;
                        foundCollision = true;
                        collisionPoint = p1;
                    }

                    //P2
                    b = 2*(Vector3.Dot(velocity, (basePoint - p2)));
                    c = (p2 - basePoint).LengthSquared - 1.0f;
                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        t = newT;
                        foundCollision = true;
                        collisionPoint = p2;
                    }

                    //P3
                    b = 2*(Vector3.Dot(velocity, (basePoint - p3)));
                    c = (p3 - basePoint).LengthSquared - 1.0f;
                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        t = newT;
                        foundCollision = true;
                        collisionPoint = p3;
                    }

                    //Check against edges

                    //p1 -> p2
                    Vector3 edge = p2 - p1;
                    Vector3 baseToVertex = p1 - basePoint;
                    float edgeSquaredLength = edge.LengthSquared;
                    float edgeDotVelocity = Vector3.Dot(edge, velocity);
                    float edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

                    //Calculate parameters for eqn
                    a = edgeSquaredLength*-velocitySquaredLength + edgeDotVelocity*edgeDotVelocity;
                    b = edgeSquaredLength*(2*Vector3.Dot(velocity, baseToVertex)) -
                        2*edgeDotVelocity*edgeDotBaseToVertex;
                    c = edgeSquaredLength*(1 - baseToVertex.LengthSquared) + edgeDotBaseToVertex*edgeDotBaseToVertex;

                    //Does the swept sphere collide against inifinte edge?

                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        //check if intersection is within line segment
                        float f = (edgeDotVelocity*newT - edgeDotBaseToVertex)/edgeSquaredLength;
                        if (f >= 0.0 && f <= 1.0)
                        {
                            //intersection took place within segment
                            t = newT;
                            foundCollision = true;
                            collisionPoint = p1 + f*edge;
                        }
                    }

                    //p2 -> p3
                    edge = p3 - p2;
                    baseToVertex = p2 - basePoint;
                    edgeSquaredLength = edge.LengthSquared;
                    edgeDotVelocity = Vector3.Dot(edge, velocity);
                    edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

                    //Calculate parameters for eqn
                    a = edgeSquaredLength * -velocitySquaredLength + edgeDotVelocity * edgeDotVelocity;
                    b = edgeSquaredLength * (2 * Vector3.Dot(velocity, baseToVertex)) -
                        2 * edgeDotVelocity * edgeDotBaseToVertex;
                    c = edgeSquaredLength * (1 - baseToVertex.LengthSquared) + edgeDotBaseToVertex * edgeDotBaseToVertex;

                    //Does the swept sphere collide against inifinte edge?

                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        //check if intersection is within line segment
                        float f = (edgeDotVelocity * newT - edgeDotBaseToVertex) / edgeSquaredLength;
                        if (f >= 0.0 && f <= 1.0)
                        {
                            //intersection took place within segment
                            t = newT;
                            foundCollision = true;
                            collisionPoint = p2 + f * edge;
                        }
                    }

                    //p3 -> p1
                    edge = p1 - p3;
                    baseToVertex = p3 - basePoint;
                    edgeSquaredLength = edge.LengthSquared;
                    edgeDotVelocity = Vector3.Dot(edge, velocity);
                    edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

                    //Calculate parameters for eqn
                    a = edgeSquaredLength * -velocitySquaredLength + edgeDotVelocity * edgeDotVelocity;
                    b = edgeSquaredLength * (2 * Vector3.Dot(velocity, baseToVertex)) -
                        2 * edgeDotVelocity * edgeDotBaseToVertex;
                    c = edgeSquaredLength * (1 - baseToVertex.LengthSquared) + edgeDotBaseToVertex * edgeDotBaseToVertex;

                    //Does the swept sphere collide against inifinte edge?

                    if (Utilities.GetLowestRoot(a, b, c, t, out newT))
                    {
                        //check if intersection is within line segment
                        float f = (edgeDotVelocity * newT - edgeDotBaseToVertex) / edgeSquaredLength;
                        if (f >= 0.0 && f <= 1.0)
                        {
                            //intersection took place within segment
                            t = newT;
                            foundCollision = true;
                            collisionPoint = p3 + f * edge;
                        }
                    }
                }

                if (foundCollision)
                {
                    //distance to collision: t is time of collision
                    float distToCollision = t*colPacket.Velocity.Length;

                    //Does this triangle qualify for teh closest hit?
                    //it does if its first hit or closest
                    if (colPacket.FoundCollision == false || distToCollision < colPacket.NearestDistance)
                    {
                        //Collision info necessary for sliding
                        colPacket.NearestDistance = distToCollision;
                        colPacket.IntersectionPoint = collisionPoint;
                        colPacket.FoundCollision = true;
                    }

                    
                }

                colFound = foundCollision;
            }
        }
    }
}
