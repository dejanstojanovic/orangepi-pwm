#!/bin/bash
grep 'cpu ' /proc/stat | awk '{usage=($2+$4)*100/($2+$4+$5)} END {print usage "%"}' | grep -oP '(\d+(\.\d+)?(?=%))'