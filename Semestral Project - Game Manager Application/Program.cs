using System.Xml.Linq;
using NAudio.Wave;

namespace GameManager
{
    class Program
    {
        //Declare and initialize the objects used by the NAudio package to control the music. 
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;

        //Function used to manage the background music.
        static void PlayMusic(string filepath, float volume)
        {
            //Stops the current music.
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                audioFile.Dispose();
            }

            audioFile = new AudioFileReader(filepath)
            {
                Volume = volume
            };
            
            //Starts the music. 
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }

        //Function used to manage the Sound Effects.
        static void SoundFX(string filepath, float volume)
        {
            using var sfx = new AudioFileReader(filepath);
            using var outputDevice2 = new WaveOutEvent();
            
            sfx.Volume = volume;
            outputDevice2.Init(sfx);
            outputDevice2.Play();
            if (outputDevice2.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(700); //Gives enough time for the sound effect to play.
            }
        }

        //Function used to create the database.
        static void CreateDatabase(string path)
        {
            XDocument doc = new(new XElement("GameList"));
            doc.Save(path);

            string databasePath = "xmlDoc.xml";
            if (!File.Exists(databasePath)) //If the XML file does not exist, this error message pops up.
            {
                Console.WriteLine("games.xml file could not be found." +
                                  "\ncreate file games.xml and execute the program again.");
                SoundFX("Error.wav", 1.0f);
                return;
            }
        }

        //Function used to add games to the XML file.
        static void AddGame(string path)
        {
            Console.Clear();
            Console.Write("Enter the name of the game: ");
            string name = Console.ReadLine();

            Console.Write("Enter the genre of the game: ");
            string genre = Console.ReadLine();

            //If successfull, adds the game to the list using the following structure.
            Console.Write("Enter game price: ");
            if (float.TryParse(Console.ReadLine(), out float price))
            {
                XDocument doc = XDocument.Load(path);
                XElement newGame = new ("Game",
                                   new XElement("Name", name),
                                   new XElement("Genre", genre),
                                   new XElement("Price", price));
                doc.Root.Add(newGame);
                doc.Save(path);
                Console.WriteLine("\n" +"Game added successfully.");
                SoundFX("Success.wav", 1.0f);
            }
            else
            {
                Console.WriteLine("\n" + "Invalid number format."); //Sends error if failed. 
                SoundFX("Error.wav", 1.0f);
            }
        }

        //Function used to remove games from the XML file.
        static void RemoveGame(string path)
        {
            Console.Clear();
            Console.Write("To erase, please type the name of the game: ");
            string name = Console.ReadLine();

            //Loads the node (Game) of XML tree and checks if the name of the game is typed correctly. 
            XDocument doc = XDocument.Load(path);
            XElement gameDeletion = doc.Root.Elements("Game") .FirstOrDefault(g => g.Element("Name").Value == name); 
            if (gameDeletion != null)
            {
                gameDeletion.Remove();
                doc.Save(path); 
                Console.WriteLine("\n" + "Game erased successfully.");
                SoundFX("Success.wav", 1.0f);
            }
            else
            {
                Console.WriteLine("\n" + "Sorry, game not found.");
                SoundFX("Error.wav", 1.0f);
            }
        }

        //Function used to show all stored games.
        static void DisplayGames(string path)
        {
            //Loads the node (Game) of XML tree and displays information in the following order.
            Console.Clear();   
            XDocument doc = XDocument.Load(path);
            foreach (XElement game in doc.Root.Elements("Game")) 
            {
                Console.WriteLine($"\nName: {game.Element("Name").Value}" +
                                  $"\nGenre: {game.Element("Genre").Value}" +
                                  $"\nPrice: {game.Element("Price").Value}"); 
            }
        }

        //Function used to show the most expensive game.
        static void DisplayExpensive(string path)
        {
            //Loads the node and displays the game and its price.  
            Console.Clear();
            XDocument doc = XDocument.Load(path);
            XElement expensive = doc.Root.Elements("Game") .OrderByDescending(e => (float) e.Element("Price")) .FirstOrDefault(); 
            if (expensive != null)
            {
                Console.WriteLine($"Most expensive game: {expensive.Element("Name").Value}" +
                                  $"\nPrice: {expensive.Element("Price").Value}"); 
            }
            else
            {
                Console.WriteLine("Sorry, there are no saved games."); //pops up if database is empty.
                SoundFX("Error.wav", 1.0f);
            }
        }

        //Function used to show the cheapest game.
        static void DisplayCheap(string path)
        {
            //Loads the node and displays the game and its price.
            Console.Clear();
            XDocument doc = XDocument.Load(path);
            XElement cheap = doc.Root.Elements("Game") .OrderBy(c => (float) c.Element("Price")) .FirstOrDefault(); 
            if (cheap != null)
            {
                Console.WriteLine($"Cheapest game: {cheap.Element("Name").Value}" +
                                  $"\nPrice: {cheap.Element("Price").Value}"); 
            }
            else
            {
                Console.WriteLine("Sorry, there are no saved games."); //pops up if database is empty.
                SoundFX("Error.wav", 1.0f);
            }
        }
        
        //The Main Function Which contains the main menu.
        static void Main()
        {
            string databasePath = "games.xml";
            
            //If file does not exist, calls function that creates the database to build an empty XML.
            if (!File.Exists(databasePath))
            {
                CreateDatabase(databasePath);
            }

            bool exit = false;
            
            PlayMusic("Menu 2.wav", 0.5f);
            
            while (!exit) //Falsifies menu exit loop condition.
            {
                Console.ForegroundColor = ConsoleColor.Green; //Changes text color to Green
                Console.Write("Main Menu:" +
                              "\n" +
                              "\n1. Add game" +
                              "\n2. Remove game" +
                              "\n3. All games" +
                              "\n4. Most expensive game" +
                              "\n5. Cheapest game" +
                              "\n6. Exit" +
                              "\n" +
                              "\nChoose point: ");
                
                //Second argument is the music volume which varies from 0.0f (min) to 1.0f (max)
                SoundFX("Back.wav", 1.0f);

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            SoundFX("Select.wav",1.0f);
                            AddGame(databasePath);
                            Console.WriteLine("\n"+ "Press any key to go back..." + "\n");
                            Console.ReadKey();
                            break;
                        case 2:
                            SoundFX("Select.wav", 1.0f);
                            RemoveGame(databasePath);
                            Console.WriteLine("\n" + "Press any key to go back..." + "\n");
                            Console.ReadKey();
                            break;
                        case 3:
                            SoundFX("Select.wav", 1.0f);
                            DisplayGames(databasePath);
                            Console.WriteLine("\n" + "Press any key to go back..." + "\n");
                            Console.ReadKey();
                            break;
                        case 4:
                            SoundFX("Select.wav", 1.0f);
                            DisplayExpensive(databasePath);
                            Console.WriteLine("\n" + "Press any key to go back..." + "\n");
                            Console.ReadKey();
                            break;
                        case 5:
                            SoundFX("Select.wav", 1.0f);
                            DisplayCheap(databasePath);
                            Console.WriteLine("\n" + "Press any key to go back..." + "\n");
                            Console.ReadKey();
                            break;
                        case 6:
                            exit = true; //Makes true the exit loop condition and sends a goodbye message.
                            Console.Clear();
                            Console.WriteLine("See you next time, take care user.");
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("User you must choose from 1 to 6." + "\n");
                            SoundFX("Message.wav", 1.0f);
                            break;
                    }
                }
                else //Pops up if user types anything that is not a number.
                {
                    Console.Clear();
                    Console.WriteLine("Wrong key, you must use numbers" + "\n");
                    SoundFX("Message.wav", 1.0f);
                }
            }
        }
    }
}