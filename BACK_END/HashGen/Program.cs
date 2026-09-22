using System;
using BCrypt.Net;
class Program
{
    static void Main()
    {
        string hash = BCrypt.Net.BCrypt.HashPassword("password123");
        Console.WriteLine(hash);
    }
}
