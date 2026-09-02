# Auto Clicker

A tiny Windows auto clicker with a simple UI. Clicks the left mouse button at the
current cursor position at a configurable rate — useful for idle/clicker games.

![icon](AutoClicker.ico)

## Download

Grab `AutoClicker.exe` from the
[latest release](https://github.com/wasomma/auto-clicker/releases/latest).
The exe is unsigned, so Windows SmartScreen may warn on first run — click
*More info → Run anyway*.

## Usage

Run `AutoClicker.exe` (no install, no dependencies — just .NET Framework, which ships
with Windows).

1. Set **Clicks per second** (1–60, default 15). Adjustable live while clicking.
2. Hover the mouse over the thing you want clicked.
3. Press **F6** to start/stop (works globally, even while the game window has focus),
   or use the Start/Stop button.

The window stays on top by default (checkbox to turn that off) and shows a running
click count. Closing the window stops everything.

If clicks don't register in a game, the game is probably running as administrator —
run AutoClicker as administrator too.

## Building from source

Compiles with the C# compiler that ships with Windows — no SDK required:

```bat
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /target:winexe ^
  /out:AutoClicker.exe /win32icon:AutoClicker.ico ^
  /r:System.Windows.Forms.dll /r:System.Drawing.dll AutoClicker.cs
```

## Also in this repo

- `autoclicker.ps1` + `Start Auto Clicker.bat` — the original console version
  (PowerShell, F6 start/stop, F7 quit), superseded by the exe.

## Note

Intended for idle/clicker games and single-player use. Online games with anti-cheat
may flag auto clickers — use at your own risk there.
