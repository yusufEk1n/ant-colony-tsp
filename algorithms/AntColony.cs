using TravellingSalesman.models;

namespace TravellingSalesman.algorithms
{
    public class AntColony
    {
        public static List<Vertex> _vertices;
        public static int nn = 20;
        public static int seed = 1563895359;
        public static bool dlb_flag = true;
        private static Random _random = new Random();
        // Feromonun yön seçme üzerindeki etkisini belirlerken kullanılacak olan katsayı.
        private static int _alpha = 1;
        // Mesafenin yön seçme üzerindeki etkisini belirlerken kullanılacak olan katsayı.
        private static int _beta = 3;
        // Feromonun buharlaşma katsayısı.
        private static double _rho = 0.1;
        // Feromon artırma katsayısı.
        private static double _Q = 2.0;

        public AntColony(List<Vertex> vertices)
        {
            _vertices = vertices;
        }

        public void AntColonyAlgorithm()
        {
            try
            {
                int citiesCount = _vertices.Count;
                int antsCount = 20;
                int iterationsCount = 15;

                // Mesafeleri hesaplayıp bir graf oluşturur.
                double[][] distance = InitializeDistanceMatrix(_vertices);

                int[][] nnList = ComputeNNList();

                // Her bir karınca için rastgele bir yol oluşturur.
                int[][] ants = InitializeAnts(antsCount, citiesCount, nnList);
                // Her iki mesafe arasındaki feromon miktarını başlatır.
                double[][] phromones = InitializePhromones(citiesCount);


                // En iyi rotayı tutar.
                int[] bestTrail = BestTrail(ants, distance);
                // En iyi rotanın uzunluğunu tutar.
                double bestLength = TrailLength(bestTrail, distance);

                GlobalUpdatePheromones(ants, phromones, distance);

                int time = 0;
    
                while (time < iterationsCount)
                {
                    UpdateAnts(ants, phromones, distance, nnList);

                    int[] currentBestTrail = BestTrail(ants, distance);
                    double currentBestLength = TrailLength(currentBestTrail, distance);

                    if (currentBestLength < bestLength)
                    {
                        bestLength = currentBestLength;
                        bestTrail = currentBestTrail;
                        Console.WriteLine("Iteration: " + time + " Best Length: " + bestLength);
                    }
                    time += 1;
                }

                Console.WriteLine($"[{string.Join(", ", bestTrail)}]");
                Console.WriteLine($"Best Length: {(int)bestLength}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region GlobalUpdatePheromones
        private void GlobalUpdatePheromones(int[][] ants, double[][] phromones, double[][] distance)
        {
            double minPheromone = 0.0001;
            double maxPheromone = 100000.0;

            for (int i = 0; i < phromones.Length; i++)
            {
                for (int j = i + 1; j < phromones[i].Length; j++)
                {
                    double decrease = (1 - _rho) * phromones[i][j];
                    double increase = 0.0;

                    for (int a = 0; a < ants.Length; a++)
                    {
                        if (EdgeInTrail(i, j, ants[a]))
                        {
                            double length = TrailLength(ants[a], distance);
                            increase += _Q / length;
                        }
                    }

                    double pheromone = decrease + increase;

                    phromones[i][j] = pheromone < minPheromone ? (minPheromone) :
                                      pheromone > maxPheromone ? maxPheromone : pheromone;

                    phromones[j][i] = phromones[i][j];
                }
            }
        }
        #endregion

        #region EdgeInTrail
        private static bool EdgeInTrail(int cityX, int cityY, int[] trail)
        {
            int lastIndex = trail.Length - 1;
            int index = Array.IndexOf(trail, cityX);

            if (index == 0 && (trail[1] == cityY || trail[lastIndex] == cityY))
            {
                return true;
            }
            else if (index == 0)
            {
                return false;
            }
            if (index == lastIndex && (trail[lastIndex - 1] == cityY || trail[0] == cityY))
            {
                return true;
            }
            else if (index == lastIndex)
            {
                return false;
            }
            else if (trail[index - 1] == cityY)
            {
                return true;
            }
            else if (trail[index + 1] == cityY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Karıncaların yolunu günceller. 
        /// </summary>
        /// <param name="ants"></param>
        /// <param name="pheromones"></param>
        /// <param name="distance"></param>
        #region UpdateAnts
        private static void UpdateAnts(int[][] ants, double[][] pheromones, double[][] distance, int[][] nnList)
        {
            for (int a = 0; a < ants.Length; a++)
            {
                int start = _random.Next(pheromones.Length);
                ants[a] = BuildTrail(start, pheromones, distance);
                two_opt_first(ants[a], nnList);
                LocalPhromoneUpdate(ants[a], pheromones, distance, ants);
                Console.WriteLine(TrailLength(ants[a], distance));
            }
        }
        #endregion

        #region LocalPhromoneUpdate
        private static void LocalPhromoneUpdate(int[] ints, double[][] pheromones, double[][] distance, int[][] ants)
        {
            double minPheromone = 0.000001;
            double maxPheromone = 100000.0;

            for (int i = 0; i < ints.Length - 1; i++)
            {
                int cityX = ints[i];
                int cityY = ints[i + 1];

                double length = TrailLength(ints, distance);
                double decrease = (1 - _rho) * pheromones[cityX][cityY];
                double increase = 0.0;

                for(int a = 0; a < ants.Length; a++)
                {
                    if (EdgeInTrail(cityX, cityY, ants[a]))
                    {
                        increase += _Q / length;
                    }
                }
                pheromones[cityX][cityY] += decrease + increase;

                if (pheromones[cityX][cityY] < minPheromone)
                {
                    pheromones[cityX][cityY] = minPheromone;
                }
                else if (pheromones[cityX][cityY] > double.MaxValue / 2)
                {
                    pheromones[cityX][cityY] = maxPheromone;
                }

                pheromones[cityY][cityX] += pheromones[cityX][cityY];
            }
        }
        #endregion

        /// <summary>
        /// Karıncaların yolunu oluşturur.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="pheromones"></param>
        /// <param name="distance"></param>
        /// <returns>Oluşturulan yol.</returns>
        #region BuildTrail
        private static int[] BuildTrail(int start, double[][] pheromones, double[][] distance)
        {
            int n = pheromones.Length;

            // Karınca için yeni bir yol oluşturulur.
            int[] trail = new int[n + 1];
            // Ziyaret edilen şehirlerin tutulduğu dizi.
            bool[] visited = new bool[n];

            trail[0] = start;
            visited[start] = true;

            for (int i = 1; i < n; i++)
            {
                int cityX = trail[i - 1];
                int cityY = ChooseNextCity(cityX, visited, pheromones, distance); // feromon ve mesafe tablosuna göre yeni bir yol oluşturulur.
                trail[i] = cityY;
                visited[cityY] = true;
            }
            trail[n] = start;
            return trail;
        }
        #endregion

        /// <summary>
        /// Olasılıklara göre bir sonraki şehri seçer. 
        /// </summary>
        /// <param name="cityX"></param>
        /// <param name="visited"></param>
        /// <param name="pheromones"></param>
        /// <param name="distance"></param>
        /// <returns>Seçilen şehir.</returns>
        #region ChooseNextCity
        private static int ChooseNextCity(int cityX, bool[] visited, double[][] pheromones, double[][] distance)
        {
            // Şehirlerin seçilme olasılıklarını hesaplar.
            double[] probs = GetProbs(cityX, visited, pheromones, distance);

            // Şehirlerin seçilme olasılıklarının toplamını tutar.
            double[] cumul = new double[probs.Length + 1];

            // Şehirlerin seçilme olasılıklarının toplamını hesaplar.
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
            }

            double p = _random.NextDouble();

            for (int i = 0; i <= cumul.Length - 2; i++)
            {
                if (p < cumul[i + 1] && p >= cumul[i])
                {
                    return i;
                }
            }

            // Eğer hiçbir şehir seçilmediyse hata fırlatılır.
            throw new Exception("Geçerli şehir bulunamadı.");
        }
        #endregion

        /// <summary>
        /// Şehirlerin seçilme olasılıklarını hesaplar.
        /// </summary>
        /// <param name="cityX"></param>
        /// <param name="visited"></param>
        /// <param name="pheromones"></param>
        /// <param name="distance"></param>
        /// <returns>Olasılık tablosu</returns>
        #region GetProbs
        private static double[] GetProbs(int cityX, bool[] visited, double[][] pheromones, double[][] distance)
        {
            int n = pheromones.Length;
            double[] probs = new double[n];

            double sum = 0.0;

            for (int i = 0; i <= probs.Length - 1; i++)
            {
                // Ziyaret edilmiş şehirlerin olasılığı 0 olarak ayarlanır.
                if (visited[i] || i == cityX)
                {
                    probs[i] = 0.0;
                }
                else
                {
                    // Olasılık parametreleri hesaplanır.
                    double pheromone = pheromones[cityX][i];        // feromon değerinin olasılığa etkisi
                    double heuristic = 1.0 / distance[cityX][i];    // mesafenin olasılığa etkisi

                    // _alpha ve _beta parametreleri ile olasılık hesaplanır.
                    probs[i] = Math.Pow(pheromone, _alpha) * Math.Pow(heuristic, _beta);

                    // Olasılık değerleri 0.001 ile 1000000 arasında olması tercih edilir.
                    // if (probs[i] < 0.001)
                    // {
                    //     probs[i] = 0.001;
                    // }
                    // else if (probs[i] > (double.MaxValue / (n * 100)))
                    // {
                    //     probs[i] = double.MaxValue / (n * 100);
                    // }
                }
                sum += probs[i];
            }

            // Olasılıkların toplamı 1 olacak şekilde ayarlanır.
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                probs[i] /= sum;
            }

            return probs;
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Her iki mesafe arasındaki feromon miktarını başlatır.
        /// </summary>
        /// <param name="citiesCount"></param>
        /// <returns>Feromonları tutan graf.</returns>
        #region InitializePhromones
        private double[][] InitializePhromones(int citiesCount)
        {
            double[][] phromones = new double[citiesCount][];

            for (int i = 0; i <= citiesCount - 1; i++)
            {
                phromones[i] = new double[citiesCount];
            }

            for (int i = 0; i <= citiesCount - 1; i++)
            {
                for (int j = i + 1; j <= citiesCount - 1; j++)
                {
                    phromones[i][j] = phromones[j][i] = 0.01;
                }
            }

            return phromones;
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gelen karınca rotalarından en iyi olanını seçer.
        /// </summary>
        /// <param name="ants"></param>
        /// <param name="distance"></param>
        /// <returns>En iyi rota.</returns>
        #region BestTrail
        private int[] BestTrail(int[][] ants, double[][] distance)
        {
            double bestLength = TrailLength(ants[0], distance);
            int bestIndex = 0;

            for (int i = 1; i <= ants.Length - 1; i++)
            {
                double length = TrailLength(ants[i], distance);
                if (length < bestLength)
                {
                    bestLength = length;
                    bestIndex = i;
                }
            }

            int numCities = ants[0].Length;
            int[] bestTrail = new int[numCities];
            ants[bestIndex].CopyTo(bestTrail, 0);
            return bestTrail;
        }
        #endregion

        /// <summary>
        /// Gelen rota uzunluğunu hesaplar.
        /// </summary>
        /// <param name="trail"></param>
        /// <param name="distance"></param>
        /// <returns>Rota uzunluğu.</returns>
        #region TrailLength
        private static double TrailLength(int[] trail, double[][] distance)
        {
            double length = 0;
            for (int i = 0; i <= trail.Length - 2; i++)
            {
                length += distance[trail[i]][trail[i + 1]];
            }
            return length;
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Her bir karınca için rastgele bir yol oluşturur. 
        /// </summary>
        /// <param name="antsCount"></param>
        /// <param name="citiesCount"></param>
        /// <returns>Karıncıların oluşturduğu yolları tutan graf.</returns>
        #region InitializeAnts
        private int[][] InitializeAnts(int antsCount, int citiesCount, int[][] nnList)
        {
            int[][] ants = new int[antsCount][];

            for (int i = 0; i <= antsCount - 1; i++)
            {
                int start = _random.Next(0, citiesCount);
                ants[i] = BuildTrail2(start, citiesCount);
                two_opt_first(ants[i], nnList);
            }
            return ants;
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Mesafeleri hesaplayıp bir graf oluşturur.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns>Mesafeleri tutan graf.</returns>
        #region InitializeDistanceMatrix
        private double[][] InitializeDistanceMatrix(List<Vertex> vertices)
        {
            double[][] distance = new double[vertices.Count][];

            for (int i = 0; i <= vertices.Count - 1; i++)
            {
                distance[i] = new double[vertices.Count];
            }

            for (int i = 0; i <= vertices.Count - 1; i++)
            {
                for (int j = i + 1; j <= vertices.Count - 1; j++)
                {
                    distance[i][j] = distance[j][i] = GetDistance(_vertices[i], _vertices[j]);
                }
            }
            return distance;
        }
        #endregion

        /// <summary>
        /// İki nokta arasındaki mesafeyi hesaplar.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns> İki nokta arasındaki mesafe.</returns>
        #region GetDistance
        public static double GetDistance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }
        #endregion

        // ----------------------------------------------------------------------------------------------

        #region ComputeNNList
        static int[][] ComputeNNList()
        {
            int i, node;
            int n = _vertices.Count;
            double[] distanceVector = new double[n];
            int[] helpVector = new int[n];

            if (nn >= n)
                nn = n - 1;

            int[][] mNear = new int[n][];

            for (int k = 0; k < n; k++)
            {
                mNear[k] = new int[nn];
            }

            for (node = 0; node < n; node++)
            {
                for (i = 0; i < n; i++)
                {
                    distanceVector[i] = GetDistance(_vertices[node], _vertices[i]);
                    helpVector[i] = i;
                }

                distanceVector[node] = double.MaxValue;
                QuickSort(distanceVector, helpVector, 0, n - 1);

                for (i = 0; i < nn; i++)
                {
                    mNear[node][i] = helpVector[i];
                }
            }

            return mNear;
        }
        #endregion

        #region QuickSort
        static void QuickSort(double[] v, int[] v2, int left, int right)
        {
            int k, last;

            if (left >= right)
                return;

            swap2(v, v2, left, (left + right) / 2);

            last = left;

            for (k = left + 1; k <= right; k++)
                if (v[k] < v[left])
                    swap2(v, v2, ++last, k);

            swap2(v, v2, left, last);

            QuickSort(v, v2, left, last);
            QuickSort(v, v2, last + 1, right);
        }
        #endregion

        #region Swap2
        static void swap2(double[] v, int[] v2, int i, int j)
        {
            double tmp1;
            int tmp2;

            tmp1 = v[i];
            v[i] = v[j];
            v[j] = tmp1;
            tmp2 = v2[i];
            v2[i] = v2[j];
            v2[j] = tmp2;
        }
        #endregion

        #region BuildTrail2
        private static int[] BuildTrail2(int start, int citiesCount)
        {
            int n = citiesCount;

            // Karınca için yeni bir yol oluşturulur.
            int[] trail = new int[n + 1];
            // Ziyaret edilen şehirlerin tutulduğu dizi.
            bool[] visited = new bool[n];

            trail[0] = start;
            visited[start] = true;

            for (int i = 1; i < n; i++)
            {
                int city, current_city, next_city;

                next_city = n;
                current_city = trail[i - 1];
                double min_distance = double.MaxValue; /* Search shortest edge */
                for (city = 0; city < n; city++)
                {
                    if (visited[city])
                        continue;
                    else
                    {
                        double closestDistance = GetDistance(_vertices[current_city], _vertices[city]);
                        if (closestDistance < min_distance)
                        {
                            next_city = city;
                            min_distance = closestDistance;
                        }
                    }
                }
                trail[i] = next_city;
                visited[next_city] = true;
            }
            trail[n] = start;
            return trail;
        }
        #endregion

        #region two_opt_first
        static void two_opt_first(int[] tour, int[][] nn_list)
        {
            bool gotoExchange = false;

            int n = _vertices.Count;
            int c1, c2; /* cities considered for an exchange */
            int s_c1, s_c2; /* successor cities of c1 and c2 */
            int p_c1, p_c2; /* predecessor cities of c1 and c2 */
            int pos_c1, pos_c2; /* positions of cities c1, c2 */
            int i, j, h, l;
            int help;
            bool improvement_flag;
            int h1 = 0, h2 = 0, h3 = 0, h4 = 0;
            int radius; /* radius of nn-search */
            int gain = 0;
            int[] random_vector;
            int[] pos; /* positions of cities in tour */
            bool[] dlb; /* vector containing don't look bits */

            pos = new int[n];
            dlb = new bool[n];
            for (i = 0; i < n; i++)
            {
                pos[tour[i]] = i;
                dlb[i] = false;
            }

            improvement_flag = true;
            random_vector = generate_random_permutation(n);

            while (improvement_flag)
            {

                improvement_flag = false;

                for (l = 0; l < n; l++)
                {

                    c1 = random_vector[l];
                    // DEBUG ( assert ( c1 < Tsp.n && c1 >= 0); )
                    if (dlb_flag && dlb[c1])
                        continue;
                    pos_c1 = pos[c1];
                    s_c1 = tour[pos_c1 + 1];
                    radius = (int)GetDistance(_vertices[c1], _vertices[s_c1]);

                    /* First search for c1's nearest neighbours, use successor of c1 */
                    for (h = 0; h < 20; h++)
                    {
                        c2 = nn_list[c1][h]; /* exchange partner, determine its position */
                        if (radius > GetDistance(_vertices[c1], _vertices[c2]))
                        {
                            s_c2 = tour[pos[c2] + 1];
                            gain = (int)(-radius + GetDistance(_vertices[c1], _vertices[c2]) + GetDistance(_vertices[s_c1], _vertices[s_c2]) - GetDistance(_vertices[c2], _vertices[s_c2]));
                            if (gain < 0)
                            {
                                h1 = c1;
                                h2 = s_c1;
                                h3 = c2;
                                h4 = s_c2;
                                gotoExchange = true;
                                break;
                            }
                        }
                        else
                            break;
                    }

                    if (gotoExchange)
                    {
                        /* Search one for next c1's h-nearest neighbours, use predecessor c1 */
                        if (pos_c1 > 0)
                            p_c1 = tour[pos_c1 - 1];
                        else
                            p_c1 = tour[n - 1];
                        radius = (int)GetDistance(_vertices[p_c1], _vertices[c1]);
                        for (h = 0; h < 20; h++)
                        {
                            c2 = nn_list[c1][h]; /* exchange partner, determine its position */
                            if (radius > GetDistance(_vertices[c1], _vertices[c2]))
                            {
                                pos_c2 = pos[c2];
                                if (pos_c2 > 0)
                                    p_c2 = tour[pos_c2 - 1];
                                else
                                    p_c2 = tour[n - 1];
                                if (p_c2 == c1)
                                    continue;
                                if (p_c1 == c2)
                                    continue;
                                gain = (int)(-radius + GetDistance(_vertices[c1], _vertices[c2]) + GetDistance(_vertices[p_c1], _vertices[p_c2]) - GetDistance(_vertices[p_c2], _vertices[c2]));
                                if (gain < 0)
                                {
                                    h1 = p_c1;
                                    h2 = c1;
                                    h3 = p_c2;
                                    h4 = c2;
                                    gotoExchange = true;
                                    break;
                                }
                            }
                            else
                                break;
                        }
                    }

                    if (!gotoExchange)
                    {
                        /* No exchange */
                        dlb[c1] = true;
                        continue;
                    }

                    if (gotoExchange)
                    {
                        gotoExchange = false;
                        improvement_flag = true;
                        dlb[h1] = false;
                        dlb[h2] = false;
                        dlb[h3] = false;
                        dlb[h4] = false;
                        /* Now perform move */
                        if (pos[h3] < pos[h1])
                        {
                            help = h1;
                            h1 = h3;
                            h3 = help;
                            help = h2;
                            h2 = h4;
                            h4 = help;
                        }
                        if (pos[h3] - pos[h2] < n / 2 + 1)
                        {
                            /* reverse inner part from pos[h2] to pos[h3] */
                            i = pos[h2];
                            j = pos[h3];
                            while (i < j)
                            {
                                c1 = tour[i];
                                c2 = tour[j];
                                tour[i] = c2;
                                tour[j] = c1;
                                pos[c1] = j;
                                pos[c2] = i;
                                i++;
                                j--;
                            }
                        }
                        else
                        {
                            /* reverse outer part from pos[h4] to pos[h1] */
                            i = pos[h1];
                            j = pos[h4];
                            if (j > i)
                                help = n - (j - i) + 1;
                            else
                                help = (i - j) + 1;
                            help = help / 2;
                            for (h = 0; h < help; h++)
                            {
                                c1 = tour[i];
                                c2 = tour[j];
                                tour[i] = c2;
                                tour[j] = c1;
                                pos[c1] = j;
                                pos[c2] = i;
                                i--;
                                j++;
                                if (i < 0)
                                    i = n - 1;
                                if (j >= n)
                                    j = 0;
                            }
                            tour[n] = tour[0];
                        }
                    }
                    else
                    {
                        dlb[c1] = true;
                    }

                }
            }

        }
        #endregion

        #region generate_random_permutation
        static int[] generate_random_permutation(int n)
        {
            int i, help, node, tot_assigned = 0;
            double rnd;
            int[] r;

            r = new int[n];

            for (i = 0; i < n; i++)
                r[i] = i;

            for (i = 0; i < n; i++)
            {
                /* find (randomly) an index for a free unit */
                rnd = ran01(seed);
                node = (int)(rnd * (n - tot_assigned));
                help = r[i];
                r[i] = r[i + node];
                r[i + node] = help;
                tot_assigned++;
            }
            return r;
        }
        #endregion

        #region ran01
        static double ran01(long idum)
        {
            if (_random == null)
            {
                _random = new Random(seed);
            }

            return _random.NextDouble();
        }
        #endregion
    }
}