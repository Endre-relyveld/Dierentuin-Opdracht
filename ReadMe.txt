Wat doet dit?

Update-Database 0
Update-Database

Deze twee regels resetten de database en maken hem opnieuw aan via Entity Framework migrations.
Wanneer je daarna het project start, seedt het project automatisch nieuwe data in de database.

Waar plak je dit in Visual Studio?

1. Open de sln van de repo met Visual Studio
2. Ga naar Tools → NuGet Package Manager → Package Manager Console
3. Onderaan verschijnt een consolevenster
4. Plak exact dit in de Package Manager Console en druk op Enter:

Update-Database 0
Update-Database

Klaar
- De database is leeg gemaakt
- Het schema is opnieuw opgebouwd
- Bij het starten van het project wordt automatisch nieuwe seed-data toegevoegd
