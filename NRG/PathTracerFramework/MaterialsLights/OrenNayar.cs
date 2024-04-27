using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer
{
    /// <summary>
    /// Example BxDF implementation of a perfect lambertian surface
    /// </summary>
    public class OrenNayar : BxDF
    {
        private Spectrum kd;
        private float sigma;
        private float A;
        private float B;
        
        public OrenNayar(Spectrum r, double sigma_p, double A_p, double B_p)
        {   
            // convert sigma_p to radians
            sigma = (float) sigma_p;
            kd = r;
            float sigma2 = sigma * sigma;
            A = 1.0f - (sigma2 / (2 * (sigma2 + 0.33f)));
            B = 0.45f * sigma2 / (sigma2 + 0.09f);     
        }


        // compute cosine term of Oren–Nayar model
        public float OrenNayarCosine(float sinThetaI, float sinThetaO, Vector3 wo, Vector3 wi) {
            float maxCos = 0.0f;
            if (sinThetaI > 1e-4 && sinThetaO > 1e-4) {
                float sinPhiI = (float) Utils.SinPhi(wo);
                float cosPhiI = (float) Utils.CosPhi(wo);
                float sinPhiO = (float) Utils.SinPhi(wi);
                float cosPhiO = (float) Utils.CosPhi(wi);
                float dCos = cosPhiI * cosPhiO + sinPhiI * sinPhiO;
                maxCos = Math.Max(0, dCos);
            }
            return maxCos;
        }

        // compute sine and tangent terms of Oren–Nayar model
        public (float, float) OrenNayarSineAndTangent(float sinThetaI, float sinThetaO, Vector3 wo, Vector3 wi) {
            float sinAlpha, tanBeta;
            if (Utils.AbsCosTheta(wi) > Utils.AbsCosTheta(wo)){
                sinAlpha = sinThetaO;
                tanBeta = sinThetaI / (float) Utils.AbsCosTheta(wi);
            } else {
                sinAlpha = sinThetaI;
                tanBeta = sinThetaO / (float) Utils.AbsCosTheta(wo);
            }
            return (sinAlpha, tanBeta);
        }



        /// <summary>
        /// Lambertian f is kd/pi
        /// </summary>
        /// <param name="wo">output vector</param>
        /// <param name="wi">input vector</param>
        /// <returns></returns>
        public override Spectrum f(Vector3 wo, Vector3 wi)
        {
            if (!Utils.SameHemisphere(wo, wi))
                return Spectrum.ZeroSpectrum;

            // float sinThetaI = Math.sqrt(Math.max((float)0, (float)1 - (wi.z * wi.z)))
            float sinThetaI = (float) Utils.SinTheta(wi);
            float sinThetaO = (float) Utils.SinTheta(wo);
            // float sinThetaO = Math.sqrt(Math.max((float)0, (float)1 - (wo.z * wo.z)))

            // compute cosine term of Oren–Nayar model
            float maxCos = OrenNayarCosine(sinThetaI, sinThetaO, wo, wi);
            (float sinAlpha, float tanBeta) = OrenNayarSineAndTangent(sinThetaI, sinThetaO, wo, wi);

            return kd * Utils.PiInv * (A + B * maxCos * sinAlpha * tanBeta);
        }


        /// <summary>
        /// Cosine weighted sampling of wi
        /// </summary>
        /// <param name="wo">wo in local</param>
        /// <returns>(f, wi, pdf)</returns>
        public override (Spectrum, Vector3, double) Sample_f(Vector3 wo)
        {
            var wi = Samplers.CosineSampleHemisphere();
            if (wo.z < 0)
                wi.z *= -1;
            double pdf = Pdf(wo, wi);
            return (f(wo, wi), wi, pdf);
        }

        /// <summary>
        /// returns pdf(wo,wi) as |cosTheta|/pi
        /// </summary>
        /// <param name="wo">output vector in local</param>
        /// <param name="wi">input vector in local</param>
        /// <returns></returns>
        public override double Pdf(Vector3 wo, Vector3 wi)
        {
            if (!Utils.SameHemisphere(wo, wi))
                return 0;

            return Math.Abs(wi.z) * Utils.PiInv; // wi.z == cosTheta
        }
    }
}
