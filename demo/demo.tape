Output demo.gif
Set FontFamily "JetBrainsMono Nerd Font"
Set FontSize 18
Set Theme "GitHub Dark"
Set Padding 10
Set Width 1000
Set Height 800
Set Shell bash

Type './tagselecta read song.mp3'
Enter
Sleep 4s
Ctrl+L

Type './tagselecta write song.mp3 -g "Reggae; Dub" -N 5 -y 2024 -c "URL=https://cantti.bandcamp.com"'
Enter
Wait /Confirm/
Type y
Enter
Sleep 4s
Ctrl+L

Type './tagselecta discogs song.mp3 -r https://www.discogs.com/master/186940-King-Tubby-Dub-From-The-Roots'
Enter
Wait /Confirm/
Type y
Enter
Sleep 4s
Ctrl+L
