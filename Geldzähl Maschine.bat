@echo off
title Geldzählmaschine - Es ist Zahltag
setlocal enabledelayedexpansion

:: Verzeichnis angeben (aktuelles Verzeichnis)
set "dir=."

:: Zähler für das Dollarzeichen
set /a count=0

:: Alle Dateien im Verzeichnis und in Unterverzeichnissen durchsuchen
for /r "%dir%" %%f in (*) do (
    :: Jede Datei öffnen und nach "$" suchen
    for /f "delims=" %%a in ('findstr /c:"$" "%%f"') do (
        set /a count+=1
		echo %%f
    )
)
cls
color a
:: Dynamische und humorvolle Ausgabe basierend auf der Anzahl der Dollarzeichen
if %count%==0 (
    msg * "Computer: Dein Programm ist $%count% Wert. Wo sind die Dollar? Bist du sicher, dass du programmierst?"
) else if %count% LEQ 10 (
    msg * "Computer: Dein Programm ist $%count% Wert. Fast keine Dollar... als ob du das Programm in der Mittagspause geschrieben hast!"
) else if %count% LEQ 50 (
    msg * "Computer: Dein Programm ist $%count% Wert. Bisschen mehr Einsatz, sonst wird das nix mit deinem Code-Imperium!"
) else if %count% LEQ 100 (
    msg * "Computer: Dein Programm ist $%count% Wert. Nicht schlecht, aber noch lange nicht der nächste Code-Guru."
) else if %count% LEQ 200 (
    msg * "Computer: Dein Programm ist $%count% Wert. Hmm, könnte mehr sein. Aber hey, es geht in die richtige Richtung!"
) else if %count% LEQ 300 (
    msg * "Computer: Dein Programm ist $%count% Wert. Sehr gut, jetzt kommt der Profi in dir durch!"
) else if %count% GTR 400 (
    msg * "Computer: Dein Programm ist $%count% Wert. Wow, ein echtes Meisterwerk! Hast du ein Team hinter dir?"
)

exit