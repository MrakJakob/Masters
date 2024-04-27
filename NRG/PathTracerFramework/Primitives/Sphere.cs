using System;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using MathNet.Numerics;
using MathNet.Numerics.Integration;

namespace PathTracer
{
    /// <summary>
    /// Sphere Shape template class - NOT implemented completely
    /// </summary>
    class Sphere : Shape
    {
        public double Radius { get; set; }
        public Sphere(double radius, Transform objectToWorld)
        {
            Radius = radius;
            ObjectToWorld = objectToWorld;
        }

        /// <summary>
        /// Ray-Sphere intersection - NOT implemented
        /// </summary>
        /// <param name="r">Ray</param>
        /// <returns>t or null if no hit, point on surface</returns>
        public override (double?, SurfaceInteraction) Intersect(Ray ray)
        {
            Ray r = WorldToObject.Apply(ray);

            // TODO: Initialize _double_ ray coordinate values
            double ox = r.o.x, oy = r.o.y, oz = r.o.z;
            double dx = r.d.x, dy = r.d.y, dz = r.d.z;
            // TODO: Compute quadratic sphere coefficients
            double a = dx * dx + dy * dy + dz * dz;
            double b = 2 * (dx * ox + dy * oy + dz * oz);
            double c = ox * ox + oy * oy + oz * oz - Radius * Radius;

            // TODO: Solve quadratic equation for _t_ values
            (bool real_solutions, double t0, double t1) = Utils.Quadratic(a, b, c);
            if (!real_solutions) return (null, null);

            // Check quadric shape _t0_ and _t1_ for nearest intersection
            if (t1 <= 0) return (null, null);
            double tShapeHit = t0;
            if (tShapeHit <= 0)
            {
                tShapeHit = t1;
                if (tShapeHit <= 0) return (null, null);
            }
            
            // Compute sphere hit position and $\phi$
            Vector3 pHit = r.Point(tShapeHit);
            
            // scale pHit
            pHit *= Radius / pHit.Length();

            Vector3 dpdu = new Vector3(-2 * Math.PI * pHit.y, 2 * Math.PI * pHit.x, 0);

            // return shape hit and surface interaction
            Vector3 wo = -r.d;


            SurfaceInteraction si = new SurfaceInteraction(pHit, pHit, wo, dpdu, this);
            return (tShapeHit, ObjectToWorld.Apply(si));
        }

        /// <summary>
        /// Sample point on sphere in world
        /// </summary>
        /// <returns>point in world, pdf of point</returns>
        public override (SurfaceInteraction, double) Sample()
        {
            // Implement Sphere sampling
            Vector3 pObj = new Vector3(0, 0, 0) + Radius * Samplers.UniformSampleSphere();

            Vector3 normal = pObj.Clone().Normalize();

            // reproject pObj to sphere surface
            pObj = pObj * (Radius / pObj.Length());

            Vector3 wo = Vector3.ZeroVector;
            Vector3 dpdu = new Vector3(-pObj.y, pObj.x, 0);


            double pdf = 1 / Area();
            SurfaceInteraction si = new SurfaceInteraction(pObj, normal, wo, dpdu, this);
            // Return surface interaction and pdf
            return (ObjectToWorld.Apply(si), pdf);
        }

        public override double Area() { return 4 * Math.PI * Radius * Radius; }

        /// <summary>
        /// Estimates pdf of wi starting from point si
        /// </summary>
        /// <param name="si">point on surface that wi starts from</param>
        /// <param name="wi">wi</param>
        /// <returns>pdf of wi given this shape</returns>
        public override double Pdf(SurfaceInteraction si, Vector3 wi)
        {
            return 1 / Area();
        }

    }
}
