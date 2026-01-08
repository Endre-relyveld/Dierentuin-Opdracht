Deze handleiding beschrijft exact wat nodig is 
om het project succesvol lokaal te draaien.

1. Vereiste Software

.NET
.NET 8.0 SDK / Runtime
Moet geïnstalleerd zijn op Visual Studio

Dit kan via:
Visual Studio Installer (ASP.NET & web development workload)
Zonder .NET 8.0 zal het project niet starten.

(als er al eerder op .NET 8.0 is gewerkt of er zijn andere 
leerlingen beoordeeld voor deze opdracht is dit waarschijnlijk al gedaan)


2. Database Setup (SQL Server + Entity Framework)

De database moet worden gemigrate via Entity Framework migrations.

Stappen:
Open de .sln van het project in Visual Studio
2. bovenaan in de balk ga naar Tools → NuGet Package Manager → Package Manager Console
3. Plak exact het volgende en druk op Enter:

Update-Database



Klaar
- het programma is ready om gestart te worden
