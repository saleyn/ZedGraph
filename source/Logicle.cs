using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZedGraph
{
    public class Logicle
    {
        static readonly double LN_10 = Math.Log(10);
        public const double DEFAULT_DECADES = 4.5;
        public const int TAYLOR_LENGTH = 16;

        protected struct logicle_params
        {
            internal double T, W, M, A;
            internal double a, b, c, d, f;
            internal double w, x0, x1, x2;
            internal double xTaylor;
            internal double[] taylor;

            internal double[] lookup;
            internal int bins;
        };

        protected logicle_params p;

        public Logicle(double T, double W, double M = DEFAULT_DECADES, double A = 0)
        {
            initialize(T, W, M, A, 0);
        }

        protected Logicle(double T, double W, double M, double A, int bins)
        {
            initialize(T, W, M, A, bins);
        }

        public double T
        {
            get { return p.T; }
        }

        public double W
        {
            get { return p.W; }
        }

        public double M
        {
            get { return p.M; }
        }

        public double A
        {
            get { return p.A; }
        }


        protected void initialize(double T, double W, double M, double A, int bins)
        {
            if (T <= 0)
                throw new Exception("IllegalParameter: T is not positive");
            //throw IllegalParameter("T is not positive");
            if (W < 0)
                throw new Exception("IllegalParameter: W is not positive");
            //throw IllegalParameter("W is not positive");
            if (M <= 0)
                throw new Exception("IllegalParameter: M is not positive");
            //throw IllegalParameter("M is not positive");
            if (2 * W > M)
                throw new Exception("IllegalParameter: W is too large");
            //throw IllegalParameter("W is too large");
            if (-A > W || A + W > M - W)
                throw new Exception("IllegalParameter: A is too large");
            //throw IllegalParameter("A is too large");

            // if we're going to bin the data make sure that
            // zero is on a bin boundary by adjusting A
            if (bins > 0)
            {
                double zero = (W + A) / (M + A);
                zero = Math.Floor(zero * bins + .5) / bins;
                A = (M * zero - W) / (1 - zero);
            }

            // standard parameters
            p.T = T;
            p.M = M;
            p.W = W;
            p.A = A;

            // actual parameters
            // formulas from biexponential paper
            p.w = W / (M + A);
            p.x2 = A / (M + A);
            p.x1 = p.x2 + p.w;
            p.x0 = p.x2 + 2 * p.w;
            p.b = (M + A) * LN_10;
            p.d = solve(p.b, p.w);
            double c_a = Math.Exp(p.x0 * (p.b + p.d));
            double mf_a = Math.Exp(p.b * p.x1) - c_a / Math.Exp(p.d * p.x1);
            p.a = T / ((Math.Exp(p.b) - mf_a) - c_a / Math.Exp(p.d));
            p.c = c_a * p.a;
            p.f = -mf_a * p.a;

            // use Taylor series near x1, i.e., data zero to
            // avoid round off problems of formal definition
            p.xTaylor = p.x1 + p.w / 4;
            // compute coefficients of the Taylor series
            double posCoef = p.a * Math.Exp(p.b * p.x1);
            double negCoef = -p.c / Math.Exp(p.d * p.x1);
            // 16 is enough for full precision of typical scales
            p.taylor = new double[TAYLOR_LENGTH];
            for (int i = 0; i < TAYLOR_LENGTH; ++i)
            {
                posCoef *= p.b / (i + 1);
                negCoef *= -p.d / (i + 1);
                (p.taylor)[i] = posCoef + negCoef;
            }
            p.taylor[1] = 0; // exact result of Logicle condition
        }

        class sfun_info
        {
            public double b, w;
        };

        // f(w,b) = 2 * (ln(d) - ln(b)) + w * (b + d)
        double logicle_fn(double x, dynamic info)
        {
            sfun_info p = (sfun_info)info;
            double B = 2 * (Math.Log(x) - Math.Log(p.b)) + p.w * (p.b + x);
            return (B);
        }

        delegate double R_zeroin_fn(double x, dynamic info);

        /*
         * root finder routines are copied from stats/src/zeroin.c
         */
        double R_zeroin(                    /* An estimate of the root */
            double ax,                      /* Left border | of the range	*/
            double bx,                      /* Right border| the root is seeked*/
            R_zeroin_fn f,	                /* Function under investigation	*/
            dynamic info,				    /* Add'l info passed on to f	*/
            ref double Tol,			        /* Acceptable tolerance		*/
            ref int Maxit)				    /* Max # of iterations */
        {
            double fa = f(ax, info);
            double fb = f(bx, info);
            return R_zeroin2(ax, bx, fa, fb, f, info, ref Tol, ref Maxit);
        }

        /* R_zeroin2() is faster for "expensive" f(), in those typical cases where
         *             f(ax) and f(bx) are available anyway : */

        double R_zeroin2(			        /* An estimate of the root */
            double ax,				        /* Left border | of the range	*/
            double bx,				        /* Right border| the root is seeked*/
            double fa, double fb,	        /* f(a), f(b) */
            R_zeroin_fn f,	                /* Function under investigation	*/
            dynamic info,				    /* Add'l info passed on to f	*/
            ref double Tol,			        /* Acceptable tolerance		*/
            ref int Maxit)				    /* Max # of iterations */
        {
            double a, b, c, fc;             /* Abscissae, descr. see above,  f(c) */
            double tol;
            int maxit;

            a = ax; b = bx;
            c = a; fc = fa;
            maxit = Maxit + 1; tol = Tol;

            /* First test if we have found a root at an endpoint */
            if (fa == 0.0)
            {
                Tol = 0.0;

                Maxit = 0;
                return a;
            }

            if (fb == 0.0)
            {
                Tol = 0.0;
                Maxit = 0;
                return b;
            }

            while (maxit-- != 0)		    /* Main iteration loop	*/
            {
                double prev_step = b - a;       /* Distance from the last but one
					               to the last approximation	*/
                double tol_act;         /* Actual tolerance		*/
                double p;           /* Interpolation step is calcu- */
                double q;           /* lated in the form p/q; divi-
					                 * sion operations is delayed
					                 * until the last moment	*/
                double new_step;        /* Step at this iteration	*/

                if (Math.Abs(fc) < Math.Abs(fb))
                {               /* Swap data for b to be the	*/
                    a = b; b = c; c = a;    /* best approximation		*/
                    fa = fb; fb = fc; fc = fa;
                }

                tol_act = 2 * Double.Epsilon * Math.Abs(b) + tol / 2;
                new_step = (c - b) / 2;

                if (Math.Abs(new_step) <= tol_act || fb == (double)0)
                {
                    Maxit -= maxit;
                    Tol = Math.Abs(c - b);
                    return b;           /* Acceptable approx. is found	*/
                }

                /* Decide if the interpolation can be tried	*/
                if (Math.Abs(prev_step) >= tol_act  /* If prev_step was large enough*/
                    && Math.Abs(fa) > Math.Abs(fb))
                {   /* and was in true direction,
					             * Interpolation may be tried	*/
                    double t1, cb, t2;
                    cb = c - b;
                    if (a == c)
                    {       /* If we have only two distinct	*/
                            /* points linear interpolation	*/
                        t1 = fb / fa;       /* can only be applied		*/
                        p = cb * t1;
                        q = 1.0 - t1;
                    }
                    else
                    {           /* Quadric inverse interpolation*/

                        q = fa / fc; t1 = fb / fc; t2 = fb / fa;
                        p = t2 * (cb * q * (q - t1) - (b - a) * (t1 - 1.0));
                        q = (q - 1.0) * (t1 - 1.0) * (t2 - 1.0);
                    }
                    if (p > (double)0)      /* p was calculated with the */
                        q = -q;         /* opposite sign; make p positive */
                    else            /* and assign possible minus to	*/
                        p = -p;         /* q				*/

                    if (p < (0.75 * cb * q - Math.Abs(tol_act * q) / 2) /* If b+p/q falls in [b,c]*/
                    && p < Math.Abs(prev_step * q / 2)) /* and isn't too large	*/
                        new_step = p / q;           /* it is accepted
						         * If p/q is too large then the
						         * bisection procedure can
						         * reduce [b,c] range to more
						         * extent */
                }

                if (Math.Abs(new_step) < tol_act)
                {   /* Adjust the step to be not less*/
                    if (new_step > (double)0)   /* than tolerance		*/
                        new_step = tol_act;
                    else
                        new_step = -tol_act;
                }
                a = b; fa = fb;         /* Save the previous approx. */
                b += new_step; fb = f(b, info); /* Do step to a new approxim. */
                if ((fb > 0 && fc > 0) || (fb < 0 && fc < 0))
                {
                    /* Adjust c for it to have a sign opposite to that of b */
                    c = a; fc = fa;
                }

            }
            /* failed! */
            Tol = Math.Abs(c - b);
            Maxit = -1;
            return b;
        }
        /*
         * use R built-in root finder API :R_zeroin
         */
        double solve(double b, double w)
        {
            // w == 0 means its really arcsinh
            if (w == 0)
                return b;

            // precision is the same as that of b
            double tolerance = 2 * b * Double.Epsilon;

            sfun_info param = new sfun_info() { b = b, w = w };

            // bracket the root
            double d_lo = 0;
            double d_hi = b;


            int MaxIt = 20;
            double d;
            d = R_zeroin(d_lo, d_hi, logicle_fn, param, ref tolerance, ref MaxIt);
            return d;
        }

        double slope(double scale)
        {
            // reflect negative scale regions
            if (scale < p.x1)

                scale = 2 * p.x1 - scale;

            // compute the slope of the biexponential
            return p.a * p.b * Math.Exp(p.b * scale) + p.c * p.d / Math.Exp(p.d * scale);
        }

        double seriesBiexponential(double scale)
        {
            // Taylor series is around x1
            double x = scale - p.x1;
            // note that taylor[1] should be identically zero according
            // to the Logicle condition so skip it here
            double sum = p.taylor[TAYLOR_LENGTH - 1] * x;
            for (int i = TAYLOR_LENGTH - 2; i >= 2; --i)
                sum = (sum + p.taylor[i]) * x;
            return (sum * x + p.taylor[0]) * x;
        }

        virtual public double scale(double value)
        {
            // handle true zero separately
            if (value == 0)
                return p.x1;

            // reflect negative values
            bool negative = value < 0;
            if (negative)
                value = -value;

            // initial guess at solution
            double x;
            if (value < p.f)
                // use linear approximation in the quasi linear region
                x = p.x1 + value / p.taylor[0];
            else
                // otherwise use ordinary logarithm
                x = Math.Log(value / p.a) / p.b;

            // try for double precision unless in extended range
            double tolerance = 3 * Double.Epsilon;
            if (x > 1)
                tolerance = 3 * x * Double.Epsilon;

            for (int i = 0; i < 10; ++i)
            {
                // compute the function and its first two derivatives
                double ae2bx = p.a * Math.Exp(p.b * x);
                double ce2mdx = p.c / Math.Exp(p.d * x);
                double y;
                if (x < p.xTaylor)
                    // near zero use the Taylor series
                    y = seriesBiexponential(x) - value;
                else
                    // this formulation has better roundoff behavior
                    y = (ae2bx + p.f) - (ce2mdx + value);
                double abe2bx = p.b * ae2bx;
                double cde2mdx = p.d * ce2mdx;
                double dy = abe2bx + cde2mdx;
                double ddy = p.b * abe2bx - p.d * cde2mdx;

                // this is Halley's method with cubic convergence
                double delta = y / (dy * (1 - y * ddy / (2 * dy * dy)));
                x -= delta;

                // if we've reached the desired precision we're done
                if (Math.Abs(delta) < tolerance)
                {
                    // handle negative arguments
                    if (negative)
                        return 2 * p.x1 - x;
                    else
                        return x;
                }
            }

            throw new Exception("DidNotConverge: scale() didn't converge");
            //throw DidNotConverge("scale() didn't converge");
        }

        virtual public double inverse(double scale)
        {
            // reflect negative scale regions
            bool negative = scale < p.x1;
            if (negative)
                scale = 2 * p.x1 - scale;

            // compute the biexponential
            double inverse;
            if (scale < p.xTaylor)
                // near x1, i.e., data zero use the series expansion
                inverse = seriesBiexponential(scale);
            else
                // this formulation has better roundoff behavior
                inverse = (p.a * Math.Exp(p.b * scale) + p.f) - p.c / Math.Exp(p.d * scale);

            // handle scale for negative values
            if (negative)
                return -inverse;
            else
                return inverse;
        }

        double dynamicRange()
        {
            return slope(1) / slope(p.x1);
        }
    }

    class FastLogicle : Logicle
    {
        const int DEFAULT_BINS = (1 << 12);


        void initialize(int bins)
        {
            p.bins = bins;
            p.lookup = new double[bins + 1];
            for (int i = 0; i <= bins; ++i)
                p.lookup[i] = base.inverse((double)i / (double)bins);
        }

        public FastLogicle(double T, double W, double M, double A, int bins)
            : base(T, W, M, A, bins)
        {
            initialize(bins);
        }

        public FastLogicle(double T, double W, double M, int bins)
            : base(T, W, M, 0, bins)
        {
            initialize(bins);
        }

        public FastLogicle(double T, double W, int bins)
            : base(T, W, DEFAULT_DECADES, 0, bins)
        {
            initialize(bins);
        }

        public FastLogicle(double T, double W, double M, double A)
            : base(T, W, M, A, DEFAULT_BINS)
        {
            initialize(DEFAULT_BINS);
        }

        public FastLogicle(double T, double W, double M)
            : base(T, W, M, 0, DEFAULT_BINS)
        {
            initialize(DEFAULT_BINS);
        }

        public FastLogicle(double T, double W)
            : base(T, W, DEFAULT_DECADES, 0, DEFAULT_BINS)
        {
            initialize(DEFAULT_BINS);
        }

        int intScale(double value)
        {
            // binary search for the appropriate bin
            int lo = 0;
            int hi = p.bins;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                double key = p.lookup[mid];
                if (value < key)
                    hi = mid - 1;
                else if (value > key)
                    lo = mid + 1;
                else if (mid < p.bins)
                    return mid;
                else
                    // equal to table[bins] which is for interpolation only
                    throw new Exception(String.Format("IllegalArgument {0}", value));
            }

            // check for out of range
            if (hi < 0 || lo > p.bins)
                throw new Exception(String.Format("IllegalArgument {0}", value));

            return lo - 1;
        }

        override public double scale(double value)
        {
            // lookup the nearest value
            int index = intScale(value);

            // inverse interpolate the table linearly
            double delta = (value - p.lookup[index])
              / (p.lookup[index + 1] - p.lookup[index]);

            return (index + delta) / (double)p.bins;
        }

        override public double inverse(double scale)
        {
            // find the bin
            double x = scale * p.bins;
            int index = (int)Math.Floor(x);
            if (index < 0 || index >= p.bins)
                throw new Exception(String.Format("IllegalArgument {0}", scale));

            // interpolate the table linearly
            double delta = x - index;

            return (1 - delta) * p.lookup[index] + delta * p.lookup[index + 1];
        }

        public double inverse(int index)
        {
            if (index < 0 || index >= p.bins)
                throw new Exception(String.Format("IllegalArgument {0}", index));

            return p.lookup[index];
        }
    }
}
