# Blocker

## Dyknow blocker
This is a directory-based blacklist, this means that only the directories hardcoded in will be blocked. To run this, restart with airplane mode, log in, and wait until you're unblocked (DyKnow will take about 5 minutes to stop blocking you after a restart) then run the .exe file, then turn on wifi again and it will block DyKnow until either the next restart or until you right-click the icon in the taskbar and exit.

## For flash drive
To autorun in flash drive, add a file `autorun.inf` to your flash drive, content:

    [AutoRun]
    OPEN=BLOCKER.EXE
    UseAutoPlay=1
