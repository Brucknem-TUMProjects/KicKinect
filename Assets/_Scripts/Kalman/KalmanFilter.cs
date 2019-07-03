public class KalmanFilter
{
    //public Matrix<double> A { get; private set; }
    //public Matrix<double> F { get; private set; }
    //public Matrix<double> H { get; private set; }
    //private Matrix<double> X { get; set; }

    //public Matrix<double> Q
    //{
    //    get
    //    {
    //        return DenseMatrix.OfArray(new double[2, 2] { { qf / 3, qf / 2 }, { qf / 2, qf } });
    //    }
    //}

    //private Matrix<double> P { get; set; }

    //public Matrix<double> R
    //{
    //    get
    //    {
    //        return DenseMatrix.OfArray(new double[1, 1] { { r } });
    //    }
    //}

    //private double r = 0.1;
    //private double qf = 0.00001;
    //private double p = 1000;

    //private Matrix<double> x_hat_t_tmin;
    //private Matrix<double> P_t_tmin;
    //private Matrix<double> K;

    //public KalmanFilter(float dt)
    //{
    //    A = DenseMatrix.OfArray(new double[2, 2] { { 0, 1 }, { 0, 0 } });
    //    F = DenseMatrix.OfArray(new double[2, 2] { { 1, dt }, { 0, 1 } });
    //    H = DenseMatrix.OfArray(new double[1, 2] { { 1, 0 } });

    //    X = DenseMatrix.OfArray(new double[2, 1] { { 0 }, { 0 } });
    //    P = DenseMatrix.OfArray(new double[2, 2] { { p, 0 }, { 0, p } });
    //}

    //private void Predict()
    //{
    //    x_hat_t_tmin = F * X;
    //    P_t_tmin = F * P * F.Transpose() + Q;
    //}

    //private void Update(double measurement)
    //{
    //    Matrix<double> y = DenseMatrix.OfArray(new double[2, 1] { { measurement }, { 0 } });
    //    K = P_t_tmin * H.Transpose() * (H * P_t_tmin * H.Transpose() + R).Inverse();
    //    X = x_hat_t_tmin + K * (y - H * x_hat_t_tmin);
    //    P = (DenseMatrix.CreateIdentity(2) - K * H) * P_t_tmin;
    //}

    //public double Calculate(double measurement)
    //{
    //    Predict();
    //    Update(measurement);
    //    return X.At(0,0);
    //}
}