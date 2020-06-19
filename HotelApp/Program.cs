using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER to make a custom reservation \nor\nType in 't' to run test cases");
            string input = Console.ReadLine();
            if (input.Equals("t") || input.Equals("T"))
            {
                RunTestCases();
                return;
            }


            int size;
            Console.WriteLine("Hotel size: ");   
            
            if(!int.TryParse(Console.ReadLine(), out size) || size < 0)
            {
                Console.WriteLine("Positive number expected");
                return;
            }

            Hotel hotel = new Hotel(size);
            bool appIsRunning = true;
            int start;
            int end;
            while (appIsRunning)
            {
                try
                {
                    Console.Write("Start date: ");
                    start = int.Parse(Console.ReadLine());
                    Console.Write("End date: ");
                    end = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Positive numbers expected");
                    break;
                    // throw;
                }

                // Method will check for available rooms and output
                // wether the reservation was accepted or declined
                hotel.MakeReservation(start, end);

                Console.WriteLine("Press ENTER to make another reservation, type in 'stop' to exit app");
                if (Console.ReadLine().Equals("stop"))
                {
                    appIsRunning = false;
                }
            }
        }

        private static void RunTestCases()
        {
            Hotel hotel;
            // Test Case I
            Console.WriteLine("Test Case 1a/1b, outside planning periods");
            hotel = new Hotel(1);
            hotel.MakeReservation(-4, 2);
            Console.WriteLine("\nTest Case 2, size = 3");
            hotel = new Hotel(3);
            hotel.MakeReservation(0, 5);
            hotel.MakeReservation(7, 13);
            hotel.MakeReservation(3, 9);
            hotel.MakeReservation(5, 7);
            hotel.MakeReservation(6, 6);
            hotel.MakeReservation(0, 4);
            Console.WriteLine("\nTest Case 3, size = 3");
            hotel = new Hotel(3);
            hotel.MakeReservation(1, 3);
            hotel.MakeReservation(2, 5);
            hotel.MakeReservation(1, 9);
            hotel.MakeReservation(0, 15);
            Console.WriteLine("\nTest Case 4, size = 3");
            hotel = new Hotel(3);
            hotel.MakeReservation(1, 3);
            hotel.MakeReservation(0, 15);
            hotel.MakeReservation(1, 9);
            hotel.MakeReservation(2, 5);
            hotel.MakeReservation(4, 9);
            Console.WriteLine("\nTest Case 5, size = 2");
            hotel = new Hotel(2);
            hotel.MakeReservation(1, 3);
            hotel.MakeReservation(0, 4);
            hotel.MakeReservation(2, 3);
            hotel.MakeReservation(5, 5);
            hotel.MakeReservation(4, 10);
            hotel.MakeReservation(10, 10);
            hotel.MakeReservation(6, 7);
            hotel.MakeReservation(8, 10);
            hotel.MakeReservation(8, 9);
            Console.WriteLine("\n End of test cases...");
        }

        class Hotel
        {
            private List<Room> rooms;

            public Hotel(int size)
            {
                rooms = new List<Room>();
                for (int i = 0; i < size; i++)
                {
                    rooms.Add(new Room());
                }
            }

            public void MakeReservation(int start, int end)
            {
                if(start >= 0 && end <= 365)
                {
                    var availableRooms = rooms.Where(r => r.IsAvailable(start, end));
                    if(availableRooms.Count() != 0)
                    {
                        if (start > 0) // Prevents IndexOutOfRangeException
                        {
                            // Rooms with priority are the ones whose reservation ends one day before the requested 'start' date
                            // These rooms are prioritized in order to enable appending reservations
                            // Where one ends, the next one is placed next day
                            var priorityRooms = availableRooms.Where(r => r.days[start - 1] == 1);
                            if(priorityRooms.Count() != 0)
                            {
                                // If there is at least one such room available
                                // Place a reservation on the first one available
                                priorityRooms.FirstOrDefault().Reserve(start, end);
                                Console.WriteLine("ACCEPTED!");
                                return;
                            }
                        }
                        // If no priority rooms exist 
                        // A reservation is placed on the first available room
                        availableRooms.FirstOrDefault().Reserve(start, end);
                        Console.WriteLine("ACCEPTED!");
                        return;
                    }
                }
                
                // Either invalid numbers were requested 
                // Or no available rooms were found
                Console.WriteLine("DECLINED!");
            }
        }

        class Room
        {
            private static readonly int MAX_DAYS = 365;
            public int[] days;

            public Room()
            {
                // A room initally contains 365 days and all 'day' values are set to 0 by default
                // which means that a room is available
                // So, when a 'Room' instance is created, 365 days are automatically added and ready to be checked for further availability
                days = new int[MAX_DAYS];
            }

            public bool IsAvailable(int start, int end)
            {
                // Checks wether a room is available
                for(int i = start; i <= end; ++i)
                {
                    if(days[i] == 1)
                    {
                        // The day is reserved, room not available for requested days
                        return false;
                    }
                }

                // If 'for' loop ended without breaking, room is available for reservation
                return true;
            }

            public void Reserve(int start, int end)
            {
                // Fill the days within a room object as reserved
                for (int i = start; i <= end; ++i)
                {
                    days[i] = 1;
                }
            }
        }
    }
}