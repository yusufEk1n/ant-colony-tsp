using System.Diagnostics;
using TravellingSalesman.algorithms;
using TravellingSalesman.Model;

namespace TravellingSalesman
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                //Menu yazıdrılır
                PrintMenu();

                //Kullanıcıdan seçim yapması istenir. Seçim doğruysa döngüden çıkılır.
                if (CheckSelection())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Dosyadan vertex'leri okur.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Okunan Vertex'lerin listesi</returns>
        #region ReadFile
        public static List<Vertex> ReadFile(string filePath)
        {
            List<Vertex> vertices = new List<Vertex>(); // vertex'leri tutmak için bir liste oluştur

            try
            {
                // Dosyadaki her bir satırı oku.
                foreach (string line in File.ReadAllLines(filePath))
                {
                    if (line.Split(' ').Length != 2)
                    {
                        continue;
                    }
                    string[] coords = line.Split(' ');

                    // Her bir satırdaki koordinatları al.
                    double x = Convert.ToDouble(coords[0].Replace(".", ","));
                    double y = Convert.ToDouble(coords[1].Replace(".", ","));

                    // Vertex nesnesi oluştur ve listeye ekle.
                    Vertex vertex = new Vertex(x, y);
                    vertices.Add(vertex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dosya okuma hatası: " + ex.Message);
            }
            return vertices;
        }
        #endregion
        
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Bruteforce algoritmasını çalıştırır.
        /// </summary>
        /// <param name="vertices"></param>
        #region BruteForce
        public static void BruteForce(List<Vertex> vertices)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            // Brute Force TSP algoritmasını çalıştırmak için BruteForceTSP sınıfından bir nesne oluştur.
            BruteForceTSP bruteForceTSP = new BruteForceTSP(vertices);

            sw.Stop();

            Console.WriteLine("Geçen süre (ms): {0}", sw.Elapsed.TotalMilliseconds);

            // Algoritma sonucunu ekrana yazdır.
            var bestTour = bruteForceTSP.BruteForceTSPAlgorithm();

            Console.WriteLine();
            Console.Write("En iyi yol: ");
            Console.WriteLine(String.Join("->", bestTour));
            Console.WriteLine("En iyi yol uzunluğu: " + bruteForceTSP.GetTourLength(vertices, bestTour));
            Console.WriteLine();

            Console.Write("Algoritma hakkında daha fazla bilgi almak ister misiniz? (y/Y): ");
            string selection = Console.ReadLine();

            if (selection == "y" || selection == "Y")
            {
                Console.WriteLine();
                BruteForceMoreInfo(bruteForceTSP);
            }

            Console.WriteLine();
            Console.WriteLine("Brute Force algoritması sonlandı.");
        }
        #endregion

        /// <summary>
        /// Bruteforce algoritmasının sonucunu daha fazla bilgi için ekrana yazdırır.
        /// </summary>
        /// <param name="bruteForceTSP"></param>
        #region BruteForceMoreInfo
        public static void BruteForceMoreInfo(BruteForceTSP bruteForceTSP)
        {
            var paths = bruteForceTSP.GetPaths();
            var pathLenghts = bruteForceTSP.GetPathLengths();

            for (int i = 0; i < paths.Count; i++)
            {
                Console.WriteLine($"Route {i} : " + String.Join("->", paths[i]) + " |" + " Lenght: " + pathLenghts[i]);
            }
        }
        #endregion

        // ----------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Antcolony algoritmasını çalıştırır.
        /// </summary>
        /// <param name="vertices"></param>
        #region AntColony
        private static void AntColony(List<Vertex> vertices)
        {
            AntColony antColony = new AntColony(vertices);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            antColony.AntColonyAlgorithm();

            sw.Stop();

            Console.WriteLine("Geçen süre (ms): {0}", sw.Elapsed.TotalMilliseconds);
        }
        #endregion
        
        /// <summary>
        /// Menüyü ekrana yazdırır.
        /// </summary>
        #region PrintMenu
        private static void PrintMenu()
        {
            Console.WriteLine("Lütfen bir seçim yapınız:");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("tsp_5_1 için 1'e basınız.");
            Console.WriteLine("ts_124_1 için 2'ye basınız.");
            Console.WriteLine("tsp_1000_1 için 3'e basınız.");
            Console.WriteLine("tsp_5915_1 için 4'e basınız.");
            Console.WriteLine("tsp_11849_1 için 5'e basınız.");
            Console.WriteLine();
            Console.Write("Seçiminiz: ");
        }
        #endregion

        /// <summary>
        /// Kullanıcının seçimini kontrol eder.
        /// </summary>
        /// <returns>Seçim doğruysa true, yanlışsa false döner.</returns>
        #region CheckSelection
        private static bool CheckSelection()
        {
            bool isSelectionValid = false;

            if (int.TryParse(Console.ReadLine(), out int selection))
            {
                switch (selection)
                {
                    case 1:
                        var tsp_5_Matrix = ReadFile("./dataset/tsp_5_1");
                        BruteForce(tsp_5_Matrix);
                        isSelectionValid = true;
                        break;
                    case 2:
                        var tsp_124_Matrix = ReadFile("./dataset/tsp_124_1");
                        AntColony(tsp_124_Matrix);
                        isSelectionValid = true;
                        break;
                    case 3:
                        var tsp_1000_Matrix = ReadFile("./dataset/tsp_1000_1");
                        AntColony(tsp_1000_Matrix);
                        isSelectionValid = true;
                        break;
                    case 4:
                        var tsp_5915_Matrix = ReadFile("./dataset/tsp_5915_1");
                        AntColony(tsp_5915_Matrix);
                        isSelectionValid = true;
                        break;
                    case 5:
                        var tsp_11849_Matrix = ReadFile("./dataset/tsp_11849_1");
                        AntColony(tsp_11849_Matrix);
                        isSelectionValid = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Hatalı bir seçim yaptınız. Lütfen tekrar deneyin.\n");
                        break;
                }
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Hatalı bir seçim yaptınız. Lütfen tekrar deneyin.\n");
            }

            return isSelectionValid;
        }
        #endregion
    }
}