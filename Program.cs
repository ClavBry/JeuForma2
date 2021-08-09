using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace JeuForma2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Déclaration des Constantes
            const string KirbySaute = @"^(°o°)^";
            const string KirbyTombe = @"v(o_o)v";
            const float Gravite = .9f;
            const int LargeurTuyeau = 8;
            const int TrouTuyeau = 7;
            const int EspaceEntreLesTuyeaux = 45;

            //Décalaration des variables 
            int TailleLargeurJeu = Console.WindowWidth;
            int TailleHauteurJeu = Console.WindowHeight;

            TimeSpan Timer = TimeSpan.FromMilliseconds(90);
            Random Random = new();
            List<(int X, int TrouY)> Tuyeaux = new();

            int Largeur, Hauteur, Frame, FrameTuyeau;
            float KirbyX, KirbyY, KirbyDY;

            AfficherEcranDem();

            void Jeu()
            {


                //Clear de la console 
                //Initialisation de la fenêtre de jeu en fonction du system
                //Utilisation d'étiquettes pour lancer des actions via un goto



                Console.Clear();
                Tuyeaux.Clear();
                if (OperatingSystem.IsWindows())
                {
                    Largeur = Console.WindowWidth = 120;
                    Hauteur = Console.WindowHeight = 30;
                }
                else
                {
                    Largeur = Console.WindowWidth;
                    Hauteur = Console.WindowHeight;
                }
                KirbyX = Largeur / 6;
                KirbyY = Hauteur / 2;
                KirbyDY = 0;
                Frame = 0;
                FrameTuyeau = EspaceEntreLesTuyeaux;
                Console.CursorVisible = false;
                //Attente du début du jeu 
                AfficherKirby();
                Console.SetCursorPosition((int)KirbyX - 10, (int)KirbyY + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Appuie Espace pour Jouer");

            //Lecture des touche entrée par l'utilisateur pour commencer le jeu
            LireEntree:
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Spacebar:
                        KirbyDY = -2;
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        Console.Write("Kirby flap à été fermé");
                        return;
                    default:
                        goto LireEntree;
                }
                Console.SetCursorPosition((int)KirbyX - 10, (int)KirbyY + 1);
                Console.Write("                          ");

                //Corp du prog boucle pendant toute la partie
                while (true)
                {

                    //Vérification du cas de la modification de la taille de la fenêtre 
                    if (Console.WindowHeight != Hauteur || Console.WindowWidth != Largeur)
                    {
                        Console.Clear();
                        Console.Write("Vous avez modifier la taille de la console. Kirby Flap à été fermé !!");
                        AfficherEcranDem();
                    }
                    //Vérification si on arrive à la fin 
                    if (Frame == int.MaxValue)
                    {
                        Console.SetCursorPosition(0, Hauteur - 1);
                        Console.Write("Bravo, tu as gagné ton score est de:" + Frame + ".");
                        AfficherEcranDem();
                    }
                    //Vérification si Kirby se tape un tuyeau
                    if (!(KirbyY < Hauteur - 1 && KirbyY > 0) || IsKirbyLaFaceDansUnTuyeau())
                    {
                        Console.SetCursorPosition(0, Hauteur - 1);
                        Console.Write("Perdu ! Ton score est de : " + Frame + ".");
                        Console.Write("Appuie sur <Enter pour rejouer> ou <ESC> pour revenir au menu de démarrage");


                    //Vérifier si le joueur veut rejouer ou pas 
                    GetRejouer:
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if (key is ConsoleKey.Enter)
                        {
                            Jeu();
                        }
                        else if (!(key is ConsoleKey.Escape))
                        {
                            goto GetRejouer;
                        }
                        Console.Clear();
                        AfficherEcranDem();
                    }
                    //Mise à jour affichage des tuyeaux et Kirby
                    {
                        //Management des Tuyeaux
                        {
                            //Effacer les éléments afficher
                            foreach (var (X, TrouY) in Tuyeaux)
                            {
                                int x = X + LargeurTuyeau / 2;
                                if (x >= 0 && x < Largeur)
                                {
                                    for (int y = 0; y < Hauteur; y++)
                                    {
                                        Console.SetCursorPosition(x, y);
                                        Console.Write(' ');
                                    }
                                }
                            }
                            //Mettre à jour affichage
                            for (int i = 0; i < Tuyeaux.Count; i++)
                            {
                                Tuyeaux[i] = (Tuyeaux[i].X - 1, Tuyeaux[i].TrouY);
                            }
                            //Gestion de la supression du tuyeau
                            if (Tuyeaux.Count > 0 && Tuyeaux[0].X < -LargeurTuyeau)
                            {
                                Tuyeaux.RemoveAt(0);
                            }
                            //Initialisation d'un nouveau tuyeau
                            if (FrameTuyeau >= EspaceEntreLesTuyeaux)
                            {
                                int trouY = Random.Next(0, Hauteur - TrouTuyeau - 1 - 6) + 3;
                                Tuyeaux.Add((Largeur + LargeurTuyeau / 2, trouY));
                                FrameTuyeau = 0;
                            }
                            //Afficher (image actuelle)
                            foreach (var (X, trouY) in Tuyeaux)
                            {
                                Console.SetCursorPosition(0, 0);
                                int score = Frame + 1;
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("Score :" + score);
                                Console.ForegroundColor = ConsoleColor.Green;
                                int x = X - LargeurTuyeau / 2;
                                for (int y = 0; y < Hauteur; y++)
                                {
                                    if (x > 0 && x < Largeur - 1 && (y < trouY || y > trouY + TrouTuyeau))
                                    {
                                        Console.SetCursorPosition(x, y);

                                        Console.Write('█');
                                    }
                                }
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            AfficherKirby();
                            FrameTuyeau++;
                        }
                        //Management de Kirby
                        {
                            //Effacer (image précédente)
                            {
                                bool velociteVerticalr = KirbyDY < 0;
                                Console.SetCursorPosition((int)(KirbyX) - 3, (int)KirbyY);
                                Console.Write("       ");

                            }
                            //Mettre à jour affichage
                            while (Console.KeyAvailable)
                            {
                                switch (Console.ReadKey(true).Key)
                                {
                                    case ConsoleKey.Spacebar:
                                        KirbyDY = -2;
                                        break;
                                    case ConsoleKey.Escape:
                                        Console.Clear();
                                        Console.Write("Kirby Flap à été fermé");
                                        return;
                                }

                            }
                            KirbyY += KirbyDY;
                            KirbyDY += Gravite;
                            //Afficher image actuelle
                            AfficherKirby();

                        }
                        Frame++;
                    }
                    Thread.Sleep(Timer);
                }
            }



            //Fonction pour vérifier pour tout les tuyeaux en cours si Kirby touche des tuyeau ou pas 
            //en fonction de l'emplacement du trou du tuyeau
            //
            bool IsKirbyLaFaceDansUnTuyeau()
            {
                foreach (var (X, trouY) in Tuyeaux)
                {
                    if (Math.Abs(X - KirbyX) < LargeurTuyeau / 2 + 5 && ((int)KirbyY < trouY || (int)KirbyY > trouY + TrouTuyeau))
                    {
                        return true;
                    }
                }
                return false;
            }


            //Fonction pour afficher la position de Kirby affiche différement kirby en fonction de si il tombe 
            //Ou si il remonte
            void AfficherKirby()
            {
                if ((int)KirbyY < Hauteur - 1 && (int)KirbyY >= 0)
                {
                    bool velociteverticale = KirbyDY < 0;
                    Console.SetCursorPosition((int)KirbyX - 3, (int)KirbyY);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(velociteverticale ? KirbySaute : KirbyTombe);
                }
            }

            void AfficherEcranDem()
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("             Clavie Bryan Jeu pour formation TechnofuturTic 2021 \n\n\n");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(
@"               _   ___      _            ______ _             " + "\n" +
@"              | | / (_)    | |           |  ___| |            " + "\n" +
@"              | |/ / _ _ __| |__  _   _  | |_  | | __ _ _ __  " + "\n" +
@"              |    \| | '__| '_ \| | | | |  _| | |/ _` | '_ \ " + "\n" +
@"              | |\  \ | |  | |_) | |_| | | |   | | (_| | |_) |" + "\n" +
@"              \_| \_/_|_|  |_.__/ \__, | \_|   |_|\__,_| .__/ " + "\n" +
@"                                   __/ |               | |    " + "\n" +
@"                                  |___/                |_|    " + " \n\n\n\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
"               Commande : Espace pour faire voler Kirby ! \n              Ne pas redimensionner la console pendant le jeu !!!\n                    Appuie sur Enter pour commencer!");


                //Vérification du choix utilisateur 
                ConsoleKey k = Console.ReadKey(true).Key;
                if (k == ConsoleKey.Enter)
                {
                    Jeu();
                }
                if (k == ConsoleKey.Escape)
                {
                    Console.Clear();
                    Console.Write("A bientôt");
                    Environment.Exit(0);
                }
                else AfficherEcranDem();
            }
        }

    }
}
