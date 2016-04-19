using System;

namespace KonikaGlo.colors
{
    public class Conversion
    {
        public static RGB XYZtoRGB(double x, double y, double z)
        {
            double[] Clinear = new double[3];
            Clinear[0] = x * 3.2410 - y * 1.5374 - z * 0.4986; // red
            Clinear[1] = -x * 0.9692 + y * 1.8760 - z * 0.0416; // green
            Clinear[2] = x * 0.0556 - y * 0.2040 + z * 1.0570; // blue

            for (int i = 0; i < 3; i++)
            {
                Clinear[i] = (Clinear[i] <= 0.0031308) ? 12.92 * Clinear[i] : (
                    1 + 0.055) * Math.Pow(Clinear[i], (1.0 / 2.4)) - 0.055;
            }

            return new RGB(
                Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
                    Clinear[0] * 255.0))),
                Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
                    Clinear[1] * 255.0))),
                Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
                    Clinear[2] * 255.0)))
                );
        }

        /// <summary>
        /// Converts CIEXYZ to CIELab.
        /// </summary>
        public static CIELab XYZtoLab(double x, double y, double z)
        {
            CIELab lab = CIELab.Empty;

            lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
            lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
            lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

            return lab;
        }

        /// <summary>
        /// Converts YUV to RGB.
        /// </summary>
        /// <param name="y">Y must be in [0, 1].</param>
        /// <param name="u">U must be in [-0.436, +0.436].</param>
        /// <param name="v">V must be in [-0.615, +0.615].</param>
        public static RGB YUVtoRGB(double y, double u, double v)
        {
            RGB rgb = new RGB();

            rgb.Red = Convert.ToInt32((y + 1.139837398373983740 * v) * 255);
            rgb.Green = Convert.ToInt32((
                y - 0.3946517043589703515 * u - 0.5805986066674976801 * v) * 255);
            rgb.Blue = Convert.ToInt32((y + 2.032110091743119266 * u) * 255);

            return rgb;
        }

        private static double Fxyz(double t)
        {
            return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
        }

      
    }
}


