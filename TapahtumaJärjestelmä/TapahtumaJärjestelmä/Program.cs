using System.Runtime.InteropServices;

namespace TapahtumaJärjestelmä
{

    public class Participant
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
            public Participant(string Name)
            {
                Name = Name?.Trim() ?? "";
                //Email = Email?.Trim() ?? "";
                //Phone = Phone?.Trim() ?? "";
            }
        public override string ToString() => $"{Name} | {Email} | {Phone}";
    
    }
    public class Event
    {
        private const int EventCap = 50;

        // ilmottautumiset
        private readonly List<Participant> registations = new();
        private readonly HashSet<string> registeredNames = new(StringComparer.OrdinalIgnoreCase);

        // kirjautumiset saapuneet
        private readonly HashSet<string> checkIns = new(StringComparer.OrdinalIgnoreCase);

        //Puheenvuorojono
        private readonly Queue<string> speakingQueue = new();
        private readonly HashSet<string> inQueue = new(StringComparer.OrdinalIgnoreCase);

        public bool AddRegistration(Participant p, out string message)
        {
            // Tarkistaa onko määrä täynnä
            if(registations.Count >= EventCap)
            {
                message = "Tapahtuma on täynnä (50 Paikkaa.)";
                return false;
            }
            // Tarkistaa nimen
            if(registeredNames.Contains(p.Name))
            {
                message = $"Nimi {p.Name} on jo ilmottautunut.";
                return false;
            }

            // säilyttää ilmottautumisjärjestyksen
            registations.Add(p);
            registeredNames.Add(p.Name);
            message = "Ilmottautuminen onnistui.";
            return true;
        }
        public bool RemoveRegistration(string name, out string message)
        {
            int index = registations.FindIndex(x => 
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

            if(index < 0)
            {
                message = $"Nimeä {name} ei löydy ilmouttautuneissa.";
                return false;
            }
            var removed = registations[index];
            registations.RemoveAt(index);
            registeredNames.Remove(removed.Name);

            // Poistetaan henkilö, joka on check-in tai jonossa
            checkIns.Remove(removed.Name);
            if(inQueue.Remove(removed.Name))
            {
                // Tekee jonon uudestaan ilman poistettua nimiä
                RebuildQueue(removed.Name);
            }
            message = "Ilmottautuminen poistettu";
            return true;
        }
        private void RebuildQueue(string nameToRemove)
        {
            // Rakennetaan jono uudelleen
            var temporaryQ = new Queue<string>();
            while(speakingQueue.Count > 0)
            {
                var listOfNames = speakingQueue.Dequeue();
                if (!string.Equals(listOfNames, nameToRemove, StringComparison.OrdinalIgnoreCase))
                    temporaryQ.Enqueue(listOfNames);
            }
            while (temporaryQ.Count > 0) speakingQueue.Enqueue(temporaryQ.Dequeue());
        }

        public IReadOnlyList<Participant> ListRegistrations() => registations;
    
        public bool CheckIn(string name, out string message)
        {
            if(!registeredNames.Contains(name))
            {
                message = $"Nimi {name} ei ole ilmoittautunut";
                return false;
            }
            if(checkIns.Contains(name))
            {
                message = $"Nimi {name} on jo kirjattu sisälle.";
                return false;
            }
            checkIns.Add(name); // lisää saapuneisiin
            message = "Kirjautuminen onnistui";
            return true;
        }

        // Kirjaa saapumisen vain ilmottautuneille ja estää tuplakirjautumisen
        public IReadOnlyCollection<string> ListCheckIns() => checkIns;
        // Palauttaa kaikki saapuneet

        public bool EnqueueSpeaker(string name, out string message)
        {
            if(!registeredNames.Contains(name))
            {
                message = $"Nimi {name} ei ole ilmottautunut, ei voi lisätä puhejonoon.";
                return false;
            }
            if(inQueue.Contains(name))
            {
                message = $"Nimi {name} on jo puhejonossa.";
                return false;
            }
            speakingQueue.Enqueue(name);
            inQueue.Add(name);
            message = $"Lisätty puhejonoon";
            return true;
        }
        public bool RemoveAfterSpeaking(out string? finishedName, out string message)
        {
            finishedName = null;
            if(speakingQueue.Count == 0)
            {
                message = "Puhenojono on tyhjä";
                return false;
            }
            finishedName = speakingQueue.Dequeue(); // poistetaan tämä jonosta
            inQueue.Remove(finishedName); // poistetaan inQueue:sta
            message = $"Puheenvuoro pidetty: {finishedName}";
            return true;
        }
        // Kurkkaa seuraavaan puhujan nimen poistamatta.
        public string? PeekNextSpeaker() => speakingQueue.Count == 0 ? null : speakingQueue.Peek();

        // Lista nykyisestä puhejonosta
        public IReadOnlyList<string> ListSpeakingQueue() => speakingQueue.ToList();
    }
    internal class Program
    {
        public static void Main(string[] args)
        {
            var system = new Event();

            while(true)
            {
                PrintMenu();

                Console.WriteLine($"\nValinta: ");
                var input = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch(input)
                {
                    case "1":
                        AddRegistrationUI(system);
                        break;
                    case "2":
                        RemoveRegistrationUI(system);
                        break;
                    case "3":
                        ListRegistrationsUI(system);
                        break;
                    case "4":
                        CheckInUI(system);
                        break;
                    case "5":
                        ListCheckInsUI(system);
                        break;
                    case "6":
                        EnqueueSpeakerUI(system);
                        break;
                    case "7":
                        NextSpeakerUI(system);
                        break;
                    case "8":
                        ShowNextSpeakerUI(system);
                        break;
                    case "9":
                        ShowSpeakingQueueUI(system);
                        break;
                    case "0":
                        Console.WriteLine($"Hei Hei!");
                        return;
                    default:
                        Console.WriteLine($"Tuntematon valinta. \n");
                        break;
                }
            }
        }
        private static void AddRegistrationUI(Event Esystem)
        {
            Console.WriteLine($"Nimi: ");
            var name = ReadNonEmpty();

            var participant = new Participant(name);
            if (Esystem.AddRegistration(participant, out string message))
                Console.WriteLine(message);
            else
                Console.WriteLine($"Virhe: {message}");
            Console.WriteLine();
        }
        private static void RemoveRegistrationUI(Event Esystem)
        {
            Console.Write("Poista Nimi: ");
            var name = ReadNonEmpty();

            if (Esystem.RemoveRegistration(name, out var message))
                Console.WriteLine($"{message}");
            else
                Console.WriteLine($"Virhe: {message}");
            Console.WriteLine();
        }
        private static void ListRegistrationsUI(Event Esystem)
        {
            var list = Esystem.ListRegistrations();
            if(list.Count == 0)
            {
                Console.WriteLine($"Ei ilmottautuneita.\n");
                return;
            }

            Console.WriteLine($"Ilmottautuneet (järjestyksessä).");
            int i = 1;
            foreach (var participant in list)
                Console.WriteLine($"{i++}. {participant}");
            Console.WriteLine();
        }
        private static void CheckInUI(Event Esystem)
        {
            Console.WriteLine($"Nimi kirjautumiseen: ");
            var name = ReadNonEmpty();

            if (Esystem.CheckIn(name, out var message))
                Console.WriteLine(message);
            else
                Console.WriteLine($"Virhe: {message}");
            Console.WriteLine();
        }
        private static void ListCheckInsUI(Event Esystem)
        {
            var set = Esystem.ListCheckIns();
            if(set.Count == 0)
            {
                Console.WriteLine("Ei Kirjautuneita.\n");
                return;
            }
            Console.WriteLine($"Kirjautuneet:");
            foreach (var n in set.OrderBy(x => x))
                Console.WriteLine($"- {n}");
            Console.WriteLine();
        }
        private static void EnqueueSpeakerUI(Event Esystem)
        {
            Console.WriteLine($"Nimi puhejonoon: ");
            var name = ReadNonEmpty();

            if (Esystem.EnqueueSpeaker(name, out var message))
                Console.WriteLine(message);
            else
                Console.WriteLine($"Virhe: {message}");
            Console.WriteLine();
        }
        private static void NextSpeakerUI(Event Esystem)
        {
            if (Esystem.RemoveAfterSpeaking(out var finished, out var message))
                Console.WriteLine($"Virhe: {message}");
            else
                Console.WriteLine($"Huom. {message}");
            Console.WriteLine();
        }
        private static void ShowNextSpeakerUI(Event Esystem)
        {
            var next = Esystem.PeekNextSpeaker();
            Console.WriteLine(next is null ? "Puheenjonossa ei ole seuraavaa." : $"Seuraava puhuja: {next}");
            Console.WriteLine();
        }
        private static void ShowSpeakingQueueUI(Event Esystem)
        {
            var queue = Esystem.ListSpeakingQueue();
            if(queue.Count == 0)
            {
                Console.WriteLine($"Puheenjono on tyhjä.\n");
                return;
            }
            Console.WriteLine($"Puheenvuorojono:");
            int position = 1;
            foreach (var next in queue)
                Console.WriteLine($"{position++}. {next}");
            Console.WriteLine();
        }

        private static void PrintMenu()
        {
            Console.WriteLine($"=== Tapahtuma Järjestelmä ===");
            Console.WriteLine($"[Ilmottautumiset]");
            Console.WriteLine($"1) Lisää osallistuja");
            Console.WriteLine($"2) Poista osallistuja");
            Console.WriteLine($"3) Näytä kaikki ilmoittautuneet (järjestyksessä)");
            
            Console.WriteLine($"\n[Kirjautumiset]");
            Console.WriteLine($"4) Kirjaa osallistuminen (saapuminen)");
            Console.WriteLine($"5) Listaa kirjautuneet");
            
            Console.WriteLine($"\n[Puheenvuorot]");
            Console.WriteLine($"6) Lisää puheenvuorojonoon");
            Console.WriteLine($"7) Poista seuraava puhuja (puheen jälkeen)");
            Console.WriteLine($"8) Näytä seuraava puhuja");
            Console.WriteLine($"9) Näytä puheenvuorojono");
            
            Console.WriteLine($"0) Lopeta");
        }
        private static string ReadNonEmpty()
        {
            while(true)
            {
                var s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
                Console.WriteLine("ei-tyhjä arvo: ");
            }
        }
    }
}