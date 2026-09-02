<#
  Auto Clicker
  Clicks the LEFT mouse button at wherever the cursor currently is.

  Run it, hover the mouse over the game button, then press F6.

  Hotkeys (global - they work while the game window has focus):
    F6  start / stop clicking
    F7  quit

  Options:
    -Cps <n>   clicks per second, 1-60 (default 15)
    example:   .\autoclicker.ps1 -Cps 25
#>
[CmdletBinding()]
param(
    [ValidateRange(1, 60)]
    [double]$Cps = 15
)

Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
public static class ClickerNative {
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
}
"@

$LEFTDOWN = 0x0002
$LEFTUP   = 0x0004
$VK_F6    = 0x75
$VK_F7    = 0x76

$intervalMs = 1000.0 / $Cps
$clicking   = $false
$clicks     = 0
$f6WasDown  = $false

Write-Host ("Auto clicker ready - {0} clicks/sec." -f $Cps)
Write-Host "Hover the mouse over the game button, then:"
Write-Host "  F6 = start / stop      F7 = quit"

$timer = [System.Diagnostics.Stopwatch]::StartNew()
$nextClickAt = 0.0

while ($true) {
    # F6 toggles clicking (edge-detected so holding it doesn't re-toggle)
    $f6Down = ([ClickerNative]::GetAsyncKeyState($VK_F6) -band 0x8000) -ne 0
    if ($f6Down -and -not $f6WasDown) {
        $clicking = -not $clicking
        if ($clicking) {
            $nextClickAt = $timer.Elapsed.TotalMilliseconds
            Write-Host "clicking ON"
        } else {
            Write-Host ("clicking OFF - {0} clicks so far" -f $clicks)
        }
    }
    $f6WasDown = $f6Down

    if (([ClickerNative]::GetAsyncKeyState($VK_F7) -band 0x8000) -ne 0) { break }

    if ($clicking) {
        $now = $timer.Elapsed.TotalMilliseconds
        if ($now -ge $nextClickAt) {
            [ClickerNative]::mouse_event($LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
            [ClickerNative]::mouse_event($LEFTUP,   0, 0, 0, [UIntPtr]::Zero)
            $clicks++
            $nextClickAt += $intervalMs
            # if the system lagged, skip ahead instead of burst-clicking to catch up
            if ($nextClickAt -lt $now) { $nextClickAt = $now }
        }
    }

    Start-Sleep -Milliseconds 10
}

Write-Host ("Stopped - {0} clicks total." -f $clicks)
