using System;
using System.IO;
using System.Net;
using System.Text;

namespace APIProxyAuth
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string cmd = args[0];
                switch (cmd)
                {
                    case "-help":
                        Program.help(args);
                        break;
                    case "-post":
                        Program.post(args);
                        break;
                    case "-get":
                        Program.get(args);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Aucun argument.");
                Environment.Exit(1);
            }
        }

        static void help(string[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine(@"    _    ____ ___ ____                         _         _   _     ");
                Console.WriteLine(@"   / \  |  _ \_ _|  _ \ _ __ _____  ___   _   / \  _   _| |_| |___ ");
                Console.WriteLine(@"  / _ \ | |_) | || |_) | '__/ _ \ \/ / | | | / _ \| | | | __| '_  \");
                Console.WriteLine(@" / ___ \|  __/| ||  __/| | | (_) >  <| |_| |/ ___ \ |_| | |_| | | |");
                Console.WriteLine(@"/_/   \_\_|  |___|_|   |_|  \___/_/\_\\__, /_/   \_\__,_|\__|_| |_|");
                Console.WriteLine(@"                                      |___/                        ");
                Console.WriteLine(@"===================================================================");
                Console.WriteLine(@"                          ~ APIProxyAuth ~                         ");
                Console.WriteLine(@"                    ~ Fait par BUSTOS Thibault ~                   ");
                Console.WriteLine(@"                  ~ Version 1.0.0.0 (19/12/2021) ~                 ");
                Console.WriteLine(@"===================================================================");
                Console.WriteLine(@"                                                                   ");
                Console.WriteLine(@"Permet d'envoyer des requetes HTTP(S) avec l'authentification proxy");
                Console.WriteLine(@"par defaut de Windows.   ");
                Console.WriteLine(@"                                                                   ");
                Console.WriteLine(@"Documentation :                                                    ");
                Console.WriteLine(@"---------------                                                    ");
                Console.WriteLine(@"                                                                   ");
                Console.WriteLine(@" -help                       Affiche la documentation.             ");
                Console.WriteLine(@" -get [prm] [url]            Envoi des donnees en methode GET.     ");
                Console.WriteLine(@" -post [prm] [url] [args]    Envoi des donnees en methode POST.    ");
                Console.WriteLine(@"                                                                   ");
                Console.WriteLine(@"Parametres autorises :                                             ");
                Console.WriteLine(@"----------------------                                             ");
                Console.WriteLine(@"                                                                   ");
                Console.WriteLine(@" -string                     Traite une chaine de caracteres.      ");
                Console.WriteLine(@" -file                       Traite un fichier.                    ");
                Console.WriteLine(@" -data                       Traite des donnees.                   ");
                Environment.Exit(0);
            }
            else
            {
                Program.bad();
            }
        }

        static void post(string[] args)
        {
            if (args.Length == 4)
            {
                try
                {
                    string prms = args[1];
                    string url = args[2];
                    string arg = args[3];

                    using (WebClient wc = Program.proxy())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Encoding = Encoding.UTF8;

                        switch (prms)
                        {
                            case "-string":
                                string response = wc.UploadString(url, arg);
                                Console.WriteLine(response);
                                break;
                            case "-file":
                                byte[] body = wc.UploadFile(url, arg);
                                File.WriteAllBytes("post.bin", body);
                                break;
                            case "-data":
                                byte[] data = File.ReadAllBytes(arg);
                                byte[] body2 = wc.UploadData(url, data);
                                File.WriteAllBytes("post.bin", body2);
                                break;
                            default:
                                Program.bad();
                                break;
                        }
                    }
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    Program.error(e);
                }
            }
            else
            {
                Program.bad();
            }
        }

        static void get(string[] args)
        {
            if (args.Length == 3)
            {
                try
                {
                    string prms = args[1];
                    string url = args[2];
                    using (WebClient wc = Program.proxy())
                    {
                        switch (prms)
                        {
                            case "-string":
                                string response = wc.DownloadString(url);
                                Console.WriteLine(response);
                                break;
                            case "-file":
                                wc.DownloadFile(url, "get.json");
                                break;
                            case "-data":
                                byte[] data = wc.DownloadData(url);
                                File.WriteAllBytes("get.bin", data);
                                break;
                            default:
                                Program.bad();
                                break;
                        }
                    }
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    Program.error(e);
                }
            }
            else
            {
                Program.bad();
            }
        }

        static void error(Exception e)
        {
            Console.WriteLine("Une erreur est apparue, message : " + e.Message);
            Environment.Exit(3);
        }

        static void bad()
        {
            Console.WriteLine("Mauvais arguments, essayez -help.");
            Environment.Exit(2);
        }

        static WebClient proxy()
        {
            IWebProxy prox = WebRequest.DefaultWebProxy;
            prox.Credentials = CredentialCache.DefaultCredentials;
            return new WebClient { Proxy = prox };
        }
    }
}
