using TravellingSalesman.Model;

namespace TravellingSalesman.algorithms
{
    public class BruteForceTSP
    {
        private List<Vertex> _vertices;
        private List<int[]> paths = new List<int[]>();
        private List<double> pathLengths = new List<double>();

        public BruteForceTSP(List<Vertex> vertices)
        {
            _vertices = vertices;
        }

        /// <summary>
        /// Olası tüm rotaları dolaşır ve en iyi rotayı bulur.
        /// </summary>
        /// <returns>En iyi rotayı döndürür.</returns>
        public int[] BruteForceTSPAlgorithm()
        {
            // En iyi rotayı tutmak için bir dizi oluştur.
            int[] bestTour = new int[_vertices.Count];
            double bestTourLength = double.PositiveInfinity;
            
            // Rotaları tutmak için bir dizi oluştur.
            int[] tour = new int[_vertices.Count];
            for (int i = 0; i < tour.Length; i++)
            {
                tour[i] = i;
            }

            // En iyi rotayı bulana kadar döngüyü çalıştır.
            do
            {
                paths.Add(tour.Clone() as int[]);
                double tourLength = GetTourLength(_vertices, tour);
                pathLengths.Add(tourLength);
                
                if (tourLength < bestTourLength)
                {
                    bestTourLength = tourLength;
                    bestTour = (int[])tour.Clone();
                }

            } while (NextPermutation(tour));

            return bestTour;
        }
        
        /// <summary>
        /// Rotaların uzunluklarını hesaplar.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="tour"></param>
        /// <returns>Rotaların uzunluklarını döndürür.</returns>
        public double GetTourLength(List<Vertex> vertices, int[] tour)
        {
            double tourLength = 0.0;

            // Rotaları dolaş ve uzunlukları hesapla.
            for (int i = 0; i < tour.Length - 1; i++)
            {
                Vertex fromVertex = vertices[tour[i]];
                Vertex toVertex = vertices[tour[i + 1]];
                tourLength += Math.Sqrt(Math.Pow(fromVertex.X - toVertex.X, 2) + Math.Pow(fromVertex.Y - toVertex.Y, 2));
            }

            return tourLength;
        }

        /// <summary>
        /// Bir sonraki permutasyonu bulur.
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Sonraki permütasyonu döndürür.</returns>
        private bool NextPermutation(int[] array)
        {
            // Bir sonraki permütasyonu bulmak için diziyi sondan başa doğru dolaş.
            int i = array.Length - 2;
            while (i >= 0 && array[i] >= array[i + 1])
            {
                i--;
            }

            // Dizinin sonuna gelindiğinde false döndür.
            if (i < 0)
            {
                return false;
            }
            
            // i indexindeki elemandan büyük olan en son elemanı bul.
            int j = array.Length - 1;
            while (array[j] <= array[i])
            {
                j--;
            }

            // Bulunan elemanları yer değiştir.
            Swap(array, i, j);

            
            // i indexinden sonraki elemanları ters çevir.
            int start = i + 1;
            int end = array.Length - 1;
            while (start < end)
            {
                Swap(array, start, end);
                start++;
                end--;
            }

            return true;
        }

        /// <summary>
        /// Dizideki i ve j indexindeki elemanları yer değiştirir.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        /// <summary>
        /// Tüm rotaları döndürür.
        /// </summary>
        /// <returns>Tüm rotalar</returns>
        public List<int[]> GetPaths()
        {
            return paths;
        }

        /// <summary>
        /// Tüm rotaların uzunluklarını döndürür.
        /// </summary>
        /// <returns>Rota uzunlukları</returns>
        public List<double> GetPathLengths()
        {
            return pathLengths;
        }
    }
}