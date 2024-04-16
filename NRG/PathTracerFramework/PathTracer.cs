using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PathTracer.Samplers;

namespace PathTracer
{
    class PathTracer
    {
        const int max_depth = 5;
        /// <summary>
        /// Given Ray r and Scene s, trace the ray over the scene and return the estimated radiance
        /// </summary>
        /// <param name="r">Ray direction</param>
        /// <param name="s">Scene to trace</param>
        /// <returns>Estimated radiance in the ray direction</returns>
        public Spectrum Li(Ray r, Scene s)
        {
            var L = Spectrum.ZeroSpectrum;
            var beta = Spectrum.Create(1);
            var depth = 0;  // current depth/bounces
            bool specularBounce = false;

            while (depth < max_depth) {
                (double? t, SurfaceInteraction si) = s.Intersect(r);
                // Add emitted light at path vertex or from the environment
                

                // if no intersection, path sampling iteration terminates
                if (!t.HasValue) break;
                
                Vector3 wo = -r.d;
                // if the intersection is with a light, add the light's contribution
                if (si.Obj is Light)
                {
                    var light = si.Obj as Light;
                    if (depth == 0 || specularBounce)
                    {
                        L.AddTo(beta * light.L(si, wo));
                    }
                    break;
                }

                Shape shape = (Shape) si.Obj;
                
                Spectrum f = null;
                Vector3 wiW = Vector3.ZeroVector;
                double pdf = 0;

                (f, wiW, pdf, specularBounce) = shape.BSDF.Sample_f(wo, si);  // TODO: check if this is correct

                // if the current surface has no effect on light
                // skip over medium boundaries
                if (shape.BSDF.Equals(null)) {  // TODO: check if this is correct
                    r = si.SpawnRay(r.d);
                    depth--;
                    continue;
                }

                // Sample illumination from lights to find path contribution
                L.AddTo(beta * Light.UniformSampleOneLight(si, s));

                if (f.IsBlack() || pdf == 0) break;

                beta *= f * Vector3.AbsDot(wiW, si.Normal) / pdf;
                r = si.SpawnRay(wiW);

                // Russian roulette
                if (depth > 3) {
                    double q = Math.Max(0.05, 1 - beta.Max());
                    if (ThreadSafeRandom.NextDouble() < q) break;
                    beta /= 1 - q;
                }
                depth++;
            }
            return L;
        }
    }
}
