using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int larghezzaBordo = 40;
        static int altezzaBordo = 25;
        static int punteggio = 0;
        static List<Point> serpente = new List<Point>();
        static Point cibo;
        static Direction direzione = Direction.Destra;
        static Random random = new Random();
        static bool fineGioco = false;
        static string percorsoFilePunteggi = "punteggi.txt";
        static ConsoleColor coloreCibo;

        // Caratteri per la griglia di gioco
        static char bordoOrizzontale = '─';
        static char bordoVerticale = '│';
        static char angoloAltoSinistro = '┌';
        static char angoloAltoDestro = '┐';
        static char angoloBassoSinistro = '└';
        static char angoloBassoDestro = '┘';
        static char spazioVuoto = ' ';

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            while (true)
            {
                InizializzaGioco();
                while (!fineGioco)
                {
                    DisegnaGriglia();
                    OttieniInput();
                    MuoviSerpente();
                    Thread.Sleep(100);
                }
                GestisciFineGioco();
                Console.WriteLine("Vuoi ricominciare la partita? (S/N)");
                var tasto = Console.ReadKey(true).Key;
                if (tasto != ConsoleKey.S)
                {
                    break;
                }
            }
        }

        static void InizializzaGioco()
        {
            fineGioco = false;
            punteggio = 0;
            direzione = Direction.Destra;
            serpente.Clear();
            serpente.Add(new Point { X = larghezzaBordo / 2, Y = altezzaBordo / 2 });
            serpente.Add(new Point { X = larghezzaBordo / 2 - 1, Y = altezzaBordo / 2 });
            serpente.Add(new Point { X = larghezzaBordo / 2 - 2, Y = altezzaBordo / 2 });
            GeneraCibo();
        }

        static void DisegnaGriglia()
        {
            Console.Clear();
            for (int y = 0; y <= altezzaBordo; y++)
            {
                for (int x = 0; x <= larghezzaBordo; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        Console.Write(angoloAltoSinistro);
                    }
                    else if (x == larghezzaBordo && y == 0)
                    {
                        Console.Write(angoloAltoDestro);
                    }
                    else if (x == 0 && y == altezzaBordo)
                    {
                        Console.Write(angoloBassoSinistro);
                    }
                    else if (x == larghezzaBordo && y == altezzaBordo)
                    {
                        Console.Write(angoloBassoDestro);
                    }
                    else if (x == 0 || x == larghezzaBordo)
                    {
                        Console.Write(bordoVerticale);
                    }
                    else if (y == 0 || y == altezzaBordo)
                    {
                        Console.Write(bordoOrizzontale);
                    }
                    else if (serpente.Any(p => p.X == x && p.Y == y))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("O");
                        Console.ResetColor();
                    }
                    else if (cibo.X == x && cibo.Y == y)
                    {
                        Console.ForegroundColor = coloreCibo;
                        Console.Write("■");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(spazioVuoto);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Punteggio: " + punteggio);
        }

        static void OttieniInput()
        {
            if (Console.KeyAvailable)
            {
                var tasto = Console.ReadKey(true).Key;
                switch (tasto)
                {
                    case ConsoleKey.UpArrow:
                        if (direzione != Direction.Giù) direzione = Direction.Su;
                        break;
                    case ConsoleKey.DownArrow:
                        if (direzione != Direction.Su) direzione = Direction.Giù;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direzione != Direction.Destra) direzione = Direction.Sinistra;
                        break;
                    case ConsoleKey.RightArrow:
                        if (direzione != Direction.Sinistra) direzione = Direction.Destra;
                        break;
                }
            }
        }

        static void MuoviSerpente()
        {
            Point testa = serpente.First();
            Point nuovaTesta = new Point { X = testa.X, Y = testa.Y };

            switch (direzione)
            {
                case Direction.Su:
                    nuovaTesta.Y -= 1;
                    break;
                case Direction.Giù:
                    nuovaTesta.Y += 1;
                    break;
                case Direction.Sinistra:
                    nuovaTesta.X -= 1;
                    break;
                case Direction.Destra:
                    nuovaTesta.X += 1;
                    break;
            }

            if (nuovaTesta.X == 0 || nuovaTesta.Y == 0 || nuovaTesta.X == larghezzaBordo || nuovaTesta.Y == altezzaBordo || serpente.Any(p => p.X == nuovaTesta.X && p.Y == nuovaTesta.Y))
            {
                fineGioco = true;
                return;
            }

            serpente.Insert(0, nuovaTesta);
            if (nuovaTesta.X == cibo.X && nuovaTesta.Y == cibo.Y)
            {
                punteggio++;
                GeneraCibo();
            }
            else
            {
                serpente.RemoveAt(serpente.Count - 1);
            }
        }

        static void GeneraCibo()
        {
            int x, y;
            do
            {
                x = random.Next(1, larghezzaBordo);
                y = random.Next(1, altezzaBordo);
            } while (serpente.Any(p => p.X == x && p.Y == y));
            cibo = new Point { X = x, Y = y };
            coloreCibo = OttieniColoreCiboCasuale();
        }

        static ConsoleColor OttieniColoreCiboCasuale()
        {
            var colori = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue, ConsoleColor.DarkYellow };
            return colori[random.Next(colori.Length)];
        }

        static void GestisciFineGioco()
        {
            Console.Clear();
            Console.WriteLine("Fine del gioco! Punteggio finale: " + punteggio);

            string iniziali = "";
            while (iniziali.Length != 3)
            {
                Console.Write("Inserisci le tue iniziali (3 lettere): ");
                iniziali = Console.ReadLine().ToUpper();
                if (iniziali.Length != 3)
                {
                    Console.WriteLine("Input non valido. Per favore inserisci esattamente 3 lettere.");
                }
            }

            SalvaPunteggio(iniziali, punteggio);

            Console.Clear();
            Console.WriteLine("Fine del gioco! Punteggio finale: " + punteggio);
            Console.WriteLine("Le tue iniziali: " + iniziali);
            Console.WriteLine("\nPunteggi migliori:");
            MostraPunteggi();
        }

        static void SalvaPunteggio(string iniziali, int punteggio)
        {
            using (StreamWriter writer = new StreamWriter(percorsoFilePunteggi, true))
            {
                writer.WriteLine($"{iniziali}: {punteggio}");
            }
        }

        static void MostraPunteggi()
        {
            if (File.Exists(percorsoFilePunteggi))
            {
                var punteggi = File.ReadAllLines(percorsoFilePunteggi)
                                .Select(linea => linea.Split(':'))
                                .Select(parti => new { Iniziali = parti[0].Trim(), Punteggio = int.Parse(parti[1].Trim()) })
                                .OrderByDescending(ingresso => ingresso.Punteggio)
                                .Take(10);

                foreach (var ingresso in punteggi)
                {
                    Console.WriteLine($"{ingresso.Iniziali}: {ingresso.Punteggio}");
                }
            }
            else
            {
                Console.WriteLine("Nessun punteggio disponibile.");
            }
        }

        enum Direction
        {
            Su,
            Giù,
            Sinistra,
            Destra
        }

        class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
