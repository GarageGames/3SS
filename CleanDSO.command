#!/bin/bash

cd `dirname "$0"`

echo -n "Deleting all compiled script files... "
find . -iname "*.edso" -delete
echo "done."

echo -n "Deleting all compiled script files... "
find . -iname "*.dso" -delete
echo "done."

echo -n "Deleting all cached fonts... "
find . -iname "*.uft" -delete
echo "done."

echo -n "Deleting all saved keybinds... "
find . -iname "bind.cs" -delete
echo "done."

echo -n "Deleting all saved preferences... "
find . -iname "prefs.cs" -delete
echo "done."

echo ""
echo "You may now close this window."
echo ""
